using HomeBanking.Data.Context;
using Models;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ViewModels
{
    /// <summary>
    /// ViewModel de carga inicial. Realiza tareas de inicialización de datos (seed)
    /// al arranque y navega al flujo de autenticación.
    /// </summary>
    public class LoadingViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;

        public LoadingViewModel(INavigationService navigationService, AppDbContext context, ILogger<LoadingViewModel> logger)
        {
            _navigationService = navigationService;
            _context = context;
            _logger = logger;

            _ = IniciarAppAsync();
        }

        private async Task IniciarAppAsync()
        {
            try
            {
                _logger?.LogInformation("LoadingViewModel: iniciando app");
                if (!await _context.TransactionCategories.AnyAsync())
                {
                    _logger?.LogInformation("LoadingViewModel: creando categorías iniciales");
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
                    _logger?.LogInformation("LoadingViewModel: creando usuario/cuenta del sistema");
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

                _logger?.LogInformation("LoadingViewModel: navegación a SignInPage");
                await _navigationService!.NavigateToAsync("//SignInPage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error crítico al iniciar: {ex.Message}");
                _logger?.LogError(ex, "LoadingViewModel: error crítico al iniciar");
                await _navigationService!.NavigateToAsync("//SignInPage");
            }
        }
    }
}
