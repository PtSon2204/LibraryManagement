using LibraryManagement.Business.DTOs.DashboardDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Business.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardDto> GetDashboardStatsAsync()
        {
            var totalBooks = await _unitOfWork.Books.Query().CountAsync();
            
            var totalReaders = await _unitOfWork.Accounts.Query().Where(a => a.Role == "Reader").CountAsync();
            
            var activeLoans = await _unitOfWork.Loans.Query().Where(l => l.Status == "Borrowed").CountAsync();
            
            var overdueLoans = await _unitOfWork.Loans.Query()
                .Where(l => l.Status == "Borrowed" && l.DueAt < DateTime.Now)
                .CountAsync();

            return new DashboardDto
            {
                TotalBooks = totalBooks,
                TotalReaders = totalReaders,
                ActiveLoans = activeLoans,
                OverdueLoans = overdueLoans
            };
        }
    }
}
