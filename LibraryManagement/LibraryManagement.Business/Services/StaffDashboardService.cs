using LibraryManagement.Business.DTOs.DashboardDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.UnitOfWorks;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Business.Services;

public class StaffDashboardService : IStaffDashboardService
{
    private readonly IUnitOfWork _unitOfWork;

    public StaffDashboardService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<StaffDashboardDto> GetDashboardAsync()
    {
        var today = DateTime.UtcNow.Date;

        return new StaffDashboardDto
        {
            TotalBooks = await _unitOfWork.Books.Query().CountAsync(),
            TotalUsers = await _unitOfWork.Readers.Query().CountAsync(),
            TotalCopies = await _unitOfWork.BookCopies.Query().CountAsync(),
            AvailableCopies = await _unitOfWork.BookCopies.Query().CountAsync(c => c.Status == "Available"),
            ActiveLoans = await _unitOfWork.Loans.Query().CountAsync(l => l.Status == "Borrowed"),
            PendingReservations = await _unitOfWork.Reservations.Query().CountAsync(r => r.Status == "Pending"),
            OverdueLoans = await _unitOfWork.Loans.Query().CountAsync(l => l.Status == "Borrowed" && l.DueAt.Date < today),
            UnpaidFines = await _unitOfWork.Fines.Query().CountAsync(f => f.Status == "Unpaid"),
            RecentLoans = await _unitOfWork.Loans.Query()
                .AsNoTracking()
                .OrderByDescending(l => l.BorrowedAt)
                .Take(5)
                .Select(l => new RecentLoanDto
                {
                    LoanId = l.LoanId,
                    BorrowerName = l.BorrowerReader.Profile != null ? l.BorrowerReader.Profile.FullName : l.BorrowerReader.Email,
                    BorrowedAt = l.BorrowedAt,
                    DueAt = l.DueAt,
                    Status = l.Status
                })
                .ToListAsync(),
            RecentReservations = await _unitOfWork.Reservations.Query()
                .AsNoTracking()
                .OrderByDescending(r => r.ReservationDate)
                .Take(5)
                .Select(r => new RecentReservationDto
                {
                    ReservationId = r.ReservationId,
                    UserName = r.Reader.Profile != null ? r.Reader.Profile.FullName : r.Reader.Email,
                    BookTitle = r.Room.RoomName,
                    ReservationDate = r.ReservationDate,
                    Status = r.Status
                })
                .ToListAsync()
        };
    }
}
