using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Models.Models
{
    public class BookCategory
    {
        public Guid BookId { get; set; }
        public int CategoryId { get; set; }

        public Book Book { get; set; } = null!;
        public Category Category { get; set; } = null!;
    }
}
