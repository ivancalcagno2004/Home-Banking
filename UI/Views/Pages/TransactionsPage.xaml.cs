using ViewModels;
using Microsoft.Extensions.Logging;

namespace UI.Views.Pages;

/// <summary>
/// Página de transacciones. Enlaza <see cref="TransactionsViewModel"/>, carga los
/// movimientos al mostrarse y provee accesos a configuración y cierre de sesión.
/// </summary>
public partial class TransactionsPage : ContentPage
{
    private readonly TransactionsViewModel _viewModel;

    private readonly ILogger<TransactionsPage> _logger;

    public TransactionsPage(TransactionsViewModel vm, ILogger<TransactionsPage> logger)
	{
		InitializeComponent();
        _viewModel = vm;
        _logger = logger;
		BindingContext = _viewModel;
    }

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        _logger.LogInformation("TransactionsPage: navegar a configuración");
        await Shell.Current.GoToAsync("SettingsPageMobile");
    }

    private async void OnLogoutClicked(object? sender, EventArgs e)
    {
        _logger.LogInformation("TransactionsPage: logout");
        await Shell.Current.GoToAsync("//SignInPage");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _logger.LogInformation("TransactionsPage: appearing");
        await _viewModel.LoadTransactions();
    }
}