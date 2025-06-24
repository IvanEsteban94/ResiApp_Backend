using Microsoft.AspNetCore.Mvc;
using api.Models.DTO;
using api.services;
using SendEmail.Services;
using SendEmail.Models;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            if (string.IsNullOrWhiteSpace(request.To) ||
                string.IsNullOrWhiteSpace(request.Subject) ||
                string.IsNullOrWhiteSpace(request.Body) ||
                string.IsNullOrWhiteSpace(request.Role))
            {
                return BadRequest(new { success = false, message = "All fields are required." });
            }

            try
            {
                if (request.Role.Trim().ToLower() == "residente")
                {
                    // Reemplazamos To por admin y usamos el original como CC
                    string userEmail = request.To;
                    request.To = "admin@resiapp.com"; // Cambia por el correo real del admin
                    request.Cc = userEmail;
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
