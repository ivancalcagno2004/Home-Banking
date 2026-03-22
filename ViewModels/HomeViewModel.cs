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
using Services.Implementations;

namespace ViewModels
{
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

        public ObservableCollection<Account> Accounts { get; set; }

        public string WelcomeMessage { get; set; }

        public HomeViewModel(UserSession currentUser, IAccountService accountService, IUserService userService, IDialogService dialogService)
        {
            _currentUser = currentUser.CurrentUser!;
            _userService = userService;
            _accountService = accountService;
            _dialogService = dialogService;

            WelcomeMessage = $"Bienvenido/a, {_currentUser.FullName}!";
            Accounts = new ObservableCollection<Account>();
            ClaimGiftCommand = new RelayCommand(ExecuteClaimGift);
            _ = LoadData();
        }

        private async void ExecuteClaimGift(object o)
        {
            if (!IsGiftAvailable) return;

            try
            {
                var mainAccount = Accounts.FirstOrDefault();
                if (mainAccount == null) return;

                decimal regalo = 50000m;

                mainAccount.Balance += regalo;
                IsGiftAvailable = false;
                _currentUser.IsGiftClaimed = true;


                await _accountService!.UpdateAccount(mainAccount);
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
            }
            catch (Exception ex)
            {
                
                IsGiftAvailable = true;
                _currentUser.IsGiftClaimed = false;

                await _dialogService!.ShowAlertAsync(
                    "Error",
                    $"Ocurrió un error al reclamar el regalo: {ex.Message}",
                    "Ok");
            }
        }

        private async Task LoadData()
        {
            var userAccounts = await _accountService!.GetAccountsByUserId(_currentUser.UserId);

            Accounts.Clear();
            foreach (var account in userAccounts)
            {
                Accounts.Add(account);
            }
            IsGiftAvailable = !_currentUser.IsGiftClaimed;
        }
    }
}
