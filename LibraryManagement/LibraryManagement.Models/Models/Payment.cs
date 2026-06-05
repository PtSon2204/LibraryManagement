using System;
using System.Collections.Generic;

namespace LibraryManagement.Models.Models;

public partial class Payment
{
    public Guid PaymentId { get; set; }

    /// <summary>
    /// FK → Readers. Độc giả thực hiện thanh toán.
    /// Denormalized để query lịch sử thanh toán nhanh, tránh JOIN qua Fine → LoanDetail → Loan.
    /// </summary>
    public Guid ReaderId { get; set; }

    /// <summary>
    /// FK → Accounts (nullable).
    /// Thủ thư / Admin xác nhận nhận tiền mặt.
    /// NULL nếu tương lai hỗ trợ tự thanh toán online (QR, VNPay...).
    /// </summary>
    public Guid? ProcessedByAccountId { get; set; }

    /// <summary>
    /// Tổng tiền của phiên thanh toán này (= tổng tất cả Fine Unpaid của Reader tại thời điểm đó).
    /// Lưu lại làm bằng chứng snapshot, tránh sai lệch nếu Fine bị sửa sau.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>Cash | BankTransfer | QRCode</summary>
    public string Method { get; set; } = null!;

    /// <summary>Ghi chú thêm nếu cần (vd: "Đã kiểm tiền mặt, đủ số").</summary>
    public string? Note { get; set; }

    public DateTime PaidAt { get; set; }

    // Navigation
    public virtual Reader Reader { get; set; } = null!;

    public virtual Account? ProcessedByAccount { get; set; }

    /// <summary>Các khoản phạt được thanh toán trong phiên này.</summary>
    public virtual ICollection<Fine> Fines { get; set; } = new List<Fine>();
}
