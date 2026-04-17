using CommunityToolkit.Mvvm.ComponentModel;
using Services.Implementations;

namespace ViewModels
{
    /// <summary>
    /// ViewModel de configuración. Expone datos del usuario en sesión y opciones
    /// de preferencias locales (modo oscuro / notificaciones) para la UI.
    /// </summary>
    public partial class SettingsViewModel : BaseViewModel
    {
        private readonly UserSession _userSession;

        public string FullName => _userSession.CurrentUser?.FullName ?? "Usuario";
        public string Email => _userSession.CurrentUser?.Email ?? "usuario@email.com";
        public string Initials => !string.IsNullOrEmpty(FullName) ? FullName[0].ToString() : "U";

        [ObservableProperty]
        private bool _isDarkMode;

        [ObservableProperty]
        private bool _notificationsEnabled = true;

        public SettingsViewModel(UserSession userSession)
        {
            _userSession = userSession;
        }

        partial void OnIsDarkModeChanging(bool value)
        {
            if (_isDarkMode != value)
            {
                _isDarkMode = value;
            }
        }
    }
}