using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class TransactionCategory
    {
        public int TransactionCategoryId { get; set; }
        public required string Name { get; set; }

        public ICollection<TransactionCategoryMap>? TransactionCategoryMaps { get; set; }
    }
}
