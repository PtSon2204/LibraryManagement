using LibraryManagement.Business.DTOs.DashboardDTO;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Business.Services
{
    public class StaffDashboardService : IStaffDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILibraryPolicyService _libraryPolicyService;

        public StaffDashboardService(IUnitOfWork unitOfWork, ILibraryPolicyService libraryPolicyService)
        {
            _unitOfWork = unitOfWork;
            _libraryPolicyService = libraryPolicyService;
        }

        public async Task<StaffDashboardDto> GetDashboardAsync()
        {
            var today = DateTime.Today;

            return new StaffDashboardDto
            {
                TotalBooks = await _unitOfWork.Books.Query().CountAsync(),

                TotalUsers = await _unitOfWork.Users.Query().CountAsync(),

                TotalCopies = await _unitOfWork.BookCopies.Query().CountAsync(),

                AvailableCopies = await _unitOfWork.BookCopies.Query()
                    .CountAsync(ac => ac.Status == _libraryPolicyService.AvailableCopyStatus),

                ActiveLoans = await _unitOfWork.Loans.Query()
                    .CountAsync(l => l.Status == _libraryPolicyService.BorrowedLoanStatus),

                PendingReservations = await _unitOfWork.Reservations.Query()
                    .CountAsync(r => r.Status == _libraryPolicyService.PendingReservationStatus),

                OverdueLoans = await _unitOfWork.Loans.Query()
                    .CountAsync(l => l.DueAt == today && l.Status == _libraryPolicyService.BorrowedLoanStatus),

                UnpaidFines = await _unitOfWork.Fines.Query()
                    .CountAsync(f => f.Status == _libraryPolicyService.UnpaidFineStatus),

                RecentLoans = await _unitOfWork.Loans.Query()
                    .OrderByDescending(l => l.BorrowedAt)
                    .Take(5)
                    .Select(l => new RecentLoanDto
                    {
                        LoanId = l.LoanId,
                        BorrowerName = l.BorrowerUser.FullName,
                        BorrowedAt = l.BorrowedAt,
                        DueAt = l.DueAt,
                        Status = l.Status
                    })
                    .ToListAsync(),

                RecentReservations = await _unitOfWork.Reservations.Query()
                    .OrderByDescending(r => r.ReservationDate)
                    .Take(5)
                    .Select(r => new RecentReservationDto
                    {
                        ReservationId = r.ReservationId,
                        UserName = r.User.FullName,
                        BookTitle = r.Book.Title,
                        ReservationDate = r.ReservationDate,
                        Status = r.Status
                    })
                    .ToListAsync()
            };

        }
    }
}
