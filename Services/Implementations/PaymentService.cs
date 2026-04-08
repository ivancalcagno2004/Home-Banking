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
    /// Servicio de pagos. Obtiene vencimientos/historial y ejecuta el pago de servicios,
    /// afectando balances, marcando facturas como pagadas y registrando transacciones.
    /// </summary>
    public class PaymentService : BaseService, IPaymentService
    {
        public PaymentService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<PaymentDTO>> GetPendingPaymentsAsync(int userId, bool showPaid)
        {

            var payments = await _unitOfWork.Payments.GetPendingByUserIdAsync(userId, showPaid);
            List<PaymentDTO> dtoList = new List<PaymentDTO>();

            IEnumerable<Transaction> transactions = null!;
            if (showPaid)
            {
                transactions = await _unitOfWork.Transactions.GetByUserIdAsync(userId);
            }

            foreach (var p in payments)
            {

                if (showPaid && transactions != null)
                {
                    var transaction = transactions.FirstOrDefault(t => t.Description.Contains($"Pago #{p.Id}"));

                    if (transaction != null)
                    {
                        p.PaymentDate = transaction.CreatedAt;
                    }
                }

                dtoList.Add(new PaymentDTO
                {
                    Id = p.Id,
                    ServiceName = p.ServiceName,
                    CategoryName = p.Category?.Name,
                    Amount = p.Amount,
                    ExpDate = p.ExpDate,
                    IsPaid = p.IsPaid,
                    PaymentDate = p.PaymentDate
                });
            }

            return dtoList;
        }

        public async Task PayServiceAsync(int paymentId, int fromAccountId)
        {
            // obtener la factura y la cuenta de origen
            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
            var fromAccount = await _unitOfWork.Accounts.GetByIdAsync(fromAccountId);
            var destAccount = await _unitOfWork.Accounts.GetAccountByCBUOrAliasAsync("PAGOS.SERVICIOS");

            // Validaciones
            if (payment == null) throw new Exception("La factura no existe.");
            if (payment.IsPaid) throw new Exception("Esta factura ya está pagada.");
            if (fromAccount == null) throw new Exception("La cuenta no existe.");
            if (fromAccount.Balance < payment.Amount) throw new Exception("Saldo insuficiente.");
            if (destAccount == null) throw new Exception("La cuenta destino no existe.");

            // Realizar el pago
            fromAccount.Balance -= payment.Amount;
            destAccount.Balance += payment.Amount;
            payment.IsPaid = true;

            // crear transacción
            var transaction = new Transaction
            {
                FromAccountId = fromAccountId,
                ToAccountId = destAccount.AccountId,
                FromAccount = fromAccount,
                ToAccount = destAccount,
                Amount = payment.Amount,
                Description = $"Pago #{payment.Id} de servicio: {payment.ServiceName}",
                CreatedAt = DateTime.UtcNow,
                Status = "Completed"
            };

            await _unitOfWork.Transactions.AddAsync(transaction);
            _unitOfWork.Accounts.Update(fromAccount);
            _unitOfWork.Accounts.Update(destAccount);
            _unitOfWork.Payments.Update(payment);

            await _unitOfWork.SaveChangesAsync();

            var map = new TransactionCategoryMap
            {
                TransactionId = transaction.TransactionId,
                TransactionCategoryId = payment.CategoryId,
                Transaction = transaction,
                TransactionCategory = payment.Category!
            };

            await _unitOfWork.TransactionCategoryMaps.AddAsync(map);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
