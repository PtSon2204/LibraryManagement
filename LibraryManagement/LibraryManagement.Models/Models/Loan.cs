using System;
using System.Collections.Generic;

namespace LibraryManagement.Models.Models;

public partial class Loan
{
    public Guid LoanId { get; set; }

    public Guid BorrowerUserId { get; set; }

    public Guid? ProcessedByUserId { get; set; }

    public DateTime BorrowedAt { get; set; }

    public DateTime DueAt { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User BorrowerUser { get; set; } = null!;

    public virtual ICollection<LoanDetail> LoanDetails { get; set; } = new List<LoanDetail>();

    public virtual User? ProcessedByUser { get; set; }
}
