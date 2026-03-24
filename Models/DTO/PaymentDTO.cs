using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class PaymentDTO
    {
        public int Id { get; set; }
        public string? ServiceName { get; set; }
        public string? CategoryName { get; set; } 
        public decimal Amount { get; set; }
        public DateTime ExpDate { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}
