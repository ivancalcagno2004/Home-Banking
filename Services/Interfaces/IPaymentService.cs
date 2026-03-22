using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<ServicePayment>> GetPendingPaymentsAsync(int userId, bool showPaid);
        Task PayServiceAsync(int paymentId, int fromAccountId);
    }
}
