using System;
using System.Collections.Generic;

namespace LibraryManagement.Models.Models;

public partial class Bookshelf
{
    public Guid BookshelfId { get; set; }

    public Guid FloorId { get; set; }

    /// <summary>Mã giá, VD: "A", "B", "C", "D"</summary>
    public string ShelfCode { get; set; } = null!;

    /// <summary>Tên giá, VD: "Giá A - Khoa học"</summary>
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual Floor Floor { get; set; } = null!;

    public virtual ICollection<Shelf> Shelves { get; set; } = new List<Shelf>();

    /// <summary>Các thể loại sách được xếp trên giá này</summary>
    public virtual ICollection<BookshelfCategory> BookshelfCategories { get; set; } = new List<BookshelfCategory>();
}
