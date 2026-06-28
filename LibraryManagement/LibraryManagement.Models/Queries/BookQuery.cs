using System;
using LibraryManagement.Data.Common;

namespace LibraryManagement.Models.Queries
{
    public class BookQuery : PaginationParams
    {
        public string? SearchTerm { get; set; }
        public int? PublisherId { get; set; }
        public int? PublicationYear { get; set; }
        public string? Language { get; set; }
        public bool? IncludeHidden { get; set; }
    }
}
