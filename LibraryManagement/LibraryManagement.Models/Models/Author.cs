using System;
using System.Collections.Generic;

namespace LibraryManagement.Models.Models;

public partial class Author
{
    public int AuthorId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Biography { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    public virtual ICollection<BookAuthor> BookAuthors { get; set; }
    = new List<BookAuthor>();
}
