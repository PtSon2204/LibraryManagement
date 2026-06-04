using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.UnitOfWorks;
using LibraryManagement.Models.Models;

namespace LibraryManagement.Business.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<AuthorDto>> GetAuthorsAsync(string? search)
        {
            var authors = await _unitOfWork.AuthorRepository.GetAllAuthorsAsync(search);
            return authors.Select(a => new AuthorDto
            {
                AuthorId = a.AuthorId,
                FullName = a.FullName,
                Biography = a.Biography
            }).ToList();
        }

        public async Task<AuthorDto?> GetAuthorByIdAsync(int authorId)
        {
            var author = await _unitOfWork.AuthorRepository.GetAuthorByIdAsync(authorId);
            if (author == null) return null;

            return new AuthorDto
            {
                AuthorId = author.AuthorId,
                FullName = author.FullName,
                Biography = author.Biography
            };
        }

        public async Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto createAuthorDto)
        {
            var author = new Author
            {
                FullName = createAuthorDto.FullName.Trim(),
                Biography = createAuthorDto.Biography?.Trim()
            };

            await _unitOfWork.AuthorRepository.AddAuthorAsync(author);
            await _unitOfWork.SaveChangesAsync();

            return new AuthorDto
            {
                AuthorId = author.AuthorId,
                FullName = author.FullName,
                Biography = author.Biography
            };
        }

        public async Task<bool> UpdateAuthorAsync(int authorId, UpdateAuthorDto updateAuthorDto)
        {
            var author = await _unitOfWork.AuthorRepository.GetAuthorByIdAsync(authorId);
            if (author == null) return false;

            author.FullName = updateAuthorDto.FullName.Trim();
            author.Biography = updateAuthorDto.Biography?.Trim();

            await _unitOfWork.AuthorRepository.UpdateAuthorAsync(author);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAuthorAsync(int authorId)
        {
            var author = await _unitOfWork.AuthorRepository.GetAuthorByIdAsync(authorId);
            if (author == null) return false;

            if (await _unitOfWork.AuthorRepository.HasBooksAsync(authorId))
            {
                throw new InvalidOperationException("Cannot delete author because they are linked to books.");
            }

            await _unitOfWork.AuthorRepository.DeleteAuthorAsync(authorId);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
