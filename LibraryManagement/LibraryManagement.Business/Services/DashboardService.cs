using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Business.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var now = DateTime.Now;

            var totalBooks = await _context.Books.CountAsync();
            var totalCopies = await _context.BookCopies.CountAsync();
            var totalMembers = await _context.Users.CountAsync(u => u.Role.RoleName == "User");

            var activeLoans = await _context.Loans.CountAsync(l => l.Status == "Borrowed");
            var overdueLoans = await _context.Loans.CountAsync(l => l.Status == "Borrowed" && l.DueAt < now);

            var totalFinesAmount = await _context.Fines
                .Where(f => f.Status == "Paid")
                .SumAsync(f => (decimal?)f.Amount) ?? 0;

            var pendingFinesAmount = await _context.Fines
                .Where(f => f.Status == "Unpaid")
                .SumAsync(f => (decimal?)f.Amount) ?? 0;

            // Recent 5 loans
            var recentLoansEntities = await _context.Loans
                .Include(l => l.BorrowerUser)
                .Include(l => l.LoanDetails)
                    .ThenInclude(ld => ld.Copy)
                        .ThenInclude(c => c.Book)
                .OrderByDescending(l => l.BorrowedAt)
                .Take(5)
                .ToListAsync();

            var recentLoans = recentLoansEntities.Select(l => new RecentLoanDto
            {
                LoanId = l.LoanId,
                BorrowedAt = l.BorrowedAt,
                Status = l.Status,
                BorrowerName = l.BorrowerUser?.FullName ?? "Unknown",
                BookTitle = string.Join(", ", l.LoanDetails.Select(ld => ld.Copy?.Book?.Title ?? "Unknown Book"))
            }).ToList();

            // Monthly loan counts for the current year
            var currentYear = DateTime.Now.Year;
            var monthlyData = await _context.Loans
                .Where(l => l.BorrowedAt.Year == currentYear)
                .GroupBy(l => l.BorrowedAt.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToListAsync();

            var monthlyStats = new List<MonthlyLoanStatsDto>();
            for (int m = 1; m <= 12; m++)
            {
                var monthData = monthlyData.FirstOrDefault(x => x.Month == m);
                monthlyStats.Add(new MonthlyLoanStatsDto
                {
                    MonthName = $"T{m}",
                    LoanCount = monthData?.Count ?? 0
                });
            }

            return new DashboardStatsDto
            {
                TotalBooks = totalBooks,
                TotalBookCopies = totalCopies,
                TotalMembers = totalMembers,
                ActiveLoans = activeLoans,
                OverdueLoans = overdueLoans,
                TotalFinesAmount = totalFinesAmount,
                PendingFinesAmount = pendingFinesAmount,
                RecentLoans = recentLoans,
                MonthlyLoanStats = monthlyStats
            };
        }
    }
}
