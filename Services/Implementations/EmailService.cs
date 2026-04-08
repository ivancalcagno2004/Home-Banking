using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Implementations
{
    /// <summary>
    /// Servicio de email. Envía notificaciones (por ejemplo, transferencia recibida)
    /// utilizando SMTP a través de MailKit/MimeKit.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly string _bancoEmail = "tandilbank.app@gmail.com";
        private readonly string _bancoPassword = "znzg ubov ifse uaxq";
        public async Task SendTransferNotificationAsync(string toEmail, string receiverName,string senderName, string senderAlias, decimal amount)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Tandil Bank", _bancoEmail));
            email.To.Add(new MailboxAddress(receiverName, toEmail));
            email.Subject = "¡Transferencia Recibida! 💸";

            var builder = new BodyBuilder
            {
                HtmlBody = $@"
                <div style='font-family: Arial, Helvetica, sans-serif; background-color: #f4f7f6; padding: 30px 10px; text-align: center;'>
            
                    <div style='max-width: 500px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 10px rgba(0,0,0,0.05); border: 1px solid #e0e0e0; text-align: left;'>
                
                        <div style='background-color: #004a94; padding: 25px 20px; text-align: center;'>
                            <h2 style='color: #ffffff; margin: 0; font-size: 24px; letter-spacing: 1px;'>TANDIL BANK</h2>
                            <p style='font-size: 18px; line-height: 1.6; color: #ffffff; margin-bottom: 0;'>
                               Banca Personal
                            </p>
                        </div>
                
                        <div style='padding: 30px; color: #333333;'>
                            <h3 style='margin-top: 0; color: #004a94; font-size: 20px;'>Hola, {receiverName}</h3>
                            <p style='font-size: 15px; line-height: 1.6; color: #555555;'>Te informamos que se ha acreditado una nueva transferencia en tu cuenta. 💵</p>
                    
                            <div style='background-color: #f8f9fa; border-left: 4px solid #004a94; padding: 15px 20px; margin: 25px 0; border-radius: 0 4px 4px 0;'>
                                <p style='margin: 8px 0; font-size: 15px; color: #333;'>
                                    <strong>Monto recibido:</strong> 
                                    <span style='color: #28a745; font-weight: bold; font-size: 18px;'>${amount:N2}</span>
                                </p>
                                <p style='margin: 8px 0; font-size: 14px; color: #555;'>
                                    <strong>Enviado por:</strong> {senderName}
                                </p>
                                <p style='margin: 8px 0; font-size: 14px; color: #555;'>
                                    <strong>Cuenta origen:</strong> {senderAlias}
                                </p>
                            </div>
                        </div>
                
                        <div style='background-color: #fcfcfc; padding: 20px; text-align: center; border-top: 1px solid #eeeeee;'>
                            <p style='color: #aaa; font-size: 11px; margin: 0 0 8px 0;'>Este es un mensaje generado automáticamente, por favor no respondas a este correo.</p>
                            <p style='color: #999999; font-size: 12px; margin: 0;'>&copy; {DateTime.Now.Year} Tandil Bank. Todos los derechos reservados.</p>
                        </div>
                
                    </div>
            
                </div>"
            };

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            try
            {
                smtp.CheckCertificateRevocation = false;
                // Conectar a los servidores de Gmail
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_bancoEmail, _bancoPassword);
                await smtp.SendAsync(email);
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
