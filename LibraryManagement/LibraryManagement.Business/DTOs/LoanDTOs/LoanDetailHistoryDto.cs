using System;
using System.Collections.Generic;

namespace LibraryManagement.Business.DTOs.LoanDTOs
{
    public class LoanDetailHistoryDto
    {
        public Guid LoanDetailId { get; set; }
        public Guid CopyId { get; set; }
        public string BookTitle { get; set; } = null!;
        public string Barcode { get; set; } = null!;
        public string? CoverImageUrl { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public string Status { get; set; } = null!;
    }
}
