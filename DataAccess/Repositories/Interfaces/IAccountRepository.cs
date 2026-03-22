using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBanking.Data.Repositories.Interfaces
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<IEnumerable<Account>> GetAccountsByUserIdAsync(int userId);

        Task<Account?> GetAccountByCBUOrAliasAsync(string cbuOrAlias);
    }
}
