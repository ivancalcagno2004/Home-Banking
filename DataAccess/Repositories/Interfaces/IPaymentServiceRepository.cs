using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBanking.Data.Repositories.Interfaces
{
    public interface IPaymentServiceRepository : IGenericRepository<ServicePayment>
    {
        Task<IEnumerable<ServicePayment>> GetPendingByUserIdAsync(int userId, bool? isPaid = null);
    }
}
