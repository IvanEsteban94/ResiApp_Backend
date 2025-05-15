using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApi.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;

        public AuthController(AuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginRequest request)
        {
            var defaultRole = "Resident"; // Puedes permitir al usuario enviar el rol si lo deseas
            var token = await _auth.RegisterAsync(request.Email, request.Password, defaultRole);
            if (token == null) return BadRequest("El usuario ya existe.");
            return Ok(new { token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _auth.LoginAsync(request.Email, request.Password);
            if (token == null) return Unauthorized("Credenciales inválidas.");
            return Ok(new { token });
        }

        [HttpGet("protected")]
        [Authorize]
        public IActionResult Protected()
        {
            return Ok(new { message = "Este endpoint está protegido", user = User.Identity?.Name });
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var success = await _auth.ChangePasswordAsync(email, request.CurrentPassword, request.NewPassword);
            if (!success) return BadRequest("La contraseña actual es incorrecta.");

            return Ok(new { message = "Contraseña cambiada exitosamente" });
        }
        [HttpGet("welcome")]
        [Authorize]
        public IActionResult Welcome()
        {
            var email = User.Identity?.Name;
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(role))
                return Unauthorized();

            var message = role switch
            {
                "Admin" => $"Bienvenido, administrador {email}.",
                "Resident" => $"Bienvenido, residente {email}.",
                _ => $"Bienvenido, {email}."
            };

            return Ok(new { message, email, role });
        }


        [HttpGet("validate-token")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            var email = User.Identity?.Name;
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            return Ok(new { valid = true, email, role });
        }

        public record ChangePasswordRequest(
            [Required] string CurrentPassword,
            [Required][MinLength(6)] string NewPassword
        );

        public record LoginRequest(
            [Required][EmailAddress] string Email,
            [Required][MinLength(6)] string Password
        );
    }
}
