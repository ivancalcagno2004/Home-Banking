using HomeBanking.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBanking.Data.UnitOfWork
{
    /// <summary>
    /// Patrón Unit of Work. Agrupa repositorios y coordina la persistencia mediante
    /// una operación de guardado unificada.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        ITransactionRepository Transactions { get; }
        IAccountRepository Accounts { get; }

        IPaymentServiceRepository Payments { get; }

        ITransactionCategoryMapRepository TransactionCategoryMaps { get; }

        Task SaveChangesAsync();
    }
}
