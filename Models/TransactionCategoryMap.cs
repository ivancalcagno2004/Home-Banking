using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class TransactionCategoryMap
    {
        public int TransactionId { get; set; }
        public int TransactionCategoryId { get; set; }
        public required Transaction Transaction { get; set; }
        public required TransactionCategory TransactionCategory { get; set; }
    }
}
