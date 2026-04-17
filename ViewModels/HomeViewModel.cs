using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTO;
using Services.Implementations;
using Services.Interfaces;
using System.Collections.ObjectModel;

namespace ViewModels
{
    /// <summary>
    /// ViewModel de la pantalla principal. Carga las cuentas del usuario,
    /// construye un mensaje de bienvenida y gestiona la acción de reclamar un regalo.
    /// </summary>
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly User _currentUser;

        [ObservableProperty]
        private bool _isGiftAvailable;

        [ObservableProperty]
        private string _welcomeMessage;

        public ObservableCollection<AccountDTO> Accounts { get; } = new();

        public HomeViewModel(UserSession userSession, IAccountService accountService, IUserService userService, IDialogService dialogService, ILogger<HomeViewModel> logger)
        {
            _currentUser = userSession.CurrentUser!;
            _userService = userService;
            _accountService = accountService;
            _dialogService = dialogService;
            _logger = logger;

            _welcomeMessage = $"Bienvenido/a, {_currentUser.FullName}!";
        }

        public async Task<AccountDTO?> GetAccountData() {
            return Accounts!.FirstOrDefault();
        }

        [RelayCommand]
        private async Task ClaimGift()
        {
            if (!IsGiftAvailable || IsBusy) return;

            try
            {
                IsBusy = true; 
                _logger?.LogInformation("HomeViewModel: reclamando regalo");

                var mainAccount = Accounts.FirstOrDefault();
                if (mainAccount == null) return;

                decimal regalo = 50000m;

                IsGiftAvailable = false;
                _currentUser.IsGiftClaimed = true;

                await _accountService!.ClaimGiftAsync(mainAccount.Id, regalo);
                await _userService!.UpdateUser(_currentUser);

                mainAccount.Balance += regalo;
                var index = Accounts.IndexOf(mainAccount);
                if (index != -1)
                {
                    Accounts.RemoveAt(index);
                    Accounts.Insert(index, mainAccount);
                }

                await _dialogService!.ShowAlertAsync(
                    "¡Regalo reclamado!",
                    $"Has recibido un regalo de {regalo:C} en tu cuenta principal.",
                    "Ok");

                _logger?.LogInformation("HomeViewModel: regalo acreditado");
            }
            catch (Exception ex)
            {
                IsGiftAvailable = true;
                _currentUser.IsGiftClaimed = false;

                await _dialogService!.ShowAlertAsync(
                    "Error",
                    $"Ocurrió un error al reclamar el regalo: {ex.Message}",
                    "Ok");

                _logger?.LogError(ex, "HomeViewModel: error al reclamar regalo");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task LoadData()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                _logger?.LogInformation("HomeViewModel: cargando datos");

                var userAccounts = await _accountService!.GetAccountsByUserId(_currentUser.UserId);

                Accounts.Clear();
                foreach (var account in userAccounts)
                {
                    Accounts.Add(account);
                }

                IsGiftAvailable = !_currentUser.IsGiftClaimed;

                _logger?.LogInformation("HomeViewModel: datos cargados");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "HomeViewModel: error al cargar datos");

                await _dialogService!.ShowAlertAsync(
                    "Error de conexión",
                    "No pudimos sincronizar tus cuentas. Revisa tu internet.",
                    "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}