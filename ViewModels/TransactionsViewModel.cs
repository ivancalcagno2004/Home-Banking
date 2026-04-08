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
using Microsoft.Extensions.Logging;

namespace ViewModels
{
    /// <summary>
    /// ViewModel de transacciones. Obtiene y expone el listado de movimientos del usuario
    /// desde <see cref="ITransactionService"/> para ser mostrado en la UI.
    /// </summary>
    public class TransactionsViewModel : BaseViewModel
    {
        private readonly User _currentUser;

        public ObservableCollection<TransactionDTO> Transactions { get; set; }

        public TransactionsViewModel(UserSession currentUser, ITransactionService transactionService, ILogger<TransactionsViewModel> logger)
        {
            _currentUser = currentUser.CurrentUser!;
            _transactionService = transactionService;
            Transactions = new ObservableCollection<TransactionDTO>();
            _logger = logger;

        }

        public async Task LoadTransactions()
        {
            try
            {
                _logger?.LogInformation("TransactionsViewModel: cargando transacciones");
                var transactions = await _transactionService!.GetTransactionsByUserIdAsync(_currentUser.UserId);
                Transactions.Clear();
                foreach (var transaction in transactions)
                {
                    Transactions.Add(transaction);
                }

                _logger?.LogInformation("TransactionsViewModel: transacciones cargadas ({Count})", Transactions.Count);

            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "TransactionsViewModel: error al cargar transacciones");
                await _dialogService!.ShowAlertAsync("Error", $"No se pudieron cargar las transacciones: {ex.Message}", "Ok");
            }
        }
    }
}
