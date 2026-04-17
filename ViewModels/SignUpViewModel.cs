using Models.DTO;
using Services.Interfaces;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.Input;

namespace ViewModels
{
    /// <summary>
    /// ViewModel de registro. Recolecta datos del usuario, valida campos requeridos
    /// y crea una nueva cuenta mediante <see cref="IUserService"/>.
    /// </summary>
    public partial class SignUpViewModel : BaseViewModel
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }

        public SignUpViewModel(IUserService userService, INavigationService navigationService, IDialogService dialogService, ICredentialService credentialService, ILogger<SignUpViewModel> logger)
        {
            _userService = userService;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _credentialService = credentialService;
            _logger = logger;
        }

        [RelayCommand]
        private async Task NavigateToSignIn()
        {
            _logger?.LogInformation("SignUpViewModel: navegando a SignInPage");
            await _navigationService!.NavigateToAsync("//SignInPage");
        }


        [RelayCommand]
        private async Task Register(object parameter)
        {
            var password = parameter as string;

            if (string.IsNullOrWhiteSpace(FullName) ||
                string.IsNullOrWhiteSpace(UserName) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(password))
            {
                await _dialogService!.ShowAlertAsync("Campos Requeridos", "Por favor, completa todos los campos para registrarte.", "Ok");
                return;
            }

            UserProfileDTO? user = await _userService!.GetUserByName(UserName);

            if (user != null)
            {
                await _dialogService!.ShowAlertAsync("Usuario Existente", "El nombre de usuario ya está en uso. Por favor, inicia sesión o elige otro.", "Ok");
                return;
            }

            if (IsBusy) return;
           
            try
            {
                _logger?.LogInformation("SignUpViewModel: registrando usuario");
                IsBusy = true;
                await Task.Delay(1000);
                await _userService!.CreateAsync(FullName, UserName, Email, password, DateTime.UtcNow, false);
                await _credentialService!.ClearCredentialsAsync();

                _logger?.LogInformation("SignUpViewModel: registro OK");
                await _dialogService!.ShowAlertAsync("Registro Exitoso", "Tu cuenta ha sido creada exitosamente. Ahora puedes iniciar sesión.", "Ok");
                await _navigationService!.NavigateToAsync("//SignInPage");
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "SignUpViewModel: error en registro");
                var innerMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                await _dialogService!.ShowAlertAsync("Error de Registro", $"Ocurrió un error al crear tu cuenta: {innerMessage}", "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
