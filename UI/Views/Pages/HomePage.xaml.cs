using ViewModels;

namespace UI.Views.Pages;

public partial class HomePage : ContentPage
{
	public HomePage(HomeViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("SettingsPageMobile");
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("🚪 [NAVEGACIÓN] Cerrando sesión desde el dropdown.");
        await Shell.Current.GoToAsync("//SignInPage");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ViewModels.HomeViewModel viewModel)
        {
            await viewModel.LoadData();
        }
    }
}