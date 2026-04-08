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
    /// <summary>
    /// Repositorio de transacciones. Incluye consultas por usuario y por cuenta,
    /// cargando relaciones necesarias para construir el historial.
    /// </summary>
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(DbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Transaction>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(t => t.FromAccount).ThenInclude(a => a.User)
                .Include(t => t.ToAccount).ThenInclude(a => a.User)
                .Where(t => t.FromAccount.UserId == userId || t.ToAccount.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByAccountIdAsync(int accountId)
        {
            return await _dbSet
                .Where(t => t.FromAccountId == accountId || t.ToAccountId == accountId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}
