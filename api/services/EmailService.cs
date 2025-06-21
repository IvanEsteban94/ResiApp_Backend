using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace api.services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string fromEmail = "Notifications_resiapp@gmail.com";
        private readonly string fromPassword = "TU_CONTRASEÑA_GENERADA_APP";

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string bodyHtml, string? cc = null)
        {
            var message = new MailMessage();
            message.From = new MailAddress(fromEmail, "ResiApp Notificaciones");
            message.To.Add(new MailAddress(toEmail));

            if (!string.IsNullOrWhiteSpace(cc))
                message.CC.Add(cc);

            message.Subject = subject;
            message.IsBodyHtml = true;

            // Diseño básico con logo (puedes mejorar estilos con CSS)
            message.Body = $@"
                <div style='font-family:Arial,sans-serif; padding:20px;'>
                    <img src='https://i.ibb.co/YZXwmtM/logo.png' alt='ResiApp Logo' height='60' style='margin-bottom:20px;'/>
                    <h2 style='color:#2C3E50;'>Notificación de ResiApp</h2>
                    <div style='font-size:16px; color:#333;'>{bodyHtml}</div>
                    <hr style='margin-top:30px;'>
                    <p style='font-size:12px; color:#888;'>Este correo fue enviado automáticamente por ResiApp.</p>
                </div>";

            using var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(fromEmail, fromPassword)
            };

            await smtp.SendMailAsync(message);
        }
    }
}

