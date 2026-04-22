using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui.Controls;
using System.Linq;

namespace UI.Services
{
    /// <summary>
    /// Servicio de diálogos de la capa UI.
    /// Encapsula el uso de <see cref="Page.DisplayAlertAsync(string, string, string)"/> para mostrar alertas
    /// y confirmaciones desde ViewModels mediante <see cref="IDialogService"/>.
    /// </summary>
    public class DialogService : IDialogService
    {
        public async Task ShowAlertAsync(string title, string message, string buttonText)
        {
            var window = Application.Current?.Windows.FirstOrDefault();

            if (window?.Page != null)
            {
                await window.Page.DisplayAlertAsync(title, message, buttonText);
            }
        }

        public async Task<bool> ShowConfirmationAsync(string title, string message, string confirmText, string cancelText)
        {
            var window = Application.Current?.Windows.FirstOrDefault();
            if (window?.Page != null)
            {
                return await window.Page.DisplayAlertAsync(title, message, confirmText, cancelText);
            }
            return false;
        }

        public async Task<string?> ShowActionSheetAsync(string title, string cancelText, string? destructionText, params string[] buttons)
        {
            var window = Application.Current?.Windows.FirstOrDefault();
            if (window?.Page != null)
            {
                return await window.Page.DisplayActionSheetAsync(title, cancelText, destructionText, buttons);
            }
            return null;
        }
    }
}
