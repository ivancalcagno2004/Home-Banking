using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Interfaces;

namespace ViewModels
{
    public class AccountViewModel : BaseViewModel
    {
        public ObservableCollection<Account> Accounts { get; }

        private Account? _selectedAccount;
        public Account? SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                _selectedAccount = value;
                OnPropertyChanged();
            }
        }

        public AccountViewModel(IAccountService accountService)
        {
            _accountService = accountService;
            Accounts = new ObservableCollection<Account>();
            LoadAccountAsync();
        }

        private async void LoadAccountAsync()
        {
            var accounts = await _accountService.GetAllAsync();
            Accounts.Clear();

            foreach (var account in accounts)
            {
                Accounts.Add(account);
            }
        }
    }
}
