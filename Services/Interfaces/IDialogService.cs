using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IDialogService
    {
        Task ShowAlertAsync(string title, string message, string buttonText);
        Task<bool> ShowConfirmationAsync(string title, string message, string confirmText, string cancelText);
    }
}
