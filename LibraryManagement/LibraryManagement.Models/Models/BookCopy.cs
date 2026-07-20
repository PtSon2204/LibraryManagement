using System;
using System.Collections.Generic;

namespace LibraryManagement.Models.Models;

public partial class BookCopy
{
    public Guid CopyId { get; set; }

    public Guid BookId { get; set; }

    public string Barcode { get; set; } = null!;

    public string Status { get; set; } = null!;

    /// <summary>Giá thay thế áp dụng khi bản sao bị mất hoặc hư hỏng không thể sử dụng.</summary>
    public decimal ReplacementPrice { get; set; }

    /// <summary>FK đến ShelfSlot — ô kệ vật lý chứa bản sao này (nullable)</summary>
    public Guid? ShelfSlotId { get; set; }

    public DateOnly AddedDate { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual ShelfSlot? ShelfSlot { get; set; }

    public virtual LoanDetail? LoanDetail { get; set; }
}
