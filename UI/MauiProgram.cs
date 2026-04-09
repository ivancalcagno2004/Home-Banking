using DataAccess.Context;
using HomeBanking.Data.Context;
using HomeBanking.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models;
using Plugin.LocalNotification;
using Serilog;
using Services.Implementations;
using Services.Interfaces;
using System.Reflection;
using UI.Services;
using UI.Views;
using UI.Views.Components;
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

            string rutaLog = Path.Combine(FileSystem.AppDataDirectory, "TandilBank_Log.txt");

            System.Diagnostics.Debug.WriteLine("\n=======================================================");
            System.Diagnostics.Debug.WriteLine($"ATENCIÓN: EL ARCHIVO DE LOGS SE ESTÁ GUARDANDO EN:");
            System.Diagnostics.Debug.WriteLine(rutaLog);
            System.Diagnostics.Debug.WriteLine("=======================================================\n");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(rutaLog, rollingInterval: RollingInterval.Day)
                .WriteTo.Debug()
                .CreateLogger();

            // 1. Configurar la Base de Datos
            //string dbPath = Path.Combine(FileSystem.AppDataDirectory, "HomeBanking.db");
            builder.Services.AddDbContext<AppDbContext>(options => 
                    options.UseSqlServer(DatabaseSecrets.ConnectionStringAzure));

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
            builder.Services.AddSingleton<IEmailService, EmailService>();
            builder.Services.AddSingleton<IGroqChatService, GroqChatService>();
            builder.Services.AddSingleton<UserSession>();

            // 4. Inyección de Dependencias: Capa ViewModels
            builder.Services.AddTransient<SignInViewModel>();
            builder.Services.AddTransient<SignUpViewModel>();
            builder.Services.AddTransient<AccountViewModel>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<PaymentsViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<TransactionsViewModel>();
            builder.Services.AddTransient<TransferViewModel>();
            builder.Services.AddTransient<LoadingViewModel>();
            builder.Services.AddSingleton<ChatViewModel>();

            // 5. Inyección de Dependencias: Capa Views (Pantallas)
            builder.Services.AddTransient<SignInPage>(); 
            builder.Services.AddTransient<SignUpPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<PaymentsPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<TransferPage>();
            builder.Services.AddTransient<TransactionsPage>();
            builder.Services.AddTransient<LoadingPage>();
            builder.Services.AddSingleton<FloatingChatView>();

            builder.Services.AddSerilog();

            #if DEBUG
                builder.Logging.AddDebug();
            #endif

            var app = builder.Build();

            // Inicializar la Base de datos al arrancar
            //InitializeDatabase(app);

            return app;
        }
    }
}
