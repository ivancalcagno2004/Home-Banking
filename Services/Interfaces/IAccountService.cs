using Models;
using Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IAccountService
    {
        Task<Account> CreateAccountAsync(User user);
        Task<decimal> GetBalanceAsync(int accountId);
        Task<List<AccountDTO>> GetAllAsync();
        Task ClaimGiftAsync(int accountId, decimal amount);
        Task<List<AccountDTO>> GetAccountsByUserId(int userId);
        public Task UpdateAccount(Account account);
    }
}
