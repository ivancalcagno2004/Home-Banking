using Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DTO;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task CreateAsync(string fullname, string username, string email, string password, DateTime date, Boolean isGiftClaimed);

        Task<User?> ValidateUserAsync(string username, string password);

        public Task UpdateUser(User user);

        Task<UserProfileDTO?> GetUserByName(string username);
    }
}
