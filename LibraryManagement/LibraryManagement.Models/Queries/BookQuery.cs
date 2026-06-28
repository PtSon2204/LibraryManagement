using LibraryManagement.Data.Common;

namespace LibraryManagement.Models.Queries
{
    public class BookQuery : PaginationParams
    {
        public string? Title { get; set; }
        public string? SearchTerm { get; set; }
        public string? Publisher { get; set; }
        public int? PublisherId { get; set; }
        public int? PublicationYear { get; set; }
        public string? Language { get; set; }
        public bool AvailableOnly { get; set; }
        public string? SortBy { get; set; }
        public int? Page { get; set; }
        public bool? IncludeHidden { get; set; }
    }
}
