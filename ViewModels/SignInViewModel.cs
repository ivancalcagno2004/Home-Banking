using Services.Implementations;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using Microsoft.Extensions.Logging;

namespace ViewModels
{
    /// <summary>
    /// ViewModel de inicio de sesión. Valida credenciales, maneja sesión de usuario,
    /// navega a la pantalla principal y soporta autenticación biométrica si está disponible.
    /// </summary>
    public class SignInViewModel : BaseViewModel
    {
        private readonly UserSession _userSession;
        private readonly INotificationService _notificationService;

        private string? _userName;
        public string? UserName
        {
            get => _userName;
            set { _userName = value; OnPropertyChanged(); }
        }

        private string? _welcomeMessage;
        public string? WelcomeMessage
        {
            get => _welcomeMessage;
            set { _welcomeMessage = value; OnPropertyChanged(); }
        }

        private bool _showWelcomeMessage;
        public bool ShowWelcomeMessage
        {
            get => _showWelcomeMessage;
            set { _showWelcomeMessage = value; OnPropertyChanged(); }
        }

        private bool _showInputUser;
        public bool ShowInputUser
        {
            get => _showInputUser;
            set { _showInputUser = value; OnPropertyChanged(); }
        }

        public ICommand SignInCommand { get; }
        public ICommand NavigateToSignUpCommand { get; }
        public ICommand BiometricLoginCommand { get; }

        public ICommand ChangeUserCommand { get; }

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

            ShowInputUser = true;
            ShowWelcomeMessage = false;

            SignInCommand = new RelayCommand(SignIn);
            NavigateToSignUpCommand = new RelayCommand(NavigateToSignUp);
            BiometricLoginCommand = new RelayCommand(ExecuteBiometricLogin);
            ChangeUserCommand = new RelayCommand(ChangeUser);

            _ = CheckSavedUserAsync();
        }

        private async Task CheckSavedUserAsync()
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

        private void ChangeUser(object obj)
        {
            UserName = string.Empty;
            ShowWelcomeMessage = false;
            ShowInputUser = true;
        }

        private async void ExecuteBiometricLogin(object obj)
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

        private async void SignIn(object parameter)
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

        private async void NavigateToSignUp(object parameter)
        {
            _logger?.LogInformation("SignInViewModel: navegando a SignUpPage");
            await _navigationService!.NavigateToAsync("//SignUpPage");
        }
    }
}
