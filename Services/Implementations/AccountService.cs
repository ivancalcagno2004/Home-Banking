using BCrypt.Net;
using HomeBanking.Data.Helpers;
using HomeBanking.Data.UnitOfWork;
using Models;
using Models.DTO;
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
                Alias = await BankGenerator.GenerateUniqueAliasAsync(_unitOfWork),
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

        public async Task<List<AccountDTO>> GetAllAsync()
        {
             var accounts = await _unitOfWork.Accounts.GetAllAsync();

            var dtoList = accounts.Select(a => new AccountDTO
            {
                Id = a.AccountId,
                Alias = a.Alias,
                CBU = a.CBU,
                Balance = a.Balance,
            }).ToList();

            return dtoList;
        }

        public async Task<List<AccountDTO>> GetAccountsByUserId(int userId)
        {
            var accounts = await _unitOfWork.Accounts.GetAccountsByUserIdAsync(userId);

            var dtoList = accounts.Select(a => new AccountDTO
            {
                Id = a.AccountId,
                Alias = a.Alias,
                CBU = a.CBU,
                Balance = a.Balance,
            }).ToList();

            return dtoList;
        }

        public async Task UpdateAccount(Account account)
        {
            _unitOfWork.Accounts.Update(account);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ClaimGiftAsync(int accountId, decimal amount)
        {
            var account = await _unitOfWork.Accounts.GetByIdAsync(accountId);

            if (account == null)
            {
                throw new Exception("No se encontró la cuenta para depositar el regalo.");
            }

            account.Balance += amount;

            _unitOfWork.Accounts.Update(account);

            Account? sistema = await _unitOfWork.Accounts.GetAccountByCBUOrAliasAsync("PAGOS.SERVICIOS");
            var giftTransaction = new Transaction
            {
                FromAccount = sistema!,
                ToAccountId = account.AccountId,
                ToAccount = account,
                Amount = amount,
                Description = "Regalo de Bienvenida - Tandil Bank",
                CreatedAt = DateTime.UtcNow,
                Status = "Completed"
            };
            await _unitOfWork.Transactions.AddAsync(giftTransaction);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
