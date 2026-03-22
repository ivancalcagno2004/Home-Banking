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
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        protected IAccountService? _accountService;
        protected IUserService? _userService;
        protected ITransactionService? _transactionService;
        protected INavigationService? _navigationService;
        protected IDialogService? _dialogService;
        protected IPaymentService? _paymentService;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
