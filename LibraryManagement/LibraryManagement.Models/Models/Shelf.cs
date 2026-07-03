using System;
using System.Collections.Generic;

namespace LibraryManagement.Models.Models;

public partial class Shelf
{
    public Guid ShelfId { get; set; }

    public Guid BookshelfId { get; set; }

    /// <summary>Số kệ trong giá (1, 2, 3, 4)</summary>
    public int ShelfNumber { get; set; }

    /// <summary>Tên kệ, VD: "Kệ 1"</summary>
    public string Name { get; set; } = null!;

    public virtual Bookshelf Bookshelf { get; set; } = null!;

    public virtual ICollection<ShelfSlot> ShelfSlots { get; set; } = new List<ShelfSlot>();
}
