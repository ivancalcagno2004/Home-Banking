using Services.Implementations;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ViewModels
{
    public class SignInViewModel : BaseViewModel
    {
        private readonly UserSession _userSession;

        public string? UserName { get; set; }

        public ICommand SignInCommand { get; }
        public ICommand NavigateToSignUpCommand { get; }

        public SignInViewModel(IUserService userService, UserSession userSession, IDialogService dialogService, INavigationService navigationService)
        {
            _userService = userService;
            _userSession = userSession;
            _dialogService = dialogService;
            _navigationService = navigationService;

            SignInCommand = new RelayCommand(SignIn);
            NavigateToSignUpCommand = new RelayCommand(NavigateToSignUp);
        }

        private async void SignIn(object parameter)
        {
            var password = parameter as string;

            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(password))
            {
                await _dialogService!.ShowAlertAsync(
                    "Campos vacíos",
                    "Por favor, ingresa tu nombre de usuario y contraseña.",
                    "OK");
                return;
            }

            try
            {
                var user = await _userService!.ValidateUserAsync(UserName, password);
                if (user != null)
                {
                    _userSession.CurrentUser = user;

                    await _dialogService!.ShowAlertAsync(
                        "Bienvenido",
                        $"¡Hola {user.FullName}! Has iniciado sesión exitosamente.",
                        "OK");

                    await _navigationService!.NavigateToAsync("//DashboardPage");
                }
                else
                {
                    await _dialogService!.ShowAlertAsync(
                        "Error de autenticación",
                        "Nombre de usuario o contraseña incorrectos. Inténtalo de nuevo.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                await _dialogService!.ShowAlertAsync(
                    "Error",
                    $"Ocurrió un error al intentar iniciar sesión: {ex.Message}",
                    "OK");
            }
        }

        private async void NavigateToSignUp(object parameter)
        {
            await _navigationService!.NavigateToAsync("//SignUpPage");
        }
    }
}
