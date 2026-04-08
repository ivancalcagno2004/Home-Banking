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
    /// Repositorio para el mapeo entre transacciones y categorías.
    /// </summary>
    public class TransactionCategoryMapRepository : GenericRepository<TransactionCategoryMap>, ITransactionCategoryMapRepository
    {
        public TransactionCategoryMapRepository(DbContext context) : base(context)
        {
        }
    }
}
