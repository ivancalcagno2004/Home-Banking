using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Interfaces;
using Models.DTO;
using Microsoft.Extensions.Logging;

namespace ViewModels
{
    /// <summary>
    /// ViewModel de cuentas. Expone la colección de cuentas del usuario para
    /// ser consumida por la UI y carga la información desde <see cref="IAccountService"/>.
    /// </summary>
    public class AccountViewModel : BaseViewModel
    {
        public ObservableCollection<AccountDTO> Accounts { get; }

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

        public AccountViewModel(IAccountService accountService, ILogger<AccountViewModel> logger)
        {
            _accountService = accountService;
            Accounts = new ObservableCollection<AccountDTO>();
            _logger = logger;
            LoadAccountAsync();
        }

        private async void LoadAccountAsync()
        {
            try
            {
                _logger?.LogInformation("AccountViewModel: cargando cuentas");

                var accounts = await _accountService!.GetAllAsync();
                Accounts.Clear();

                foreach (var account in accounts)
                {
                    Accounts.Add(account);
                }

                _logger?.LogInformation("AccountViewModel: cuentas cargadas ({Count})", Accounts.Count);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "AccountViewModel: error al cargar cuentas");
            }
        }
    }
}
