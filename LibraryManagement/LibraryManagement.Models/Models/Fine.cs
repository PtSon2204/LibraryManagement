using System;
using System.Collections.Generic;

namespace LibraryManagement.Models.Models;

public partial class Fine
{
    public Guid FineId { get; set; }

    /// <summary>FK → Readers. Chỉ độc giả mới bị phạt.</summary>
    public Guid ReaderId { get; set; }

    public Guid? LoanDetailId { get; set; }

    public decimal Amount { get; set; }

    public string Reason { get; set; } = null!;

    /// <summary>Unpaid | Paid | Waived</summary>
    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? PaidAt { get; set; }

    // Navigation
    public virtual Reader Reader { get; set; } = null!;

    public virtual LoanDetail? LoanDetail { get; set; }
}
