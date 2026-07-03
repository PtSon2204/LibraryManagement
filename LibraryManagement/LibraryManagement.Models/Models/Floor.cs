using System;
using System.Collections.Generic;

namespace LibraryManagement.Models.Models;

public partial class Floor
{
    public Guid FloorId { get; set; }

    /// <summary>Số tầng (1, 2, 3, ...)</summary>
    public int FloorNumber { get; set; }

    /// <summary>Tên hiển thị, VD: "Tầng 1"</summary>
    public string FloorName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Bookshelf> Bookshelves { get; set; } = new List<Bookshelf>();
}
