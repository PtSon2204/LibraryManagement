using System;
using System.Collections.Generic;

namespace LibraryManagement.Models.Models;

public partial class Loan
{
    public Guid LoanId { get; set; }

    /// <summary>FK → Readers. Reader (độc giả) mượn sách.</summary>
    public Guid BorrowerReaderId { get; set; }

    /// <summary>FK → Accounts. Librarian/Admin duyệt phiếu mượn.</summary>
    public Guid? ProcessedByAccountId { get; set; }

    public DateTime BorrowedAt { get; set; }

    public DateTime DueAt { get; set; }

    /// <summary>Borrowed | Returned | Overdue | Lost</summary>
    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public virtual Reader BorrowerReader { get; set; } = null!;

    public virtual Account? ProcessedByAccount { get; set; }

    public virtual ICollection<LoanDetail> LoanDetails { get; set; } = new List<LoanDetail>();
}
