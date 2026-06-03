using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagement.Data.Interfaces;
using LibraryManagement.Data.Repositories;
using LibraryManagement.Models.Context;

namespace LibraryManagement.Data.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IUserRepository UserRepository { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            UserRepository = new UserRepository(context);
        }

        public void Dispose() => _context.Dispose();
        

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>  _context.SaveChangesAsync(cancellationToken);
        
    }
}
