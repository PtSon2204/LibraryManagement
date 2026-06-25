using System;

namespace LibraryManagement.Models.Models;

public partial class Fine
{
    public Guid FineId { get; set; }

    public Guid LoanDetailId { get; set; }

    public Guid? PaymentId { get; set; }

    public decimal Amount { get; set; }

    public string Reason { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? PaidAt { get; set; }

    // Navigation
    public virtual LoanDetail LoanDetail { get; set; } = null!;

    public virtual Payment? Payment { get; set; }
}
