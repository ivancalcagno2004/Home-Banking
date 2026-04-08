using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Services.Interfaces;
using Services.Implementations;
using Models.DTO;
using Microsoft.Extensions.Logging;

namespace ViewModels
{
    /// <summary>
    /// ViewModel de pagos. Carga vencimientos o historial según <see cref="ShowHistory"/>
    /// y ejecuta el pago de un servicio utilizando una cuenta del usuario.
    /// </summary>
    public class PaymentsViewModel : BaseViewModel
    {
        private readonly User _currentUser;

        public ObservableCollection<PaymentDTO> PendingPayments { get; set; }

        public ICommand PayCommand { get; }

        private bool _showHistory;


        public bool ShowHistory
        {
            get => _showHistory;
            set
            {
                if (_showHistory != value)
                {
                    _showHistory = value;
                    OnPropertyChanged();
                    LoadData();
                }
            }
        }

        public string Title => ShowHistory ? "Historial de Pagos" : "Mis Vencimientos";

        public PaymentsViewModel(UserSession currentUser, IPaymentService paymentService, IAccountService accountService, IDialogService dialogService, ILogger<PaymentsViewModel> logger)
        {
            _currentUser = currentUser.CurrentUser!;
            _paymentService = paymentService;
            _accountService = accountService;
            _dialogService = dialogService;
            _logger = logger;

            PendingPayments = new ObservableCollection<PaymentDTO>();
            PayCommand = new RelayCommand(ExecutePay);

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                _logger?.LogInformation("PaymentsViewModel: cargando pagos (historial={ShowHistory})", ShowHistory);
                var payments = await _paymentService!.GetPendingPaymentsAsync(_currentUser.UserId, ShowHistory);
                PendingPayments.Clear();
                foreach (var payment in payments)
                {
                    PendingPayments.Add(payment);
                }
                OnPropertyChanged(nameof(Title));
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "PaymentsViewModel: error al cargar pagos");
                await _dialogService!.ShowAlertAsync("Error al cargar pagos", $"No se pudieron cargar los pagos: {ex.Message}", "Ok");
            }
        }

        private async void ExecutePay(object parameter)
        {
            if (parameter is PaymentDTO paymentToPay)
            {
                try
                {
                    _logger?.LogInformation("PaymentsViewModel: iniciando pago (PaymentId={PaymentId})", paymentToPay.Id);
                    var accounts = await _accountService!.GetAccountsByUserId(_currentUser.UserId);
                    var accountToUse = accounts.FirstOrDefault();

                    if (accountToUse == null)
                    {
                        await _dialogService!.ShowAlertAsync("No hay cuentas disponibles", "No tienes cuentas disponibles para realizar el pago. Por favor, crea una cuenta primero.", "Ok");
                        return;
                    }

                    bool result = await _dialogService!.ShowConfirmationAsync(
                        "Confirmar Pago",
                        $"¿Deseas pagar {paymentToPay.ServiceName} por ${paymentToPay.Amount} usando la cuenta {accountToUse.Alias}?",
                        "Confirmar",
                        "Cancelar"
                    );

                    if (!result) return;

   
                    await _paymentService!.PayServiceAsync(paymentToPay.Id, accountToUse.Id);

                    await _dialogService.ShowAlertAsync("Pago Exitoso", $"Has pagado {paymentToPay.ServiceName} por ${paymentToPay.Amount} exitosamente.", "Ok");

                    _logger?.LogInformation("PaymentsViewModel: pago OK (PaymentId={PaymentId})", paymentToPay.Id);

                    LoadData();
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "PaymentsViewModel: error al pagar (PaymentId={PaymentId})", paymentToPay.Id);
                    await _dialogService!.ShowAlertAsync("Error al realizar el pago", $"No se pudo completar el pago: {ex.Message}", "Ok");
                }
            }
        }
    }
}
