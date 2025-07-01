using Microsoft.AspNetCore.Mvc;
using SendEmail.Services;
using SendEmail.Models;

namespace api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;

        public EmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send-email")]
        public IActionResult SendEmail([FromBody] EmailDTO request)
        {
            if (request.To == null || !request.To.Any() ||
                string.IsNullOrWhiteSpace(request.Subject) ||
                string.IsNullOrWhiteSpace(request.Body) ||
                string.IsNullOrWhiteSpace(request.Role))
            {
                return BadRequest(new { success = false, message = "To, Subject, Body and Role are required." });
            }

            try
            {
                if (request.Role.Trim().ToLower() == "residente")
                {
                    var userEmail = request.To.First();
                    request.To = new List<string> { "notificationsresiapp@gmail.com" };
                    request.Cc ??= new List<string>();
                    request.Cc.Add(userEmail);
                }

                _emailService.SendEmail(request);

                return Ok(new { success = true, message = "Email sent successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Failed to send email.",
                    error = ex.Message
                });
            }
        }


    }
}
