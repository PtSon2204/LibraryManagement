using System;
using System.Collections.Generic;

namespace LibraryManagement.Models.Models;

public partial class Book
{
    public Guid BookId { get; set; }

    public string Title { get; set; } = null!;

    public string? ISBN { get; set; }

    public int? PublisherId { get; set; }

    public int? PublicationYear { get; set; }

    public string? Language { get; set; }

    public string? Edition { get; set; }

    public string? Description { get; set; }

    public string? CoverImageUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>();

    public virtual Publisher? Publisher { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<BookAuthor> BookAuthors { get; set; }
        = new List<BookAuthor>();

    public virtual ICollection<BookCategory> BookCategories { get; set; }
        = new List<BookCategory>();
}
