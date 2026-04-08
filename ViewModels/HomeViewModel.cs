using Models;
using Services;
using Services.Interfaces;
using ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;
using Services.Implementations;
using Models.DTO;
using Microsoft.Extensions.Logging;


namespace ViewModels
{
    /// <summary>
    /// ViewModel de la pantalla principal. Carga las cuentas del usuario,
    /// construye un mensaje de bienvenida y gestiona la acción de reclamar un regalo.
    /// </summary>
    public class HomeViewModel : BaseViewModel
    {
        private readonly User _currentUser;

        private bool _isGiftAvailable;

        public bool IsGiftAvailable
        {
            get => _isGiftAvailable;
            set { _isGiftAvailable = value; OnPropertyChanged(); }
        }

        public ICommand ClaimGiftCommand { get; }

        public ObservableCollection<AccountDTO> Accounts { get; set; }

        public string WelcomeMessage { get; set; }

        public HomeViewModel(UserSession currentUser, IAccountService accountService, IUserService userService, IDialogService dialogService, ILogger<HomeViewModel> logger)
        {
            _currentUser = currentUser.CurrentUser!;
            _userService = userService;
            _accountService = accountService;
            _dialogService = dialogService;
            _logger = logger;


            WelcomeMessage = $"Bienvenido/a, {_currentUser.FullName}!";
            Accounts = new ObservableCollection<AccountDTO>();
            ClaimGiftCommand = new RelayCommand(ExecuteClaimGift);
        }

        public async Task<AccountDTO?> GetAccountData() {
            return Accounts!.FirstOrDefault();
        }

        private async void ExecuteClaimGift(object o)
        {
            if (!IsGiftAvailable) return;

            try
            {
                _logger?.LogInformation("HomeViewModel: reclamando regalo");
                var mainAccount = Accounts.FirstOrDefault();
                if (mainAccount == null) return;

                decimal regalo = 50000m;

                mainAccount.Balance += regalo;
                IsGiftAvailable = false;
                _currentUser.IsGiftClaimed = true;


                await _accountService!.ClaimGiftAsync(mainAccount.Id, regalo);
                await _userService!.UpdateUser(_currentUser);


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
        }

        public async Task LoadData()
        {
            try
            {
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
                throw;
            }
        }
    }
}
