using BCrypt.Net;
using HomeBanking.Data.Helpers;
using HomeBanking.Data.UnitOfWork;
using Models;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class AccountService : BaseService, IAccountService
    {
        public AccountService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<Account> CreateAccountAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentException("User not found", nameof(user));
            }

            Account account = new Account
            {
                UserId = user.UserId,
                User = user,
                CBU = BankGenerator.GenerateCBU(),
                Alias = BankGenerator.GenerateAlias(),
                Balance = 0,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Accounts.AddAsync(account);
            await _unitOfWork.SaveChangesAsync();

            return account;
        }

        public async Task<decimal> GetBalanceAsync(int accountId)
        {
            var account = await _unitOfWork.Accounts.GetByIdAsync(accountId);
            return account?.Balance ?? 0;
        }

        public async Task<List<Account>> GetAllAsync()
        {
            return (List<Account>) await _unitOfWork.Accounts.GetAllAsync();
        }

        public async Task<List<Account>> GetAccountsByUserId(int userId)
        {
            return (List<Account>) await _unitOfWork.Accounts.GetAccountsByUserIdAsync(userId);
        }

        public async Task UpdateAccount(Account account)
        {
            _unitOfWork.Accounts.Update(account);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
