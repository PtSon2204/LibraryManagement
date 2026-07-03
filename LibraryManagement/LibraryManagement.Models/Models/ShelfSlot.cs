using System;
using System.Collections.Generic;

namespace LibraryManagement.Models.Models;

public partial class ShelfSlot
{
    public Guid SlotId { get; set; }

    public Guid ShelfId { get; set; }

    /// <summary>Mã ô, VD: "S01", "S02"</summary>
    public string SlotCode { get; set; } = null!;

    /// <summary>Sức chứa tối đa (tổng số bản sao có thể đặt vào ô này)</summary>
    public int Capacity { get; set; }

    public string? Description { get; set; }

    public virtual Shelf Shelf { get; set; } = null!;

    /// <summary>Các bản sao sách đang nằm trong ô này</summary>
    public virtual ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>();
}
