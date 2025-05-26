using api.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApi.Models.DTO;
using MyApi.Services;
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

            return Ok(new { token = response.Token, role = response.Role });
        }

     
        [HttpPost("change-password")]
       
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var success = await _auth.ChangePasswordAsync(email, request.CurrentPassword, request.NewPassword);

            if (!success)
                return BadRequest("The current password is incorrect.");

            return Ok(new { message = "Password changed successfully" });
        }

        

        
    }
}
