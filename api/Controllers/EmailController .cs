using api.Models.DTO;
using api.services;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.To) ||
            string.IsNullOrWhiteSpace(request.Subject) ||
            string.IsNullOrWhiteSpace(request.Body))
        {
            return BadRequest("All fields are required.");
        }

        try
        {
            await _emailService.SendEmailAsync(request.To, request.Subject, request.Body);
            return Ok(new { success = true, message = "Email sent successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Failed to send email.", error = ex.Message });
        }
    }
}
