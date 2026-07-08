using System;

namespace LibraryManagement.Models.Models;

/// <summary>
/// Mẫu khoản phạt: định nghĩa các loại phạt (rách sách, ướt sách, mất sách, quá hạn...).
/// </summary>
public partial class FineTemplate
{
    public Guid FineTemplateId { get; set; }

    /// <summary>Tên loại phạt (VD: "Sách bị rách bìa", "Quá hạn trả sách"...)</summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Số tiền phạt cố định. Với loại "Overdue" thì đây là đơn giá/ngày.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Loại tính tiền: "Fixed" = cố định, "PerDay" = nhân số ngày quá hạn.
    /// </summary>
    public string FineType { get; set; } = "Fixed";

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
}
