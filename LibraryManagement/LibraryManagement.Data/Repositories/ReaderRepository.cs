using System;
using System.Threading.Tasks;
using LibraryManagement.Data.Interfaces;
using LibraryManagement.Models.Context;
using LibraryManagement.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Data.Repositories
{
    public class ReaderRepository : IReaderRepository
    {
        private readonly ApplicationDbContext _context;

        public ReaderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Reader?> GetReaderByEmailAsync(string email)
        {
            return await _context.Readers
                .AsNoTracking()
                .Include(r => r.Profile)
                .FirstOrDefaultAsync(r => r.Email == email);
        }

        public async Task<Reader?> GetReaderByIdAsync(Guid readerId)
        {
            return await _context.Readers
                .AsNoTracking()
                .Include(r => r.Profile)
                .FirstOrDefaultAsync(r => r.ReaderId == readerId);
        }

        public async Task AddReaderAsync(Reader reader)
        {
            await _context.Readers.AddAsync(reader);
        }

        public void UpdateReader(Reader reader)
        {
            _context.Readers.Update(reader);
        }
    }
}
