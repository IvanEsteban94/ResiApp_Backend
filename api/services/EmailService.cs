
using SendEmail.Models;
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

    mailMessage.From = new MailAddress("notificationsresiapp@gmail.com", "ResiApp Notifications");


    foreach (var to in request.To)
        mailMessage.To.Add(to);


    if (request.Cc != null)
    {
        foreach (var cc in request.Cc)
            mailMessage.CC.Add(cc);
    }


    mailMessage.Subject = request.Subject;
    mailMessage.Body = request.Body;
    mailMessage.IsBodyHtml = true;


    using var smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587)
    {
        Credentials = new NetworkCredential("notificationsresiapp@gmail.com", "pbxwqbcxtxdpelid"), 
        EnableSsl = true
    };

    smtp.Send(mailMessage);
}





    }
}