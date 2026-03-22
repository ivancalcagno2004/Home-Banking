using Models;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ViewModels
{
    public class SignUpViewModel : BaseViewModel
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }

        public ICommand RegisterCommand { get; }
        public ICommand NavigateToSignInCommand { get; }

        public SignUpViewModel(IUserService userService, INavigationService navigationService, IDialogService dialogService)
        {
            _userService = userService;
            _navigationService = navigationService;
            _dialogService = dialogService;
            RegisterCommand = new RelayCommand(Register);
            
            NavigateToSignInCommand = new RelayCommand(param => _navigationService.NavigateToAsync("//SignInPage"));

        }

        private async void Register(object parameter)
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

            try
            {
                var newUser = new User
                {
                    FullName = FullName,
                    UserName = UserName,
                    Email = Email,
                    Password = password,
                    CreatedAt = DateTime.UtcNow,
                    IsGiftClaimed = false
                };

                await _userService!.CreateAsync(newUser);
                await _dialogService!.ShowAlertAsync("Registro Exitoso", "Tu cuenta ha sido creada exitosamente. Ahora puedes iniciar sesión.", "Ok");

                await Task.Delay(3000);
                await _navigationService!.NavigateToAsync("//SignInPage");
            }
            catch (Exception e)
            {
                var innerMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                await _dialogService!.ShowAlertAsync("Error de Registro", $"Ocurrió un error al crear tu cuenta: {innerMessage}", "Ok");
            }
        }
    }
}
