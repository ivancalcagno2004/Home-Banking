using Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface INotificationService
    {
        public Task CheckAndNotifyServicesAsync(IEnumerable<PaymentDTO> servicios);
    }
}
