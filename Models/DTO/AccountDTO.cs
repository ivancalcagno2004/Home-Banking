using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class AccountDTO
    {
        public int Id { get; set; }
        public string? Alias { get; set; }
        public string? CBU { get; set; }
        public decimal Balance { get; set; }

        public string PickerDisplayName => $"{Alias} - {Balance:C2}";
    }
}
