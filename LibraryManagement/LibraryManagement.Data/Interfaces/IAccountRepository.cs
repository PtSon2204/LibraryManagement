using System;
using System.Threading.Tasks;
using LibraryManagement.Models.Models;

namespace LibraryManagement.Data.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetAccountByEmailAsync(string email);
        Task<Account?> GetAccountByIdAsync(Guid accountId);
        Task AddAccountAsync(Account account);
        void UpdateAccount(Account account);
    }
}
