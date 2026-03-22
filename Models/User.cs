using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class User
    {
        public int UserId { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public required string FullName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsGiftClaimed { get; set; }

        public ICollection<Account> Accounts { get; private set; }

        public User()
        {
            Accounts = new List<Account>();
        }
    }
}
