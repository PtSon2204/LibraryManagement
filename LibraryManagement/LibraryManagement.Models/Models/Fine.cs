using System;
using System.Collections.Generic;

namespace LibraryManagement.Models.Models;

public partial class Fine
{
    public Guid FineId { get; set; }

    public Guid UserId { get; set; }

    public Guid? LoanDetailId { get; set; }

    public decimal Amount { get; set; }

    public string Reason { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? PaidAt { get; set; }

    public virtual LoanDetail? LoanDetail { get; set; }

    public virtual User User { get; set; } = null!;
}
