using Models;
using Services.Implementations;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ViewModels
{
    public class TransferViewModel : BaseViewModel
    {
        private readonly User _currentUser;

        public ObservableCollection<Account> MyAccounts { get; set; }

        private Account? _selectedOriginAccount;

        public Account? SelectedOriginAccount
        {
            get => _selectedOriginAccount;
            set
            {
                _selectedOriginAccount = value;
                OnPropertyChanged();
            }
        }

        public string? DestinationCBU_Alias { get; set; }
        public decimal Amount { get; set; }

        public ICommand TransferCommand { get; }

        public string Titulo { get; }

        public TransferViewModel(UserSession user, IAccountService accountService, ITransactionService transactionService, IDialogService dialogService)
        {
            Titulo = "Transferencias";
            _currentUser = user.CurrentUser!;
            _accountService = accountService;
            _transactionService = transactionService;
            _dialogService = dialogService;

            MyAccounts = new ObservableCollection<Account>();

            TransferCommand = new RelayCommand(ExecuteTransfer);

            LoadAccounts();
        }

        private async void LoadAccounts()
        {
            var accounts = await _accountService!.GetAccountsByUserId(_currentUser.UserId);
            MyAccounts.Clear();
            foreach (var acc in accounts) MyAccounts.Add(acc);

            // Seleccionar la primera por defecto para ahorrarle un clic al usuario
            if (MyAccounts.Count > 0) SelectedOriginAccount = MyAccounts[0];
        }

        private async void ExecuteTransfer(object obj)
        {
            if (SelectedOriginAccount == null)
            {
                await _dialogService!.ShowAlertAsync("Cuenta de origen no seleccionada", "Por favor, seleccioná una cuenta de origen para realizar la transferencia.", "Ok");
                return;
            }
            if (Amount <= 0)
            {
                await _dialogService!.ShowAlertAsync("Monto inválido", "Por favor, ingresá un monto mayor a cero para la transferencia.", "Ok");
                return;
            }
            if (string.IsNullOrWhiteSpace(DestinationCBU_Alias))
            {
                await _dialogService!.ShowAlertAsync("Cuenta de destino vacía", "Por favor, ingresá el CBU o Alias de la cuenta de destino para realizar la transferencia.", "Ok");
                return;
            }

            try
            {

                bool result = await _dialogService!.ShowConfirmationAsync(
                    "Confirmar transferencia",
                    $"¿Estás seguro que querés transferir ${Amount:N2} desde {SelectedOriginAccount.Alias} a {DestinationCBU_Alias}?",
                    "Sí, transferir",
                    "No, cancelar"
                );

                if (!result) return;

                await _transactionService!.TransferToAsync(SelectedOriginAccount.AccountId, DestinationCBU_Alias, Amount);

                await _dialogService.ShowAlertAsync(
                    "Transferencia exitosa",
                    $"Se han transferido ${Amount:N2} desde {SelectedOriginAccount.Alias} a {DestinationCBU_Alias}.",
                    "Ok"
                );

                // Recargar las cuentas para actualizar saldos
                LoadAccounts();
                Amount = 0;
                DestinationCBU_Alias = string.Empty;
            }
            catch (Exception ex)
            {
                await _dialogService!.ShowAlertAsync(
                    "Error en la transferencia",
                    $"Ocurrió un error al intentar realizar la transferencia: {ex.Message}",
                    "Ok"
                );
            }
        }
    }
}
