using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Business.Interfaces
{
    public interface ILibraryPolicyService
    {
        string AvailableCopyStatus { get; }
        string BorrowedLoanStatus { get; }
        string PendingReservationStatus { get; }
        string UnpaidFineStatus { get; }
        int MaxBorrowDays { get; }
        int MaxBooksPerLoan { get; }
    }
}
