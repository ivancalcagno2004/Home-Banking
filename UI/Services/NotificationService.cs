using Models.DTO;
using Plugin.LocalNotification;
using Plugin.LocalNotification.Core.Models;
using Plugin.LocalNotification.Core.Models.AndroidOption;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using INotificationService = Services.Interfaces.INotificationService;

namespace UI.Services
{
    public class NotificationService : INotificationService
    {
        public async Task CheckAndNotifyServicesAsync(IEnumerable<PaymentDTO> servicios)
        {
            if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            }

            foreach (var servicio in servicios)
            {
                var diasParaVencer = (servicio.ExpDate - DateTime.Now).TotalDays;

                if (diasParaVencer < 0)
                {
                    SendNotification(servicio.Id, "¡Servicio Vencido!",
                        $"Tu pago de {servicio.ServiceName} venció el {servicio.ExpDate:dd/MM}.");
                }
                else if (diasParaVencer <= 3)
                {
                    SendNotification(servicio.Id, "Próximo Vencimiento",
                        $"Tu servicio {servicio.ServiceName} vence en {(int)Math.Ceiling(diasParaVencer)} días.");
                }
            }
        }

        private void SendNotification(int id, string titulo, string mensaje)
        {
            var request = new NotificationRequest
            {
                NotificationId = id,
                Title = titulo,
                Description = mensaje,
                BadgeNumber = 1,
                Android = new AndroidOptions
                {
                    Priority = AndroidPriority.High,
                    IconSmallName = new AndroidIcon("ic_notification"),
                },
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(2)
                }
            };

            LocalNotificationCenter.Current.Show(request);
        }
    }
}
