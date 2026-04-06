using HomeBanking.Data.Context;
using Models;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ViewModels
{
    public class LoadingViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;

        public LoadingViewModel(INavigationService navigationService, AppDbContext context)
        {
            _navigationService = navigationService;
            _context = context;

            _ = IniciarAppAsync();
        }

        private async Task IniciarAppAsync()
        {
            try
            {
                if (!await _context.TransactionCategories.AnyAsync())
                {
                    _context.TransactionCategories.AddRange(
                        new TransactionCategory { Name = "Impuestos Municipales" },
                        new TransactionCategory { Name = "Telefonía" },
                        new TransactionCategory { Name = "Servicios Públicos" },
                        new TransactionCategory { Name = "Automotor" }
                    );
                    await _context.SaveChangesAsync();
                }

                if (!await _context.Users.AnyAsync(u => u.UserName == "Admin"))
                {
                    User sistema = new User
                    {
                        UserName = "Admin",
                        Email = "admin@tandilbank.com",
                        Password = "admin",
                        FullName = "Sistema",
                        UserId = 0
                    };

                    _context.Users.Add(sistema);

                    _context.Accounts.Add(new Account
                    {
                        Alias = "PAGOS.SERVICIOS",
                        CBU = "0000000000000000000123",
                        User = sistema,
                        Balance = 9999999
                    });

                    await _context.SaveChangesAsync();
                }

                await _navigationService!.NavigateToAsync("//SignInPage");
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error crítico al iniciar: {ex.Message}");
                await _navigationService!.NavigateToAsync("//SignInPage");
            }
        }
    }
}
