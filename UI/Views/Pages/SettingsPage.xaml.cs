using ViewModels;

namespace UI.Views.Pages;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;

        vm.IsDarkMode = Preferences.Get("DarkMode", false);
    }

    private void OnThemeSwitchToggled(object sender, ToggledEventArgs e)
    {
        bool isDarkMode = e.Value;

        if (Application.Current != null)
        {
            Application.Current.UserAppTheme = isDarkMode ? AppTheme.Dark : AppTheme.Light;
        }

        Preferences.Set("DarkMode", isDarkMode);
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("🚪 [NAVEGACIÓN] Cerrando sesión desde el dropdown.");
        await Shell.Current.GoToAsync("//SignInPage");
    }
}