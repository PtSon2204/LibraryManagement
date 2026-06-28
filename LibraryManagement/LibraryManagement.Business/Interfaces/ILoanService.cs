using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Business.Services
{
    public interface ILoanService
    {
        Task<Data.Common.PagedResult<DTOs.LoanDTOs.LoanHistoryDto>> GetReaderLoanHistoryAsync(Guid readerId, Models.Queries.LoanQuery query);
        Task<DTOs.LoanDTOs.LoanHistoryDto?> GetLoanDetailByIdAsync(Guid loanId);
    }
}
