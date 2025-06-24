using api.Models.DTO;
using SendEmail.Models;


namespace SendEmail.Services
{
    public interface IEmailService
    {

        void SendEmail(EmailDTO request);
    }
}