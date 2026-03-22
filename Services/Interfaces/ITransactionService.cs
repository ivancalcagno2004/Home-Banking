using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ITransactionService 
    {
        //Task TransferAsync(int fromAccountId, int toAccountId, decimal amount);

        Task TransferToAsync(int fromAccountId, string toCBUOrAlias, decimal amount);

        Task<IEnumerable<Transaction>> GetTransactionsByUserIdAsync(int userId);
    }
}
