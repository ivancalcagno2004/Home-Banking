using ViewModels;
using Microsoft.Extensions.Logging;

namespace UI.Views.Pages;

/// <summary>
/// Página de transferencias. Asocia el <see cref="TransferViewModel"/> como BindingContext
/// y maneja navegación a configuración y cierre de sesión desde la UI.
/// </summary>
public partial class TransferPage : ContentPage
{
   private readonly ILogger<TransferPage> _logger;

    public TransferPage(TransferViewModel vm, ILogger<TransferPage> logger)
	{
		InitializeComponent();
        _logger = logger;
		BindingContext = vm;
    }

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        _logger.LogInformation("TransferPage: navegar a configuración");
        await Shell.Current.GoToAsync("SettingsPageMobile");
    }

    private async void OnLogoutClicked(object? sender, EventArgs e)
    {
        _logger.LogInformation("TransferPage: logout");
        await Shell.Current.GoToAsync("//SignInPage");
    }
}