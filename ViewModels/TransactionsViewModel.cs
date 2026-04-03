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

namespace ViewModels
{
    public class TransactionsViewModel : BaseViewModel
    {
        private readonly User _currentUser;

        public ObservableCollection<TransactionDTO> Transactions { get; set; }

        public TransactionsViewModel(UserSession currentUser, ITransactionService transactionService)
        {
            _currentUser = currentUser.CurrentUser!;
            _transactionService = transactionService;
            Transactions = new ObservableCollection<TransactionDTO>();

        }

        public async Task LoadTransactions()
        {
            try
            {
                var transactions = await _transactionService!.GetTransactionsByUserIdAsync(_currentUser.UserId);
                Transactions.Clear();
                foreach (var transaction in transactions)
                {
                    Transactions.Add(transaction);
                }

            }
            catch (Exception ex)
            {
                await _dialogService!.ShowAlertAsync("Error", $"No se pudieron cargar las transacciones: {ex.Message}", "Ok");
            }
        }
    }
}
