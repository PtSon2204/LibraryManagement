using System;
using System.Collections.Generic;

namespace LibraryManagement.Business.DTOs.LoanDTOs
{
    public class LoanHistoryDto
    {
        public Guid LoanId { get; set; }
        public DateTime BorrowedAt { get; set; }
        public DateTime DueAt { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string? ProcessedByLibrarian { get; set; }
        
        public List<LoanDetailHistoryDto> LoanDetails { get; set; } = new List<LoanDetailHistoryDto>();
    }
}
