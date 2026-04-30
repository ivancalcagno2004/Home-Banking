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
                //.WriteTo.File(rutaLog, rollingInterval: RollingInterval.Day)
                .WriteTo.Debug()
                .CreateLogger();

            // 1. Configurar la Base de Datos

#if WINDOWS
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(DatabaseSecrets.ConnectionStringPostgreSQL));
#elif ANDROID || IOS
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "TandilBankLocal.db");

            Directory.CreateDirectory(FileSystem.AppDataDirectory);

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Filename={dbPath}"));
#endif
            //azure
            //builder.Services.AddDbContext<AppDbContext>(options =>
            //      options.UseSqlServer(DatabaseSecrets.ConnectionStringAzure));
            // 2. Inyección de Dependencias: Capa Data
            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

            // 3. Inyección de Dependencias: Capa Services
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IAccountService, AccountService>();
            builder.Services.AddTransient<ITransactionService, TransactionService>();
            builder.Services.AddTransient<IPaymentService, PaymentService>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton<IDialogService, DialogService>();
            builder.Services.AddSingleton<ICredentialService, CredentialService>();
            builder.Services.AddSingleton<INotificationService, NotificationService>();
            builder.Services.AddSingleton<IEmailService, EmailService>();
            builder.Services.AddSingleton<IGroqChatService, GroqChatService>();
            builder.Services.AddSingleton<IClipboardService, ClipboardService>();
            builder.Services.AddSingleton<UserSession>();

            // 4. Inyección de Dependencias: Capa ViewModels
            builder.Services.AddTransient<SignInViewModel>();
            builder.Services.AddTransient<SignUpViewModel>();
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
            InitializeDatabase(app);

            return app;
        }

        private static void InitializeDatabase(MauiApp app)
        {
            using var scope = app.Services.CreateScope();

            try
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

#if WINDOWS
                dbContext.Database.Migrate();
                System.Diagnostics.Debug.WriteLine("[EXITO] Migraciones de PostgreSQL aplicadas en Windows.");
#elif ANDROID || IOS
                dbContext.Database.EnsureCreated();
                System.Diagnostics.Debug.WriteLine("[EXITO] Base de datos local SQLite creada en el dispositivo móvil.");
#endif
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR FATAL DE BD]: {ex.Message}");
            }
        }
    }
}
