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
    /// <summary>
    /// Servicio de usuarios. Registra nuevos usuarios, valida credenciales, actualiza
    /// perfil y resuelve usuarios a partir de CBU/Alias. En el alta también crea una cuenta
    /// inicial y genera servicios a pagar por defecto.
    /// </summary>
    public class UserService : BaseService, IUserService
    {

        private readonly IAccountService _accountService;

        public UserService(IUnitOfWork unitOfWork, IAccountService accountService) : base(unitOfWork)
        {
            _accountService = accountService;
        }

        public async Task CreateAsync(string fullname, string username, string email, string password, DateTime date, Boolean isGiftClaimed)
        {
            var newUser = new User
            {
                FullName = fullname,
                UserName = username,
                Email = email,
                Password = password,
                CreatedAt = DateTime.UtcNow,
                IsGiftClaimed = false
            };

            newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password); // hashing

            await _unitOfWork.Users.AddAsync(newUser);
            await _unitOfWork.SaveChangesAsync();

            await _accountService.CreateAccountAsync(newUser);

            var random = new Random();

            // Creamos una lista de servicios típicos
            var serviciosIniciales = new List<ServicePayment>
            {
                new ServicePayment
                {
                    UserId = newUser.UserId, 
                    User = newUser,
                    CategoryId = 3,
                    ServiceName = "USINA TANDIL",
                    Amount = random.Next(2500, 8000), 
                    IsPaid = false,
                    ExpDate = DateTime.Now.AddDays(10)
                },
                new ServicePayment
                {
                    UserId = newUser.UserId,
                    User = newUser,
                    CategoryId = 3,
                    ServiceName = "CAMUZZI GAS",
                    Amount = random.Next(2500, 8000),
                    IsPaid = false,
                    ExpDate = DateTime.Now.AddDays(-2)
                },
                new ServicePayment
                {
                    UserId = newUser.UserId,
                    User = newUser,
                    CategoryId = 4,
                    ServiceName = "ARBA AUTOMOTOR",
                    Amount = random.Next(18000, 38000),
                    IsPaid = false,
                    ExpDate = DateTime.Now.AddDays(10)
                },
                new ServicePayment
                {
                    UserId = newUser.UserId,
                    User = newUser,
                    CategoryId = 2,
                    ServiceName = "PERSONAL FLOW",
                    Amount = random.Next(2500, 8000),
                    IsPaid = false,
                    ExpDate = DateTime.Now.AddDays(15)
                },
                new ServicePayment
                {
                    UserId = newUser.UserId,
                    User = newUser,
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
        }

        public async Task UpdateUser(User user)
        {
            _unitOfWork.Users.Update(user); 
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<UserProfileDTO?> GetUserByName(string username) { 
        
            User? user = await _unitOfWork.Users.GetByUsernameAsync(username);

            if (user == null) return null;

            return new UserProfileDTO
            {
                FullName = user.FullName,
                Email = user.Email,
                UserName = user.UserName
            };
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

        public async Task<UserProfileDTO?> GetUserByCBUoAlias(string CBUoAlias) {
            Account? account = await _unitOfWork.Accounts.GetAccountByCBUOrAliasAsync(CBUoAlias);

            if (account == null) return null;

            var user = await _unitOfWork.Users.GetByIdAsync(account.UserId);

            if (user == null) return null;

            return new UserProfileDTO
            {
                FullName = user.FullName,
                Email = user.Email,
                UserName = user.UserName
            };
        }
    }
}
