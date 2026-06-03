using LibraryManagement.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Business.Services
{
    public class LibraryPolicyService : ILibraryPolicyService
    {
        public string AvailableCopyStatus => "Available";
        public string BorrowedLoanStatus => "Borrowed";
        public string PendingReservationStatus => "Pending";
        public string UnpaidFineStatus => "Unpaid";
        public int MaxBorrowDays => 14;
        public int MaxBooksPerLoan => 5;
    }
}
