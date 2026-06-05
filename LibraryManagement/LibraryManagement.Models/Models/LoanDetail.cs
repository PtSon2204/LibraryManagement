using System;
using System.Collections.Generic;

namespace LibraryManagement.Models.Models;

public partial class LoanDetail
{
    public Guid LoanDetailId { get; set; }

    public Guid LoanId { get; set; }

    public Guid CopyId { get; set; }

    public DateTime? ReturnedAt { get; set; }

    /// <summary>Borrowed | Returned | Overdue | Lost</summary>
    public string Status { get; set; } = null!;

    // Navigation
    public virtual BookCopy Copy { get; set; } = null!;

    public virtual Loan Loan { get; set; } = null!;

    /// <summary>Một lần mượn 1 quyển sách có thể phát sinh nhiều khoản phạt.</summary>
    public virtual ICollection<Fine> Fines { get; set; } = new List<Fine>();
}
