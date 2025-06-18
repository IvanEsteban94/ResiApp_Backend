using api.Models;
using api.Models.DTO;
using MyApi.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using api.Interface;
using api.Utilidy; // Asegúrate de que la interfaz esté en este namespace

namespace MyApi.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly ITokenBlacklistService _blacklist;

        public AuthService(ApplicationDbContext context, IConfiguration config, ITokenBlacklistService blacklist)
        {
            _context = context;
            _config = config;
            _blacklist = blacklist;
        }

        public async Task<string?> RegisterAsync(string email, string password, string role, string? residentName = null, string? apartmentInfo = null)
        {
            if (await UserExists(email)) return null;

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Email = email,
                PasswordHash = hashedPassword,
                Role = role,
                ResidentName = role == "Resident" ? residentName : null,
                ApartmentInformation = role == "Resident" ? apartmentInfo : null
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return GenerateJwtToken(email, role);
        }

        public async Task<LoginResponse?> LoginAsync(string email, string password)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            var token = GenerateJwtToken(user.Email, user.Role);

            return new LoginResponse
            {
                Token = token,
                Role = user.Role,
                ResidentId = user.Id,
                Resident = user.ResidentName
            };
        }
        public async Task<bool> ChangePasswordAsync(string email, string currentPassword, string newPassword, string securityWord)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return false;

            if (!PasswordUtils.VerifySecurityWord(securityWord, user.SecurityWord))
                return false;

           
            if (!PasswordUtils.VerifyPassword(currentPassword, user.PasswordHash))
                return false;

           
            user.PasswordHash = PasswordUtils.Hash(newPassword);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task LogoutAsync(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var expires = jwtToken.ValidTo;

            await _blacklist.AddAsync(token, expires);
        }

        public async Task<bool> IsTokenRevokedAsync(string token)
        {
            return await _blacklist.IsTokenRevokedAsync(token);
        }

        private async Task<bool> UserExists(string email)
        {
            return await _context.User.AnyAsync(u => u.Email == email);
        }
      

        private string GenerateJwtToken(string email, string role)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.Name, email),
        new Claim(ClaimTypes.Role, role)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15), 
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
