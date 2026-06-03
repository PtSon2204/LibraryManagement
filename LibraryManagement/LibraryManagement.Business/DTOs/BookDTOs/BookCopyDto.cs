using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Business.DTOs.BookDTOs
{
    public class BookCopyDto
    {
        public Guid CopyId { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Location { get; set; }
        public DateOnly AddedDate { get; set; }
    }
}
