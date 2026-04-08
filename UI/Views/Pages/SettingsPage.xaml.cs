using ViewModels;
using Microsoft.Extensions.Logging;

namespace UI.Views.Pages;

/// <summary>
/// Página de configuración. Enlaza <see cref="SettingsViewModel"/>, inicializa el
/// estado visual (tema/versión) y persiste preferencias locales del usuario.
/// </summary>
public partial class SettingsPage : ContentPage
{
   private readonly ILogger<SettingsPage> _logger;

    public SettingsPage(SettingsViewModel vm, ILogger<SettingsPage> logger)
	{
		InitializeComponent();
        _logger = logger;
		BindingContext = vm;

        vm.IsDarkMode = Preferences.Get("DarkMode", true);
        VersionLabel.Text = $"v{AppInfo.Current.VersionString} - Tandil Bank";
       _logger.LogInformation("SettingsPage: inicializada");
    }

    private void OnThemeSwitchToggled(object? sender, ToggledEventArgs e)
    {
        bool isDarkMode = e.Value;

        _logger.LogInformation("SettingsPage: cambio de tema (dark={IsDark})", isDarkMode);

        if (Application.Current != null)
        {
            Application.Current.UserAppTheme = isDarkMode ? AppTheme.Dark : AppTheme.Light;
        }

        Preferences.Set("DarkMode", isDarkMode);
    }

    private async void OnLogoutClicked(object? sender, EventArgs e)
    {
        _logger.LogInformation("SettingsPage: logout");
        await Shell.Current.GoToAsync("//SignInPage");
    }
}