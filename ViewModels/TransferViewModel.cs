using Models;
using Models.DTO;
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
using System.Diagnostics;

namespace ViewModels
{
    public class TransferViewModel : BaseViewModel
    {
        private readonly User _currentUser;

        public ObservableCollection<AccountDTO> MyAccounts { get; set; }

        private AccountDTO? _selectedOriginAccount;

        public AccountDTO? SelectedOriginAccount
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

            MyAccounts = new ObservableCollection<AccountDTO>();

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
            Debug.WriteLine("[UserAction] TransferCommand ejecutado.");
            if (SelectedOriginAccount == null)
            {
                Debug.WriteLine("[UserAction] Transfer: sin cuenta de origen seleccionada.");
                await _dialogService!.ShowAlertAsync("Cuenta de origen no seleccionada", "Por favor, seleccioná una cuenta de origen para realizar la transferencia.", "Ok");
                return;
            }
            if (Amount <= 0)
            {
                Debug.WriteLine($"[UserAction] Transfer: monto inválido Amount={Amount}.");
                await _dialogService!.ShowAlertAsync("Monto inválido", "Por favor, ingresá un monto mayor a cero para la transferencia.", "Ok");
                return;
            }
            if (string.IsNullOrWhiteSpace(DestinationCBU_Alias))
            {
                Debug.WriteLine("[UserAction] Transfer: destino vacío.");
                await _dialogService!.ShowAlertAsync("Cuenta de destino vacía", "Por favor, ingresá el CBU o Alias de la cuenta de destino para realizar la transferencia.", "Ok");
                return;
            }

            try
            {
                Debug.WriteLine($"[UserAction] Transfer: solicitando confirmación. From='{SelectedOriginAccount.Alias}' To='{DestinationCBU_Alias}' Amount={Amount}.");
                bool result = await _dialogService!.ShowConfirmationAsync(
                    "Confirmar transferencia",
                    $"¿Estás seguro que querés transferir ${Amount:N2} desde {SelectedOriginAccount.Alias} a {DestinationCBU_Alias}?",
                    "Sí, transferir",
                    "No, cancelar"
                );

                if (!result)
                {
                    Debug.WriteLine("[UserAction] Transfer cancelada por el usuario.");
                    return;
                }

                await _transactionService!.TransferToAsync(SelectedOriginAccount.Id, DestinationCBU_Alias, Amount);

                Debug.WriteLine("[UserAction] Transfer OK.");

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
                Debug.WriteLine($"[UserAction] Transfer excepción: {ex}");
                await _dialogService!.ShowAlertAsync(
                    "Error en la transferencia",
                    $"Ocurrió un error al intentar realizar la transferencia: {ex.Message}",
                    "Ok"
                );
            }
        }
    }
}
