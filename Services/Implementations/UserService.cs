using HomeBanking.Data.Helpers;
using HomeBanking.Data.UnitOfWork;
using Models;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DTO;

namespace Services.Implementations
{
    public class UserService : BaseService, IUserService
    {

        private readonly IAccountService _accountService;

        public UserService(IUnitOfWork unitOfWork, IAccountService accountService) : base(unitOfWork)
        {
            _accountService = accountService;
        }

        public async Task<User> CreateAsync(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password); // hashing

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            await _accountService.CreateAccountAsync(user);

            var random = new Random();

            // Creamos una lista de servicios típicos
            var serviciosIniciales = new List<ServicePayment>
            {
                new ServicePayment
                {
                    UserId = user.UserId, 
                    User = user,
                    CategoryId = 3,
                    ServiceName = "USINA TANDIL",
                    Amount = random.Next(2500, 8000), 
                    IsPaid = false,
                    ExpDate = DateTime.Now.AddDays(10)
                },
                new ServicePayment
                {
                    UserId = user.UserId,
                    User = user,
                    CategoryId = 3,
                    ServiceName = "CAMUZZI GAS",
                    Amount = random.Next(2500, 8000),
                    IsPaid = false,
                    ExpDate = DateTime.Now.AddDays(-2)
                },
                new ServicePayment
                {
                    UserId = user.UserId,
                    User = user,
                    CategoryId = 4,
                    ServiceName = "ARBA AUTOMOTOR",
                    Amount = random.Next(18000, 38000),
                    IsPaid = false,
                    ExpDate = DateTime.Now.AddDays(10)
                },
                new ServicePayment
                {
                    UserId = user.UserId,
                    User = user,
                    CategoryId = 2,
                    ServiceName = "PERSONAL FLOW",
                    Amount = random.Next(2500, 8000),
                    IsPaid = false,
                    ExpDate = DateTime.Now.AddDays(15)
                },
                new ServicePayment
                {
                    UserId = user.UserId,
                    User = user,
                    CategoryId = 1,
                    ServiceName = "TASAS MUNICIPALES",
                    Amount = random.Next(2500, 8000),
                    IsPaid = false,
                    ExpDate = DateTime.Now.AddDays(1)
                },
            };

            foreach (var service in serviciosIniciales)
            {
                await _unitOfWork.Payments.AddAsync(service);
            }
            await _unitOfWork.SaveChangesAsync();

            return user;
        }

        public async Task UpdateUser(User user)
        {
            _unitOfWork.Users.Update(user); 
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            var user = await _unitOfWork.Users.GetByUsernameAsync(username);
            if (user == null)
            {
                return null;
            }
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password); // verificacion

            return isPasswordValid ? user : null;
        }
    }
}
