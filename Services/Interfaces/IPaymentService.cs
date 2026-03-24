using Models;
using Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDTO>> GetPendingPaymentsAsync(int userId, bool showPaid);
        Task PayServiceAsync(int paymentId, int fromAccountId);
    }
}
