using api.Models;
using api.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using MyApi.Data;
using System.Linq;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET api/user
        [HttpGet("User")]
        public IActionResult GetUsers()
        {
            var users = _db.User.ToList();

            if (!users.Any())
                return NotFound(new { success = false, message = "No users found." });

            return Ok(new { success = true, data = users });
        }

        // GET api/user/5
        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = _db.User.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound(new { success = false, message = "User not found." });

            return Ok(new { success = true, data = user });
        }

        // POST api/user
        [HttpPost("User")]
        public IActionResult CreateUser([FromBody] UserCreateDto dto)
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
                Role = dto.Role
            };

            _db.User.Add(user);
            _db.SaveChanges();

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, new { success = true, id = user.Id, message = "User created successfully." });
        }
        // PUT api/user/5
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UserCreateDto dto)
        {
            var user = _db.User.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound(new { success = false, message = "User not found." });

            // Solo actualiza si el campo tiene valor
            if (!string.IsNullOrWhiteSpace(dto.ResidentName))
                user.ResidentName = dto.ResidentName;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                user.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.ApartmentInformation))
                user.ApartmentInformation = dto.ApartmentInformation;

            if (!string.IsNullOrWhiteSpace(dto.Role))
                user.Role = dto.Role;

            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            _db.User.Update(user);
            _db.SaveChanges();

            return Ok(new
            {
                success = true,
                message = "User updated successfully."
            });
        }



        // DELETE api/user/5
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
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
