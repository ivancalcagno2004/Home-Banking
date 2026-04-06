using ViewModels;

namespace UI.Views.Pages;

public partial class TransactionsPage : ContentPage
{
    private readonly TransactionsViewModel _viewModel;
    public TransactionsPage(TransactionsViewModel vm)
	{
		InitializeComponent();
        _viewModel = vm;
		BindingContext = _viewModel;
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
        await _viewModel.LoadTransactions();
    }
}