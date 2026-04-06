using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Interfaces
{
    public interface IEmailService
    {
        Task SendTransferNotificationAsync(string toEmail, string receiverName, string senderName, string senderAlias, decimal amount);
    }
}
