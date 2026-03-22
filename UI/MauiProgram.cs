using HomeBanking.Data.Context;
using HomeBanking.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Implementations;
using Services.Interfaces;
using UI.Services;
using UI.Views;
using ViewModels;

namespace UI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // 1. Configurar la Base de Datos (SQLite)
            // En MAUI, la base de datos debe guardarse en una ruta segura del teléfono o PC
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
            builder.Services.AddTransient<UserViewModel>();

            // 5. Inyección de Dependencias: Capa Views (Pantallas)
            builder.Services.AddTransient<SignInPage>(); 
            builder.Services.AddTransient<SignUpPage>();
            builder.Services.AddTransient<DashboardPage>();

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

            // Aquí puedes llamar a tu clase BankGenerator o SeedData si necesitas datos iniciales
        }
    }
}
