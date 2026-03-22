using HomeBanking.Data.UnitOfWork;
using Models;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class TransactionService : BaseService, ITransactionService
    {
        public TransactionService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByUserIdAsync(int userId)
        {
            return await _unitOfWork.Transactions.GetByUserIdAsync(userId);
        }

        /*public async Task TransferAsync(int fromAccountId, int toAccountId, decimal amount)
        {
            var fromAccount = await _unitOfWork.Accounts.GetByIdAsync(fromAccountId);
            var toAccount = await _unitOfWork.Accounts.GetByIdAsync(toAccountId);

            if (fromAccount == null || toAccount == null)
            {
                throw new Exception("One or both accounts not found.");
            }

            if (fromAccount.Balance < amount)
            {
                throw new Exception("Insufficient funds.");
            }

            fromAccount.Balance -= amount;
            toAccount.Balance += amount;

            _unitOfWork.Accounts.Update(fromAccount);
            _unitOfWork.Accounts.Update(toAccount);

            await _unitOfWork.Transactions.AddAsync(new Transaction
            {
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId,
                FromAccount = fromAccount,
                ToAccount = toAccount,
                Amount = amount,
                Status = "Completed",
                Description = $"",
                CreatedAt = DateTime.UtcNow
            });

            await _unitOfWork.SaveChangesAsync();
        }*/

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
