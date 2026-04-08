using ViewModels;
using Microsoft.Extensions.Logging;

namespace UI.Views.Pages;

/// <summary>
/// Página de pagos. Asocia el <see cref="PaymentsViewModel"/> como BindingContext
/// y maneja navegación a configuración y cierre de sesión desde la UI.
/// </summary>
public partial class PaymentsPage : ContentPage
{
   private readonly ILogger<PaymentsPage> _logger;

    public PaymentsPage(PaymentsViewModel vm, ILogger<PaymentsPage> logger)
	{
		InitializeComponent();
        _logger = logger;
		BindingContext = vm;
    }

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        _logger.LogInformation("PaymentsPage: navegar a configuración");
        await Shell.Current.GoToAsync("SettingsPageMobile");
    }

    private async void OnLogoutClicked(object? sender, EventArgs e)
    {
        _logger.LogInformation("PaymentsPage: logout");
        await Shell.Current.GoToAsync("//SignInPage");
    }
}