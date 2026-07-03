using System;

namespace LibraryManagement.Models.Models;

/// <summary>
/// Bảng trung gian: Giá sách (Bookshelf) chứa thể loại nào (Category)
/// </summary>
public partial class BookshelfCategory
{
    public Guid BookshelfId { get; set; }

    public int CategoryId { get; set; }

    public virtual Bookshelf Bookshelf { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;
}
