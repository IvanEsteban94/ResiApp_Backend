using api.Models;
using MyApi.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace MyApi.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<string?> RegisterAsync(string email, string password, string role, string residentName, string apartmentInfo)
        {
            if (await UserExists(email)) return null;

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            if (role == "Admin")
            {
                var admin = new Admin { Email = email, PasswordHash = hashedPassword };
                _context.Admin.Add(admin);
            }
            else
            {
                var resident = new Resident
                {
                    Email = email,
                    PasswordHash = hashedPassword,
                    ResidentName = residentName,
                    ApartmentInformation = apartmentInfo
                };
                _context.Resident.Add(resident);
            }

            await _context.SaveChangesAsync();
            return GenerateJwtToken(email, role);
        }


        public async Task<string?> LoginAsync(string email, string password)
        {
            var admin = await _context.Admin.FirstOrDefaultAsync(a => a.Email == email);
            if (admin != null && !string.IsNullOrEmpty(admin.PasswordHash) && BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash))
                return GenerateJwtToken(email, "Admin");

            var resident = await _context.Resident.FirstOrDefaultAsync(r => r.Email == email);
            if (resident != null && !string.IsNullOrEmpty(resident.PasswordHash) && BCrypt.Net.BCrypt.Verify(password, resident.PasswordHash))
                return GenerateJwtToken(email, "Resident");

            return null;
        }


        private async Task<bool> UserExists(string email)
        {
            return await _context.Admin.AnyAsync(a => a.Email == email) ||
                   await _context.Resident.AnyAsync(r => r.Email == email);
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
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<bool> ChangePasswordAsync(string email, string currentPassword, string newPassword)
        {
            var user = await _context.Admin.FirstOrDefaultAsync(a => a.Email == email)
                    as IUser ?? await _context.Resident.FirstOrDefaultAsync(r => r.Email == email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();
            return true;
        }

    }

}
