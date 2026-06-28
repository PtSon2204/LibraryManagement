using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Business.Services
{
    public class LoanService : ILoanService
    {
        private readonly Data.UnitOfWorks.IUnitOfWork _unitOfWork;

        public LoanService(Data.UnitOfWorks.IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Data.Common.PagedResult<DTOs.LoanDTOs.LoanHistoryDto>> GetReaderLoanHistoryAsync(Guid readerId, Models.Queries.LoanQuery query)
        {
            var dbQuery = _unitOfWork.Loans.Query()
                .Where(l => l.BorrowerReaderId == readerId);

            if (!string.IsNullOrEmpty(query.Status))
            {
                dbQuery = dbQuery.Where(l => l.Status == query.Status);
            }
            if (query.FromDate.HasValue)
            {
                dbQuery = dbQuery.Where(l => l.BorrowedAt >= query.FromDate.Value);
            }
            if (query.ToDate.HasValue)
            {
                dbQuery = dbQuery.Where(l => l.BorrowedAt <= query.ToDate.Value);
            }

            // Include related tables for filtering if needed, but here search might be over book titles
            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                var term = query.SearchTerm.ToLower();
                dbQuery = dbQuery.Where(l => l.LoanDetails.Any(ld => 
                    ld.Copy.Book.Title.ToLower().Contains(term) || 
                    ld.Copy.Barcode.ToLower().Contains(term)));
            }

            int totalCount = dbQuery.Count();

            // Paging
            int pageNumber = query.PageNumber > 0 ? query.PageNumber : 1;
            int pageSize = query.PageSize > 0 ? query.PageSize : 10;
            var items = dbQuery
                .OrderByDescending(l => l.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new DTOs.LoanDTOs.LoanHistoryDto
                {
                    LoanId = l.LoanId,
                    BorrowedAt = l.BorrowedAt,
                    DueAt = l.DueAt,
                    Status = l.Status,
                    CreatedAt = l.CreatedAt,
                    ProcessedByLibrarian = l.ProcessedByAccount != null ? (l.ProcessedByAccount.Profile != null ? l.ProcessedByAccount.Profile.FullName : l.ProcessedByAccount.Email) : null,
                    LoanDetails = l.LoanDetails.Select(ld => new DTOs.LoanDTOs.LoanDetailHistoryDto
                    {
                        LoanDetailId = ld.LoanDetailId,
                        CopyId = ld.CopyId,
                        BookTitle = ld.Copy.Book.Title,
                        Barcode = ld.Copy.Barcode,
                        CoverImageUrl = ld.Copy.Book.CoverImageUrl,
                        ReturnedAt = ld.ReturnedAt,
                        Status = ld.Status
                    }).ToList()
                })
                .ToList();

            return new Data.Common.PagedResult<DTOs.LoanDTOs.LoanHistoryDto>
            {
                Data = items,
                TotalRecords = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<DTOs.LoanDTOs.LoanHistoryDto?> GetLoanDetailByIdAsync(Guid loanId)
        {
            var l = _unitOfWork.Loans.Query()
                .Where(x => x.LoanId == loanId)
                .Select(x => new DTOs.LoanDTOs.LoanHistoryDto
                {
                    LoanId = x.LoanId,
                    BorrowedAt = x.BorrowedAt,
                    DueAt = x.DueAt,
                    Status = x.Status,
                    CreatedAt = x.CreatedAt,
                    ProcessedByLibrarian = x.ProcessedByAccount != null ? (x.ProcessedByAccount.Profile != null ? x.ProcessedByAccount.Profile.FullName : x.ProcessedByAccount.Email) : null,
                    LoanDetails = x.LoanDetails.Select(ld => new DTOs.LoanDTOs.LoanDetailHistoryDto
                    {
                        LoanDetailId = ld.LoanDetailId,
                        CopyId = ld.CopyId,
                        BookTitle = ld.Copy.Book.Title,
                        Barcode = ld.Copy.Barcode,
                        CoverImageUrl = ld.Copy.Book.CoverImageUrl,
                        ReturnedAt = ld.ReturnedAt,
                        Status = ld.Status
                    }).ToList()
                }).FirstOrDefault();

            return await Task.FromResult(l);
        }
    }
}
