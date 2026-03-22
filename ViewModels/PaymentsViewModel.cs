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

namespace ViewModels
{
    public class PaymentsViewModel : BaseViewModel
    {
        private readonly User _currentUser;

        public ObservableCollection<ServicePayment> PendingPayments { get; set; }

        public ICommand PayCommand { get; }

        private bool _showHistory;


        public bool ShowHistory
        {
            get => _showHistory;
            set
            {
                _showHistory = value;
                OnPropertyChanged();
                LoadData();
            }
        }

        public string Title => ShowHistory ? "Historial de Pagos" : "Mis Vencimientos";

        public PaymentsViewModel(UserSession currentUser, IPaymentService paymentService, IAccountService accountService, IDialogService dialogService)
        {
            _currentUser = currentUser.CurrentUser!;
            _paymentService = paymentService;
            _accountService = accountService;
            _dialogService = dialogService;

            PendingPayments = new ObservableCollection<ServicePayment>();
            PayCommand = new RelayCommand(ExecutePay);

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
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
                await _dialogService!.ShowAlertAsync("Error al cargar pagos", $"No se pudieron cargar los pagos: {ex.Message}", "Ok");
            }
        }

        private async void ExecutePay(object parameter)
        {
            if (parameter is ServicePayment paymentToPay)
            {
                try
                {
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

   
                    await _paymentService!.PayServiceAsync(paymentToPay.Id, accountToUse.AccountId);

                    await _dialogService.ShowAlertAsync("Pago Exitoso", $"Has pagado {paymentToPay.ServiceName} por ${paymentToPay.Amount} exitosamente.", "Ok");

                    LoadData();
                }
                catch (Exception ex)
                {
                    await _dialogService!.ShowAlertAsync("Error al realizar el pago", $"No se pudo completar el pago: {ex.Message}", "Ok");
                }
            }
        }
    }
}
