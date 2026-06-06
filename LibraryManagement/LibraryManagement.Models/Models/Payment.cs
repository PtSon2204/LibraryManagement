using System;
using System.Collections.Generic;

namespace LibraryManagement.Models.Models;

public partial class Payment
{
    public Guid PaymentId { get; set; }

    public Guid ReaderId { get; set; }

    public Guid? ProcessedByAccountId { get; set; }

    public decimal TotalAmount { get; set; }

    public string Method { get; set; } = null!;

    public string? Note { get; set; }

    public DateTime PaidAt { get; set; }

    // Navigation
    public virtual Reader Reader { get; set; } = null!;

    public virtual Account? ProcessedByAccount { get; set; }

    public virtual ICollection<Fine> Fines { get; set; } = new List<Fine>();
}
