using Models.DTO;
using ViewModels;

namespace UI.Views.Pages;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;
    public HomePage(HomeViewModel vm)
	{
		InitializeComponent();
        _viewModel = vm;
		BindingContext = _viewModel;
    }

    private async void OnViewDetailsClicked(object? sender, EventArgs e)
    {
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
        await Task.Delay(1000);

        await _viewModel.LoadData();
        
        MiRefreshView.IsRefreshing = false;
    }

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("SettingsPageMobile");
    }

    private async void OnLogoutClicked(object? sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("🚪 [NAVEGACIÓN] Cerrando sesión desde el dropdown.");
        await Shell.Current.GoToAsync("//SignInPage");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        System.Diagnostics.Debug.WriteLine("⏳ [HOME] Cargando datos del resumen...");
        await _viewModel.LoadData();
    }
}