using api.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApi.Models.DTO;
using MyApi.Services;
using System.Security.Claims;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;

        public AuthController(AuthService auth)
        {
            _auth = auth;
        }
        [Authorize]
        [HttpGet("validate-token")]
        public IActionResult ValidateToken()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(email))
                return Unauthorized(new { valid = false, message = "Invalid token." });

            return Ok(new { valid = true, message = "Token is valid.", email });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterResidentRequest request)
        {
            var defaultRole = "Resident";

            var token = await _auth.RegisterAsync(
                request.Email,
                request.Password,
                defaultRole,
                request.ResidentName,
                request.ApartmentInformation,
                request.SecurityWord  // ✅ Agregado aquí
            );

            if (token == null)
                return BadRequest("The user already exists.");

            return Ok(new
            {
                token,
                role = defaultRole,
                residentName = request.ResidentName
            });
        }

    
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Obtener token anterior del header Authorization (si existe)
            var authHeader = Request.Headers["Authorization"].ToString();
            string? oldToken = null;
            bool? oldTokenValid = null;

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                oldToken = authHeader.Replace("Bearer ", "");
                oldTokenValid = !await _auth.IsTokenRevokedAsync(oldToken);
            }

            // Proceso normal de login
            var response = await _auth.LoginAsync(request.Email, request.Password);
            if (response == null)
                return Unauthorized("Invalid credentials.");

            return Ok(new
            {
                token = response.Token,
                role = response.Role,
                residentId = response.ResidentId,
                resident = response.Resident,
                previousTokenValid = oldTokenValid  
            });
        }


        [HttpPost("FORGOT-PASSWORD")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.NewPassword) ||
                string.IsNullOrWhiteSpace(request.SecurityWord))
            {
                return BadRequest("All fields are required.");
            }

            var success = await _auth.ChangePasswordAsync(
                request.Email,
                request.NewPassword,
                request.SecurityWord);

            if (!success)
                return BadRequest("Current password or security word is incorrect.");

            return Ok(new { message = "Password changed successfully." });
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var authHeader = Request.Headers["Authorization"].ToString();

            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Ok(new { message = "No token provided, logout considered successful." });
            }

            var token = authHeader.Substring("Bearer ".Length);
            await _auth.LogoutAsync(token);

            return Ok(new { message = "Logged out successfully" });
        }
    }
}
