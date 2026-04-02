using HomeBanking.Data.Context;
using HomeBanking.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models;
using Plugin.LocalNotification;
using Services.Implementations;
using Services.Interfaces;
using System.Reflection;
using UI.Services;
using UI.Views;
using UI.Views.Pages;
using ViewModels;
using INotificationService = Services.Interfaces.INotificationService;

namespace UI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseLocalNotification()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("materialdesignicons-webfont.ttf", "MaterialDesign");
                });

            // 1. Configurar la Base de Datos
            // lee json
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "HomeBanking.db");
            
                builder.Services.AddDbContext<AppDbContext>(options => 
                    options.UseSqlite($"Data Source={dbPath}"));

            // 2. Inyección de Dependencias: Capa Data
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // 3. Inyección de Dependencias: Capa Services
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<ITransactionService, TransactionService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton<IDialogService, DialogService>();
            builder.Services.AddSingleton<ICredentialService, CredentialService>();
            builder.Services.AddSingleton<INotificationService, NotificationService>();
            builder.Services.AddSingleton<UserSession>();

            // 4. Inyección de Dependencias: Capa ViewModels
            builder.Services.AddTransient<SignInViewModel>();
            builder.Services.AddTransient<SignUpViewModel>();
            builder.Services.AddTransient<AccountViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<PaymentsViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<TransactionsViewModel>();
            builder.Services.AddTransient<TransferViewModel>();

            // 5. Inyección de Dependencias: Capa Views (Pantallas)
            builder.Services.AddTransient<SignInPage>(); 
            builder.Services.AddTransient<SignUpPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<PaymentsPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<TransferPage>();
            builder.Services.AddTransient<TransactionsPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            // Inicializar la Base de datos al arrancar
            InitializeDatabase(app);

            return app;
        }

        private static void InitializeDatabase(MauiApp app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Esto crea el archivo HomeBanking.db en el celular/PC si no existe
            context.Database.EnsureCreated();

            if (!context.TransactionCategories.Any()) {
                context.TransactionCategories.AddRange(
                    new TransactionCategory { Name = "Impuestos Municipales" },
                    new TransactionCategory { Name = "Telefonía" },
                    new TransactionCategory { Name = "Servicios Públicos" },
                    new TransactionCategory { Name = "Automotor" }
                    );
                context.SaveChanges();
            }

            User sistema = new User { UserName = "Admin", Email = "", Password = "admin", FullName = "Sistema", UserId = 0 };
            context.Users.Add(sistema);

            context.Accounts.Add(new Account { Alias = "PAGOS.SERVICIOS", CBU = "123", User = sistema, Balance = 9999999});

            context.SaveChanges();
        }
    }
}
