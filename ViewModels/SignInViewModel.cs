using Services.Implementations;
using Services.Interfaces;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ViewModels
{
    /// <summary>
    /// ViewModel de inicio de sesión. Valida credenciales, maneja sesión de usuario,
    /// navega a la pantalla principal y soporta autenticación biométrica si está disponible.
    /// </summary>
    public partial class SignInViewModel : BaseViewModel
    {
        private readonly UserSession _userSession;
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private string? _userName;

        [ObservableProperty]
        private string? _welcomeMessage;

        [ObservableProperty]
        private bool _showWelcomeMessage;

        [ObservableProperty]
        private bool _showInputUser;

        public SignInViewModel(IUserService userService, UserSession userSession, IDialogService dialogService, INavigationService navigationService, ICredentialService credentialService, INotificationService notificationService, IPaymentService paymentService, ILogger<SignInViewModel> logger)
        {
            _userService = userService;
            _userSession = userSession;
            _dialogService = dialogService;
            _navigationService = navigationService;
            _credentialService = credentialService;
            _notificationService = notificationService;
            _paymentService = paymentService;
            _logger = logger;

            _showInputUser = true;
            _showWelcomeMessage = false;
        }

        public async Task CheckSavedUserAsync()
        {
            var credentials = await _credentialService!.GetCredentialsAsync();

            if (!string.IsNullOrEmpty(credentials.username))
            {
                _logger?.LogInformation("SignInViewModel: usuario guardado detectado");
                UserName = credentials.username;
                WelcomeMessage = $"¡Hola de nuevo, {UserName}! 👋";

                ShowInputUser = false;
                ShowWelcomeMessage = true;
            }
        }

        [RelayCommand]
        private void ChangeUser()
        {
            UserName = string.Empty;
            ShowWelcomeMessage = false;
            ShowInputUser = true;
        }

        [RelayCommand]
        private async Task BiometricLogin()
        {
            var credentials = await _credentialService!.GetCredentialsAsync();

            if (string.IsNullOrEmpty(credentials.username) || string.IsNullOrEmpty(credentials.password))
            {
                await _dialogService!.ShowAlertAsync("Atención", "Primero debes iniciar sesión manualmente con tu contraseña una vez.", "OK");
                return;
            }

            bool isAvailable = await CrossFingerprint.Current.IsAvailableAsync(true);
            if (!isAvailable)
            {
                await _dialogService!.ShowAlertAsync("Error", "Tu dispositivo no soporta biometría o no la tienes configurada.", "OK");
                return;
            }

            var request = new AuthenticationRequestConfiguration("Iniciar Sesión", "Confirma tu identidad para entrar a Tandil Bank");

            var result = await CrossFingerprint.Current.AuthenticateAsync(request);

            if (result.Authenticated)
            {
                if (IsBusy) return;

                try
                {
                    IsBusy = true;
                    _logger?.LogInformation("SignInViewModel: login biométrico autenticado");
                    await Task.Delay(1000);
                    var user = await _userService!.ValidateUserAsync(credentials.username, credentials.password);
                    if (user != null)
                    {
                        _userSession.CurrentUser = user;
                        UserName = credentials.username;
                        await _navigationService!.NavigateToAsync("//HomePage");
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "SignInViewModel: error en login biométrico");
                    await _dialogService!.ShowAlertAsync("Error", $"Ocurrió un problema al leer tus credenciales: {ex.Message}", "OK");
                }
                finally {IsBusy = false;}
            }
            else
            {
                await _dialogService!.ShowAlertAsync("Autenticación fallida", "No se pudo verificar tu identidad.", "OK");
            }
        }

        [RelayCommand]
        private async Task SignIn(object parameter)
        {
            var password = parameter as string;

            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(password))
            {
                _logger?.LogInformation("SignInViewModel: campos vacíos");
                await _dialogService!.ShowAlertAsync(
                    "Campos vacíos",
                    "Por favor, ingresa tu nombre de usuario y contraseña.",
                    "OK");
                return;
            }
            if(IsBusy) return;
            try
            {
                IsBusy= true;
                _logger?.LogInformation("SignInViewModel: intentando iniciar sesión");
                var user = await _userService!.ValidateUserAsync(UserName, password);
                if (user != null)
                {
                    _logger?.LogInformation("SignInViewModel: login OK (UserId={UserId})", user.UserId);
                    _userSession.CurrentUser = user;

                    await _credentialService!.SaveCredentialsAsync(UserName, password);
                    
                    await LoadNotifications();

                    await _navigationService!.NavigateToAsync("//HomePage");
                }
                else
                {
                    _logger?.LogWarning("SignInViewModel: login fallido");
                    await _dialogService!.ShowAlertAsync(
                        "Error de autenticación",
                        "Nombre de usuario o contraseña incorrectos. Inténtalo de nuevo.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "SignInViewModel: error en inicio de sesión");
                await _dialogService!.ShowAlertAsync(
                    "Error",
                    $"Ocurrió un error al intentar iniciar sesión: {ex.Message}",
                    "OK");
            }
            finally { IsBusy = false; }
        }

        private async Task LoadNotifications()
        {
            var pagos = await _paymentService!.GetPendingPaymentsAsync(_userSession.CurrentUser!.UserId, false);

            await _notificationService.CheckAndNotifyServicesAsync(pagos);
        }

        [RelayCommand]
        private async Task NavigateToSignUp()
        {
            _logger?.LogInformation("SignInViewModel: navegando a SignUpPage");
            await _navigationService!.NavigateToAsync("//SignUpPage");
        }
    }
}
