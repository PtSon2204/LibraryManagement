using System;

namespace LibraryManagement.Models.Models;

public partial class Fine
{
    public Guid FineId { get; set; }

    /// <summary>
    /// FK → LoanDetails. Mọi khoản phạt đều phải thuộc 1 lần mượn cụ thể.
    /// Lấy Reader qua: Fine → LoanDetail → Loan → BorrowerReader.
    /// </summary>
    public Guid LoanDetailId { get; set; }

    /// <summary>
    /// FK → Payments (nullable).
    /// NULL  = chưa thanh toán (Status = Unpaid / Waived).
    /// Có giá trị = đã được thanh toán trong phiên Payment này.
    /// </summary>
    public Guid? PaymentId { get; set; }

    /// <summary>Số tiền phạt cho khoản này.</summary>
    public decimal Amount { get; set; }

    /// <summary>Lý do phạt, vd: "Trả trễ 5 ngày", "Rách trang 50", "Hỏng bìa".</summary>
    public string Reason { get; set; } = null!;

    /// <summary>Unpaid | Paid | Waived</summary>
    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? PaidAt { get; set; }

    // Navigation
    public virtual LoanDetail LoanDetail { get; set; } = null!;

    public virtual Payment? Payment { get; set; }
}
