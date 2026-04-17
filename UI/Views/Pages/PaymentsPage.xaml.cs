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
    private readonly PaymentsViewModel _viewModel;

    public PaymentsPage(PaymentsViewModel vm, ILogger<PaymentsPage> logger)
	{
		InitializeComponent();
        _viewModel = vm;
        _logger = logger;
		BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _logger.LogInformation("PaymentsPage: cargando datos");
        await _viewModel.LoadData();
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