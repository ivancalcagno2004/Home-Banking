using Models;
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
        Task<List<Account>> GetAllAsync();
        Task<List<Account>> GetAccountsByUserId(int userId);
        public Task UpdateAccount(Account account);
    }
}
