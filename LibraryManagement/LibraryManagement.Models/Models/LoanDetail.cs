using System;
using System.Collections.Generic;

namespace LibraryManagement.Models.Models;

public partial class LoanDetail
{
    public Guid LoanDetailId { get; set; }

    public Guid LoanId { get; set; }

    public Guid CopyId { get; set; }

    public DateTime? ReturnedAt { get; set; }

    public string Status { get; set; } = null!;

    public virtual BookCopy Copy { get; set; } = null!;

    public virtual ICollection<Fine> Fines { get; set; } = new List<Fine>();

    public virtual Loan Loan { get; set; } = null!;
}
