using LibraryManagement.Data.Interfaces;
using LibraryManagement.Data.Repositories;
using LibraryManagement.Models.Context;
using LibraryManagement.Models.Models;

namespace LibraryManagement.Data.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IReaderRepository ReaderRepository { get; }
        public IAccountRepository AccountRepository { get; }

        public IRepository<Book> Books { get; }
        public IRepository<BookCopy> BookCopies { get; }
        public IRepository<Reader> Readers { get; }
        public IRepository<Account> Accounts { get; }
        public IRepository<UserProfile> UserProfiles { get; }
        public IRepository<Loan> Loans { get; }
        public IRepository<LoanDetail> LoanDetails { get; }
        public IRepository<Reservation> Reservations { get; }
        public IRepository<Room> Rooms { get; }
        public IRepository<Fine> Fines { get; }
        public IRepository<Payment> Payments { get; }

        public IRepository<Author> Authors { get; }

        public IRepository<Publisher> Publishers { get; }
        public IRepository<Category> Categories { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            Publishers = new Repository<Publisher>(context);
            Categories = new Repository<Category>(context);
            ReaderRepository = new ReaderRepository(context);
            AccountRepository = new AccountRepository(context);

            Authors = new Repository<Author>(context);

            Books = new Repository<Book>(context);
            BookCopies = new Repository<BookCopy>(context);
            Readers = new Repository<Reader>(context);
            Accounts = new Repository<Account>(context);
            UserProfiles = new Repository<UserProfile>(context);
            Loans = new Repository<Loan>(context);
            LoanDetails = new Repository<LoanDetail>(context);
            Reservations = new Repository<Reservation>(context);
            Rooms = new Repository<Room>(context);
            Fines = new Repository<Fine>(context);
            Payments = new Repository<Payment>(context);
        }

        public void Dispose() => _context.Dispose();

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => _context.SaveChangesAsync(cancellationToken);
    }
}
