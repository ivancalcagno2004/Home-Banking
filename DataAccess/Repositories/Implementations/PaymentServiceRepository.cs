using HomeBanking.Data.Context;
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
    /// Repositorio de pagos de servicios (<see cref="ServicePayment"/>).
    /// Permite consultar pagos pendientes o históricos por usuario e incluye la categoría asociada.
    /// </summary>
    public class PaymentServiceRepository : GenericRepository<ServicePayment>, IPaymentServiceRepository
    {
        public PaymentServiceRepository(AppDbContext context): base(context)
        {
        }

        public async Task<IEnumerable<ServicePayment>> GetPendingByUserIdAsync(int userId, bool? isPaid = null)
        {
            var query = _dbSet
                .Include(p => p.Category)
                .Where(p => p.UserId == userId);

                if (isPaid.HasValue)
                {
                    query = query.Where(p => p.IsPaid == isPaid.Value);
                }

                return await query.OrderByDescending(p => p.ExpDate).ToListAsync();
        }
    }
}
