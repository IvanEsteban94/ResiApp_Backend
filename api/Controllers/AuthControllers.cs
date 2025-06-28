using api.Interface;
using api.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using MyApi.Models.DTO;
using MyApi.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly AuthService _auth;
        private readonly IConfiguration _config;
        public AuthController(AuthService auth, IConfiguration config)
        {
            _auth = auth;
            _config = config;
        }


        [HttpGet("validate-token")]
        public async Task<IActionResult> ValidateToken(
          [FromHeader(Name = "Authorization")] string authorizationHeader,
          [FromServices] ITokenBlacklistService blacklistService)
        {
            IdentityModelEventSource.ShowPII = true;

            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new { valid = false, message = "No token provided or invalid format." });
            }

            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            // 🔒 Verificamos si el token fue revocado
            if (await blacklistService.IsTokenRevokedAsync(token))
            {
                return Unauthorized(new { valid = false, message = "Token has been revoked." });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    ValidateIssuer = true,
                    ValidIssuer = _config["Jwt:Issuer"],

                    ValidateAudience = true,
                    ValidAudience = _config["Jwt:Audience"],

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                var email = jwtToken.Claims.FirstOrDefault(c =>
                    c.Type == ClaimTypes.Email || c.Type == "email" ||
                    c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;

                if (string.IsNullOrEmpty(email))
                    return Unauthorized(new { valid = false, message = "Email claim not found." });

                return Ok(new { valid = true, message = "Token is valid.", email });
            }
            catch (SecurityTokenExpiredException)
            {
                return Unauthorized(new { valid = false, message = "Token expired." });
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(new { valid = false, message = $"Token invalid: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { valid = false, message = $"An error occurred: {ex.Message}" });
            }
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
        public async Task<IActionResult> Logout(
       [FromHeader(Name = "Authorization")] string authorizationHeader,
       [FromServices] ITokenBlacklistService blacklistService)
        {
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
                return BadRequest(new { message = "Invalid token format." });

            var token = authorizationHeader["Bearer ".Length..].Trim();
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            await blacklistService.AddAsync(token, jwt.ValidTo);

            return Ok(new { message = "Token revoked successfully." });
        }



    }
}
