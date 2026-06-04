using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Data.Interfaces;
using LibraryManagement.Models.Context;
using LibraryManagement.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Data.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Author>> GetAllAuthorsAsync(string? search)
        {
            var query = _context.Authors.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a => a.FullName.Contains(search));
            }
            return await query.ToListAsync();
        }

        public async Task<Author?> GetAuthorByIdAsync(int authorId)
        {
            return await _context.Authors
                .FirstOrDefaultAsync(a => a.AuthorId == authorId);
        }

        public async Task AddAuthorAsync(Author author)
        {
            await _context.Authors.AddAsync(author);
        }

        public async Task UpdateAuthorAsync(Author author)
        {
            _context.Authors.Update(author);
            await Task.CompletedTask;
        }

        public async Task DeleteAuthorAsync(int authorId)
        {
            var author = await _context.Authors.FindAsync(authorId);
            if (author != null)
            {
                _context.Authors.Remove(author);
            }
        }

        public async Task<bool> HasBooksAsync(int authorId)
        {
            return await _context.BookAuthors
                .AnyAsync(ba => ba.AuthorId == authorId);
        }
    }
}
