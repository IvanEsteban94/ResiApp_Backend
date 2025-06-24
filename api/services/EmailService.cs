
using SendEmail.Models;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using api.Models.DTO;


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
            var email = new MimeMessage();

            // Remitente
            email.From.Add(MailboxAddress.Parse(_config["Email:UserName"]));

            // Destinatario principal
            email.To.Add(MailboxAddress.Parse(request.To));

            // Si hay destinatario en copia (por ejemplo, si el rol es residente)
            if (!string.IsNullOrWhiteSpace(request.Cc))
            {
                email.Cc.Add(MailboxAddress.Parse(request.Cc));
            }

            email.Subject = request.Subject;

            email.Body = new TextPart(TextFormat.Html)
            {
                Text = request.Body
            };

            using var smtp = new SmtpClient();
            smtp.Connect(
                _config["Email:Host"],
                Convert.ToInt32(_config["Email:Port"]),
                SecureSocketOptions.StartTls
            );

            smtp.Authenticate(_config["Email:UserName"], _config["Email:PassWord"]);

            smtp.Send(email);
            smtp.Disconnect(true);
        }



    }
}