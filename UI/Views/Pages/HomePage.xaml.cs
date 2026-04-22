using ViewModels;
using Microsoft.Extensions.Logging;

namespace UI.Views.Pages;

/// <summary>
/// Página principal. Inicializa el <see cref="HomeViewModel"/> como BindingContext
/// y gestiona acciones de UI (refrescar, navegación a configuración y cierre de sesión).
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

        _logger.LogInformation("HomePage: cargando datos");
        await _viewModel.LoadData();

    }
}