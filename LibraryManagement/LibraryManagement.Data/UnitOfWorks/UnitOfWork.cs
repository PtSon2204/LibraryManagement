using LibraryManagement.Data.Interfaces;
using LibraryManagement.Data.Repositories;
using LibraryManagement.Models.Context;
using LibraryManagement.Models.Models;

namespace LibraryManagement.Data.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IUserRepository UserRepository { get; }

        public IAuthorRepository AuthorRepository { get; }

        public IRepository<Book> Books { get; }

        public IRepository<BookCopy> BookCopies { get; }

        public IRepository<User> Users { get; }

        public IRepository<Loan> Loans { get; }

        public IRepository<LoanDetail> LoanDetails { get; }

        public IRepository<Reservation> Reservations { get; }

        public IRepository<Fine> Fines { get; }

        public IRepository<Payment> Payments { get; }

        public IRepository<Publisher> Publishers { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            UserRepository = new UserRepository(context);
            AuthorRepository = new AuthorRepository(context);

            Publishers = new Repository<Publisher>(context);
            Payments = new Repository<Payment>(context);

            Books = new Repository<Book>(context);
            BookCopies = new Repository<BookCopy>(context);
            Users = new Repository<User>(context);
            Loans = new Repository<Loan>(context);
            LoanDetails = new Repository<LoanDetail>(context);
            Reservations = new Repository<Reservation>(context);
            Fines = new Repository<Fine>(context);
        }

        public void Dispose() => _context.Dispose();
        

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>  _context.SaveChangesAsync(cancellationToken);
        
    }
}
