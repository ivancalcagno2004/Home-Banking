using Models;
using Models.DTO;
using Services.Implementations;
using Services.Interfaces;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ViewModels
{
    /// <summary>
    /// ViewModel de transferencias. Permite seleccionar cuenta origen, ingresar
    /// destino (CBU/Alias) y monto, confirmar la operación y ejecutar la transferencia.
    /// Además, notifica por email al destinatario cuando corresponde.
    /// </summary>
    public partial class TransferViewModel : BaseViewModel
    {
        private readonly User _currentUser;
        private readonly IEmailService _emailService;

        public ObservableCollection<AccountDTO> MyAccounts { get; set; }

        [ObservableProperty]
        private AccountDTO? _selectedOriginAccount;

        public string? DestinationCBU_Alias { get; set; }

        public decimal Amount { get; set; }

        public string Titulo { get; }

        public TransferViewModel(UserSession user, IAccountService accountService, ITransactionService transactionService, IDialogService dialogService, IEmailService emailService, IUserService userService, ILogger<TransferViewModel> logger)
        {
            Titulo = "Transferencias";
            _currentUser = user.CurrentUser!;
            _accountService = accountService;
            _transactionService = transactionService;
            _dialogService = dialogService;
            _emailService = emailService;
            _userService = userService;
            _logger = logger;

            MyAccounts = new ObservableCollection<AccountDTO>();
        }

        public async Task LoadAccounts()
        {
            try
            {
                _logger?.LogInformation("TransferViewModel: cargando cuentas de origen");

                var accounts = await _accountService!.GetAccountsByUserId(_currentUser.UserId);
                MyAccounts.Clear();
                foreach (var acc in accounts) MyAccounts.Add(acc);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "TransferViewModel: error al cargar cuentas");
                return;
            }

            // Seleccionar la primera por defecto para ahorrarle un clic al usuario
            if (MyAccounts.Count > 0) SelectedOriginAccount = MyAccounts[0];
        }

        [RelayCommand]
        private async Task ExecuteTransfer()
        {
            _logger?.LogInformation("TransferViewModel: iniciando transferencia");
            if (SelectedOriginAccount == null)
            {
                _logger?.LogWarning("TransferViewModel: intento de transferencia sin cuenta de origen seleccionada");
                await _dialogService!.ShowAlertAsync("Cuenta de origen no seleccionada", "Por favor, seleccioná una cuenta de origen para realizar la transferencia.", "Ok");
                return;
            }
            if (Amount <= 0)
            {
                _logger?.LogWarning("TransferViewModel: intento de transferencia con monto inválido: {Amount}", Amount);
                await _dialogService!.ShowAlertAsync("Monto inválido", "Por favor, ingresá un monto mayor a cero para la transferencia.", "Ok");
                return;
            }
            if (string.IsNullOrWhiteSpace(DestinationCBU_Alias))
            {
                _logger?.LogWarning("TransferViewModel: intento de transferencia con destino vacío");
                await _dialogService!.ShowAlertAsync("Cuenta de destino vacía", "Por favor, ingresá el CBU o Alias de la cuenta de destino para realizar la transferencia.", "Ok");
                return;
            }

            UserProfileDTO? userDest = await _userService!.GetUserByCBUoAlias(DestinationCBU_Alias);

            if (userDest == null)
            {
                await _dialogService!.ShowAlertAsync("Cuenta de destino inexistente", "Por favor, ingresá el CBU o Alias de la cuenta de destino para realizar la transferencia.", "Ok");
                return;
            }

            if (IsBusy) return;

            try
            {
                IsBusy = true;
                _logger?.LogInformation("TransferViewModel: confirmando transferencia con el usuario");
                bool result = await _dialogService!.ShowConfirmationAsync(
                    "Confirmar transferencia",
                    $"¿Estás seguro que querés transferir ${Amount:N2} desde {SelectedOriginAccount.Alias} a {userDest.FullName} en su cuenta {DestinationCBU_Alias}?",
                    "Sí, transferir",
                    "No, cancelar"
                );

                if (!result)
                {
                    _logger?.LogInformation("TransferViewModel: transferencia cancelada por el usuario");
                    return;
                }

                await _transactionService!.TransferToAsync(SelectedOriginAccount.Id, DestinationCBU_Alias, Amount);

                _logger?.LogInformation("TransferViewModel: transferencia OK");

                await _dialogService.ShowAlertAsync(
                    "Transferencia exitosa",
                    $"Se han transferido ${Amount:N2} desde {SelectedOriginAccount.Alias} a {userDest.FullName} en su cuenta {DestinationCBU_Alias}.",
                    "Ok"
                );

                if (!string.IsNullOrWhiteSpace(userDest.Email))
                {
                    await _emailService.SendTransferNotificationAsync(userDest.Email, userDest.FullName!, _currentUser.FullName ,SelectedOriginAccount.Alias!, Amount);
                }

                // Recargar las cuentas para actualizar saldos
                await LoadAccounts();
                Amount = 0;
                DestinationCBU_Alias = string.Empty;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "TransferViewModel: error en transferencia");
                await _dialogService!.ShowAlertAsync(
                    "Error en la transferencia",
                    $"Ocurrió un error al intentar realizar la transferencia: {ex.Message}",
                    "Ok"
                );
            }
            finally { IsBusy = false; }
        }
    }
}
