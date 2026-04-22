using HomeBanking.Data.UnitOfWork;
using Models;
using Models.DTO;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    /// <summary>
    /// Servicio de transacciones. Expone consultas de movimientos del usuario y
    /// ejecuta transferencias entre cuentas registrando el historial.
    /// </summary>
    public class TransactionService : BaseService, ITransactionService
    {
        public TransactionService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<TransactionDTO>> GetRecentTransactions(int userId, int count)
        {
            var transactions = await _unitOfWork.Transactions.GetRecentByUserIdAsync(userId, count);

            var dtoList = transactions.Select(t =>
            {
                bool isIncome = t.ToAccount != null && t.ToAccount.UserId == userId;

                return new TransactionDTO
                {
                    Date = t.CreatedAt,
                    Description = t.Description,
                    Amount = isIncome ? $"+ ${t.Amount:N2}" : $"- ${t.Amount:N2}",
                    Color = isIncome ? "#2E7D32" : "#dc3545",
                    Icon = isIncome ? "📈" : "📉"
                };
            }).ToList();

            return dtoList;
        }

        public async Task<IEnumerable<TransactionDTO>> GetTransactionsByUserIdAsync(int userId)
        {
            var transactions = await _unitOfWork.Transactions.GetByUserIdAsync(userId);

            var dtoList = transactions.Select(t =>
            {
                bool isIncome = t.ToAccount != null && t.ToAccount.UserId == userId;

                return new TransactionDTO
                {
                    Date = t.CreatedAt,

                    Description = t.Description,

                    Amount = isIncome ? $"+ ${t.Amount:N2}" : $"- ${t.Amount:N2}",
                    Color = isIncome ? "#2E7D32" : "#dc3545"
                };
            }).ToList();

            return dtoList;
        }

        public async Task TransferToAsync(int fromAccountId, string toCBUOrAlias, decimal amount)
        {
            // Validar cuenta origen
            var originAccount = await _unitOfWork.Accounts.GetByIdAsync(fromAccountId);
            if (originAccount == null) throw new Exception("La cuenta de origen no existe.");

            // Validar saldo
            if (originAccount.Balance < amount) throw new Exception("Fondos insuficientes.");

            // Buscar cuenta destino
            var destAccount = await _unitOfWork.Accounts.GetAccountByCBUOrAliasAsync(toCBUOrAlias);
            if (destAccount == null) throw new Exception("No se encontró la cuenta destino (CBU/Alias inválido).");

            // Validar que no se transfiera a sí mismo
            if (originAccount.AccountId == destAccount.AccountId) throw new Exception("No podés transferirte a la misma cuenta.");

            // Mover la plata 
            originAccount.Balance -= amount;
            destAccount.Balance += amount;

            // Actualizar entidades para que EF sepa que cambiaron
            _unitOfWork.Accounts.Update(originAccount);
            _unitOfWork.Accounts.Update(destAccount);

            // Crear el registro de la transacción (Historial)
            var transaction = new Transaction
            {
                FromAccountId = originAccount.AccountId,
                ToAccountId = destAccount.AccountId,
                FromAccount = originAccount,
                ToAccount = destAccount,
                Amount = amount,
                Description = $"Transferencia de {originAccount.User.FullName} a {destAccount.User.FullName}",
                CreatedAt = DateTime.UtcNow,
                Status = "Completed"
            };

            await _unitOfWork.Transactions.AddAsync(transaction);

            // GUARDAR
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
