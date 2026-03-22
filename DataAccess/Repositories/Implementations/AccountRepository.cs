using HomeBanking.Data.Repositories.Interfaces;
using Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBanking.Data.Repositories.Implementations
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(DbContext context) : base(context)
        {
        }

        public async Task<Account?> GetAccountByCBUOrAliasAsync(string cbuOrAlias)
        {
            return await _dbSet
                .FirstOrDefaultAsync(a => a.CBU == cbuOrAlias || a.Alias == cbuOrAlias);
        }

        public async Task<IEnumerable<Account>> GetAccountsByUserIdAsync(int userId)
        {
            return await _dbSet.Where(a => a.UserId == userId).ToListAsync();
        }
    }
}
