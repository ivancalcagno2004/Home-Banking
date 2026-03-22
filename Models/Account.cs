using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public int UserId { get; set; }

        public required string CBU { get; set; }
        public required string Alias { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }

        public required User User { get; set; }

        public ICollection<Transaction> TransactionsFrom { get; private set; }
        public ICollection<Transaction> TransactionsTo { get; private set; }

        public Account()
        {
            TransactionsFrom = new List<Transaction>();
            TransactionsTo = new List<Transaction>();
        }
    }
}
