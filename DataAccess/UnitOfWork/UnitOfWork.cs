using HomeBanking.Data.Context;
using HomeBanking.Data.Repositories.Implementations;
using HomeBanking.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBanking.Data.UnitOfWork
{
    /// <summary>
    /// Implementación concreta de <see cref="IUnitOfWork"/>.
    /// Construye los repositorios sobre un único <see cref="AppDbContext"/> y expone
    /// <see cref="SaveChangesAsync"/> para confirmar cambios.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {

        private readonly AppDbContext _context;

        public IUserRepository Users { get; }

        public ITransactionRepository Transactions { get; }

        public IAccountRepository Accounts { get; }

        public IPaymentServiceRepository Payments { get; }

        public ITransactionCategoryMapRepository TransactionCategoryMaps { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            Users = new UserRepository(context);
            Transactions = new TransactionRepository(context);
            Accounts = new AccountRepository(context);
            Payments = new PaymentServiceRepository(context);
            TransactionCategoryMaps = new TransactionCategoryMapRepository(context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
