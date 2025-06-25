using Microsoft.AspNetCore.Mvc;
using api.Models.DTO;
using api.services;
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
            // 1) Validación: ahora incluimos From
            if (string.IsNullOrWhiteSpace(request.From) ||
                request.To == null || !request.To.Any() ||
                string.IsNullOrWhiteSpace(request.Subject) ||
                string.IsNullOrWhiteSpace(request.Body) ||
                string.IsNullOrWhiteSpace(request.Role))
            {
                return BadRequest(new { success = false, message = "All fields (including From) are required." });
            }

            try
            {
                if (request.Role.Trim().ToLower() == "residente")
                {
                    // Lógica residente (igual que antes)...
                    var userEmail = request.To.First();
                    request.To = new List<string> { "admin@resiapp.com" };
                    request.Cc ??= new List<string>();
                    request.Cc.Add(userEmail);
                }

                // 2) Llamamos al servicio, que ahora usará request.From
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
