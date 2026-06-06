using LibraryManagement.Data.Interfaces;
using LibraryManagement.Models.Models;
using System.Threading.Tasks;

namespace LibraryManagement.Data.UnitOfWorks
{
    public interface IUnitOfWork
    {
        // Repositories chuyên biệt
        IReaderRepository ReaderRepository { get; }
        IAccountRepository AccountRepository { get; }

        // Generic repositories
        IRepository<Author> Authors { get; }
        IRepository<Book> Books { get; }
        IRepository<BookCopy> BookCopies { get; }
        IRepository<Reader> Readers { get; }
        IRepository<Account> Accounts { get; }
        IRepository<UserProfile> UserProfiles { get; }
        IRepository<Loan> Loans { get; }
        IRepository<LoanDetail> LoanDetails { get; }
        IRepository<Reservation> Reservations { get; }
        IRepository<Room> Rooms { get; }
        IRepository<Fine> Fines { get; }
        IRepository<Payment> Payments { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
