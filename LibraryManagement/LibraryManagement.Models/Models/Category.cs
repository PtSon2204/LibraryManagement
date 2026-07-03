using System;
using System.Collections.Generic;

namespace LibraryManagement.Models.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();

    public virtual ICollection<BookshelfCategory> BookshelfCategories { get; set; } = new List<BookshelfCategory>();
}
