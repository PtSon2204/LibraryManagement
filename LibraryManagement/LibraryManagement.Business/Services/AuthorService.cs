using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.AuthorDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.Common;
using LibraryManagement.Data.UnitOfWorks;
using LibraryManagement.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Business.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AuthorDto>> GetAllAuthorsAsync()
        {
            var authors = await _unitOfWork.Authors.Query().AsNoTracking()
                .OrderBy(a => a.FullName)
                .Select(a => new AuthorDto
                {
                    AuthorId = a.AuthorId,
                    FullName = a.FullName
                })
                .ToListAsync();

            return authors;
        }

        public async Task<PagedResult<AuthorDto>> GetAuthorsAsync(string? search, int pageNumber, int pageSize)
        {
            IQueryable<Author> query = _unitOfWork.Authors.Query();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(a => a.FullName.Contains(search));

            var total = await query.CountAsync();

            var data = await query
                .OrderBy(a => a.FullName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AuthorDto
                {
                    AuthorId = a.AuthorId,
                    FullName = a.FullName
                })
                .ToListAsync();

            return new PagedResult<AuthorDto>
            {
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<AuthorDto?> GetAuthorByIdAsync(int id)
        {
            var author = await _unitOfWork.Authors.GetByIdAsync(id);
            if (author == null) return null;

            return new AuthorDto
            {
                AuthorId = author.AuthorId,
                FullName = author.FullName
            };
        }

        public async Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto dto)
        {
            var author = new Author
            {
                FullName = dto.FullName
            };

            await _unitOfWork.Authors.AddAsync(author);
            await _unitOfWork.SaveChangesAsync();

            return new AuthorDto
            {
                AuthorId = author.AuthorId,
                FullName = author.FullName
            };
        }

        public async Task<AuthorDto?> UpdateAuthorAsync(int id, UpdateAuthorDto dto)
        {
            var author = await _unitOfWork.Authors.GetByIdAsync(id);
            if (author == null) return null;

            author.FullName = dto.FullName;

            _unitOfWork.Authors.Update(author);
            await _unitOfWork.SaveChangesAsync();

            return new AuthorDto
            {
                AuthorId = author.AuthorId,
                FullName = author.FullName
            };
        }

        public async Task<bool> DeleteAuthorAsync(int id)
        {
            var author = await _unitOfWork.Authors.GetByIdAsync(id);
            if (author == null) return false;

            _unitOfWork.Authors.Delete(author);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
