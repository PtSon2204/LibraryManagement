using LibraryManagement.Data.Interfaces;
using LibraryManagement.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Data.UnitOfWorks
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IRepository<Book> Books { get; }
        IRepository<BookCopy> BookCopies { get; }
        IRepository<User> Users { get; }
        IRepository<Loan> Loans { get; }
        IRepository<LoanDetail> LoanDetails { get; }
        IRepository<Reservation> Reservations { get; }
        IRepository<Fine> Fines { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
