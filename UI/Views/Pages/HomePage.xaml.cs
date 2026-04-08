using Models.DTO;
using ViewModels;
using Microsoft.Extensions.Logging;

namespace UI.Views.Pages;

/// <summary>
/// Página principal. Inicializa el <see cref="HomeViewModel"/> como BindingContext
/// y gestiona acciones de UI (ver detalles de cuenta, refrescar, navegación a
/// configuración y cierre de sesión).
/// </summary>
public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;

    private readonly ILogger<HomePage> _logger;

    public HomePage(HomeViewModel vm, ILogger<HomePage> logger)
	{
		InitializeComponent();
        _viewModel = vm;
        _logger = logger;
		BindingContext = _viewModel;
    }

    private async void OnViewDetailsClicked(object? sender, EventArgs e)
    {
        _logger.LogInformation("HomePage: ver detalles de cuenta");
        AccountDTO? cuenta = _viewModel.GetAccountData().Result;

        if (cuenta == null) return;

        string accion = await DisplayActionSheetAsync(
            "Opciones de Cuenta",
            "Cancelar",
            null,
            "Ver todos los datos",
            "Copiar Alias",
            "Copiar CBU");

        if (accion == "Ver todos los datos")
        {
            string mensaje = $"Saldo actual: ${cuenta.Balance:N2}\n" +
                             $"Alias: {cuenta.Alias}\n" +
                             $"CBU: {cuenta.CBU}";

            await DisplayAlertAsync("Detalles", mensaje, "Entendido");
        }
        else if (accion == "Copiar CBU")
        {
            await Clipboard.Default.SetTextAsync(cuenta.CBU);
            await DisplayAlertAsync("¡Listo!", "El CBU fue copiado al portapapeles.", "OK");
        }
        else if (accion == "Copiar Alias")
        {
            await Clipboard.Default.SetTextAsync(cuenta.Alias);
            await DisplayAlertAsync("¡Listo!", "El Alias fue copiado al portapapeles.", "OK");
        }
    }

    private async void OnRefreshViewRefreshing(object? sender, EventArgs e)
    {
        _logger.LogInformation("HomePage: refrescando datos");
        await Task.Delay(1000);

        await _viewModel.LoadData();
        
        MiRefreshView.IsRefreshing = false;
    }

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        _logger.LogInformation("HomePage: navegar a configuración");
        await Shell.Current.GoToAsync("SettingsPageMobile");
    }

    private async void OnLogoutClicked(object? sender, EventArgs e)
    {
        _logger.LogInformation("HomePage: logout");
        await Shell.Current.GoToAsync("//SignInPage");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _logger.LogInformation("HomePage: appearing");
        await _viewModel.LoadData();
    }
}