using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Services.Interfaces;

namespace ViewModels
{
    /// <summary>
    /// Clase base para los ViewModels. Hereda de <see cref="ObservableObject"/>
    /// y centraliza dependencias de servicios comunes junto con el estado IsBusy.
    /// </summary>
    public abstract partial class BaseViewModel : ObservableObject
    {
        protected IAccountService? _accountService;
        protected IUserService? _userService;
        protected ITransactionService? _transactionService;
        protected INavigationService? _navigationService;
        protected IDialogService? _dialogService;
        protected IPaymentService? _paymentService;
        protected ICredentialService? _credentialService;
        protected ILogger? _logger;

        /// <summary>
        /// Indica si el ViewModel se encuentra ejecutando una operación en curso.
        /// Útil para bloquear acciones repetidas y mostrar indicadores de carga.
        /// </summary>
        [ObservableProperty]
        private bool _isBusy;
    }
}
