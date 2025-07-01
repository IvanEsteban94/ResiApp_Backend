
using api.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using MyApi.Data;


namespace api.Controllers
{
    [Route("/api/v1/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public UsersController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult findUser()
        {
            var users = _db.User.ToList();

            if (!users.Any())
                return NotFound(new { success = false, message = "No users found." });

            return Ok(new { success = true, data = users });
        }

        [HttpGet("findUserById/{id}")]
        public IActionResult findUserById(int id)
        {
            var user = _db.User.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound(new { success = false, message = "User not found." });

            return Ok(new { success = true, data = user });
        }

       
        [HttpPost]
        public IActionResult User([FromBody] UserCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_db.User.Any(u => u.Email == dto.Email))
                return Conflict(new { success = false, message = "Email already exists." });

            var user = new User
            {
                ResidentName = dto.ResidentName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                ApartmentInformation = dto.ApartmentInformation,
                Role = dto.Role,
                ImageBase64 = dto.ImageBase64 
            };

            _db.User.Add(user);
            _db.SaveChanges();

            return CreatedAtAction(nameof(findUserById), new { id = user.Id }, new
            {
                success = true,
                id = user.Id,
                message = "User created successfully."
            });
        }

        [HttpPut("{id}")]
        public IActionResult User(int id, [FromBody] UserCreateDto dto)
        {
            var user = _db.User.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound(new { success = false, message = "User not found." });

            if (!string.IsNullOrWhiteSpace(dto.ResidentName))
                user.ResidentName = dto.ResidentName;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                user.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.ApartmentInformation))
                user.ApartmentInformation = dto.ApartmentInformation;

            if (!string.IsNullOrWhiteSpace(dto.Role))
                user.Role = dto.Role;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                if (dto.Password.Length < 6)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Password must be at least 6 characters long."
                    });
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }


            if (!string.IsNullOrWhiteSpace(dto.ImageBase64))
            {
                if (!dto.ImageBase64.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Image must be in base64 format and include a valid data URI prefix (e.g., data:image/png;base64,...)"
                    });
                }

                user.ImageBase64 = dto.ImageBase64;
            }

            _db.User.Update(user);
            _db.SaveChanges();

            return Ok(new
            {
                success = true,
                message = "User updated successfully."
            });
        }




        [HttpDelete("{id}")]
        public IActionResult User(int id)
        {
            var user = _db.User.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound(new { success = false, message = "User not found." });

            _db.User.Remove(user);
            _db.SaveChanges();

            return Ok(new { success = true, message = "User deleted successfully." });
        }
    }
}
