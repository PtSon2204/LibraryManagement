using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.EmailDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.UnitOfWorks;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Business.Services;

public class OverdueNotificationService : IOverdueNotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;

    public OverdueNotificationService(IUnitOfWork unitOfWork, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task<OverdueNotificationResultDto> SendOverdueNotificationsAsync()
    {
        var today = DateTime.UtcNow.Date;

        var overdueLoanDetails = await _unitOfWork.LoanDetails.Query()
            .Include(d => d.Loan)
                .ThenInclude(l => l.BorrowerReader)
                    .ThenInclude(r => r.Profile)
            .Include(d => d.Copy)
                .ThenInclude(c => c.Book)
            .Where(d => d.Status == "Borrowed" && d.Loan.DueAt.Date < today)
            .ToListAsync();

        if (!overdueLoanDetails.Any())
        {
            return new OverdueNotificationResultDto
            {
                TotalOverdueLoans = 0,
                TotalReadersNotified = 0,
                NotifiedAt = DateTime.UtcNow
            };
        }

        var uniqueLoans = overdueLoanDetails.Select(d => d.Loan).DistinctBy(l => l.LoanId).ToList();
        
        foreach (var loan in uniqueLoans)
        {
            if (loan.Status == "Borrowed")
            {
                loan.Status = "Overdue";
                loan.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Loans.Update(loan);
            }
        }

        foreach (var detail in overdueLoanDetails)
        {
             if (detail.Status == "Borrowed")
             {
                 detail.Status = "Overdue";
                 _unitOfWork.LoanDetails.Update(detail);
             }
        }
        
        // Group by Reader
        var detailsByReader = overdueLoanDetails.GroupBy(d => d.Loan.BorrowerReader);

        int readersNotified = 0;

        foreach (var group in detailsByReader)
        {
            var reader = group.Key;
            if (string.IsNullOrWhiteSpace(reader.Email)) continue;

            var fullName = reader.Profile?.FullName ?? reader.Email;
            
            var emailBody = BuildEmailBody(fullName, group.ToList(), today);

            try
            {
                await _emailService.SendEmailAsync(
                    reader.Email,
                    "THÔNG BÁO SÁCH QUÁ HẠN TỪ THƯ VIỆN",
                    emailBody
                );
                readersNotified++;
            }
            catch (Exception)
            {
                // In a production app, log the exception.
            }
        }

        // Save changes to DB for the status updates
        await _unitOfWork.SaveChangesAsync();

        return new OverdueNotificationResultDto
        {
            TotalOverdueLoans = overdueLoanDetails.Count,
            TotalReadersNotified = readersNotified,
            NotifiedAt = DateTime.UtcNow
        };
    }

    private string BuildEmailBody(string fullName, System.Collections.Generic.List<LibraryManagement.Models.Models.LoanDetail> details, DateTime today)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"<h3>Kính gửi {fullName},</h3>");
        sb.AppendLine("<p>Hệ thống thư viện xin thông báo bạn có những quyển sách đã <strong>quá hạn trả</strong>. Vui lòng mang sách đến trả tại thư viện trong thời gian sớm nhất để tránh phí phạt.</p>");
        
        sb.AppendLine("<table border='1' cellpadding='5' cellspacing='0' style='border-collapse: collapse; width: 100%;'>");
        sb.AppendLine("<thead><tr style='background-color: #f2f2f2;'>");
        sb.AppendLine("<th>Tên sách</th>");
        sb.AppendLine("<th>Mã vạch (Barcode)</th>");
        sb.AppendLine("<th>Ngày mượn</th>");
        sb.AppendLine("<th>Ngày phải trả</th>");
        sb.AppendLine("<th>Số ngày trễ</th>");
        sb.AppendLine("</tr></thead><tbody>");

        foreach (var item in details)
        {
            var daysLate = (today - item.Loan.DueAt.Date).Days;
            
            sb.AppendLine("<tr>");
            sb.AppendLine($"<td>{item.Copy.Book.Title}</td>");
            sb.AppendLine($"<td>{item.Copy.Barcode}</td>");
            sb.AppendLine($"<td>{item.Loan.BorrowedAt:dd/MM/yyyy}</td>");
            sb.AppendLine($"<td>{item.Loan.DueAt:dd/MM/yyyy}</td>");
            sb.AppendLine($"<td style='color: red; font-weight: bold;'>{daysLate} ngày</td>");
            sb.AppendLine("</tr>");
        }

        sb.AppendLine("</tbody></table>");
        sb.AppendLine("<br/>");
        sb.AppendLine("<p>Xin cảm ơn và chúc bạn một ngày tốt lành.</p>");
        sb.AppendLine("<p><strong>Ban quản lý Thư viện</strong></p>");

        return sb.ToString();
    }
}
