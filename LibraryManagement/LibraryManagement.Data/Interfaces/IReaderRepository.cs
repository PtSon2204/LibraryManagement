using System;
using System.Threading.Tasks;
using LibraryManagement.Models.Models;

namespace LibraryManagement.Data.Interfaces
{
    public interface IReaderRepository
    {
        Task<Reader?> GetReaderByEmailAsync(string email);
        Task<Reader?> GetReaderByIdAsync(Guid readerId);
        Task AddReaderAsync(Reader reader);
        void UpdateReader(Reader reader);
    }
}
