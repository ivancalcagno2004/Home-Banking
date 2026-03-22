using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ServicePayment
    {
        
        public int Id { get; set; }

        public required string ServiceName { get; set; }

        public decimal Amount { get; set; }

        public DateTime ExpDate { get; set; }

        public bool IsPaid { get; set; } = false;

        public int UserId { get; set; }

        [NotMapped]
        public DateTime PaymentDate { get; set; }

        public required User User { get; set; }

        public required int CategoryId { get; set; }

        
        public TransactionCategory? Category { get; set; }

    }
}
