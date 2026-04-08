using Microsoft.Extensions.Logging;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    /// <summary>
    /// Clase base para los ViewModels. Implementa <see cref="INotifyPropertyChanged"/>
    /// y centraliza dependencias de servicios comunes junto con el estado <see cref="IsBusy"/>.
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        protected IAccountService? _accountService;
        protected IUserService? _userService;
        protected ITransactionService? _transactionService;
        protected INavigationService? _navigationService;
        protected IDialogService? _dialogService;
        protected IPaymentService? _paymentService;
        protected ICredentialService? _credentialService;
        protected ILogger? _logger;

        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _isBusy;
        /// <summary>
        /// Indica si el ViewModel se encuentra ejecutando una operación en curso.
        /// Útil para bloquear acciones repetidas y mostrar indicadores de carga.
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        protected void OnPropertyChanged([CallerMemberName]string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
