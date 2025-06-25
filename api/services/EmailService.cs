
using SendEmail.Models;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using api.Models.DTO;
using System.Net.Mail;
using System.Net;


namespace SendEmail.Services
{
    public class EmailService : IEmailService
    {

        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }


        public void SendEmail(EmailDTO request)
        {
            var mailMessage = new MailMessage();

            // ✳️ Importante: asignar el remitente
            mailMessage.From = new MailAddress(request.From);

            // Destinatarios
            foreach (var to in request.To)
                mailMessage.To.Add(to);

            // CC si existen
            if (request.Cc != null)
                foreach (var cc in request.Cc)
                    mailMessage.CC.Add(cc);

            // Asunto y cuerpo
            mailMessage.Subject = request.Subject;
            mailMessage.Body = request.Body;
            mailMessage.IsBodyHtml = true;

            // Configuración SMTP
            using var smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("notificationsresiapp@gmail.com", "pbxwqbcxtxdpelid"),
                EnableSsl = true
            };

            // Envío
            smtp.Send(mailMessage);
        }




    }
}