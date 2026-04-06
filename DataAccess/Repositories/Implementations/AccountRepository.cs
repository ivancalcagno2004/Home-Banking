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
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.CBU.ToLower() == cbuOrAlias.ToLower() || a.Alias.ToLower() == cbuOrAlias.ToLower());
        }

        public async Task<IEnumerable<Account>> GetAccountsByUserIdAsync(int userId)
        {
            return await _dbSet.AsNoTracking().Where(a => a.UserId == userId).ToListAsync();
        }
    }
}
