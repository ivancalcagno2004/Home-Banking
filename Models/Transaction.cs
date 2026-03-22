using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }

        public int FromAccountId { get; set; }

        public required Account FromAccount { get; set; }

        public int ToAccountId { get; set; }

        public required Account ToAccount { get; set; }

        public decimal Amount { get; set; }

        public required string Status { get; set; }

        public required string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<TransactionCategoryMap> TransactionCategoryMaps { get; private set; }

        public Transaction()
        {
            TransactionCategoryMaps = new List<TransactionCategoryMap>();
        }
    }
}
