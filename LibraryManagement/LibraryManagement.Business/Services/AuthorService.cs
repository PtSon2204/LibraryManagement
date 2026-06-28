using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.AuthorDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.UnitOfWorks;
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
    }
}
