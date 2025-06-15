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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterResidentRequest request)
        {
            var defaultRole = "Resident";

            var token = await _auth.RegisterAsync(
                request.Email,
                request.Password,
                defaultRole,
                request.ResidentName,
                request.ApartmentInformation
            );

            if (token == null)
                return BadRequest("The user already exists.");

            return Ok(new { token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _auth.LoginAsync(request.Email, request.Password);

            if (response == null)
                return Unauthorized("Invalid credentials.");

            return Ok(new { token = response.Token, role = response.Role, residentId = response.ResidentId, resident = response.Resident });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
                return BadRequest("Email is required.");

            var success = await _auth.ChangePasswordAsync(request.Email, request.CurrentPassword, request.NewPassword);

            if (!success)
                return BadRequest("The current password is incorrect.");

            return Ok(new { message = "Password changed successfully" });
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
