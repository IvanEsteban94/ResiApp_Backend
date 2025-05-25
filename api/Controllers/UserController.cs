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

        // GET api/user/residents
        [HttpGet("residents")]
        public IActionResult GetResidents()
        {
            var residents = _db.User.Where(u => u.Role == "Resident").ToList();
            if (!residents.Any())
                return NotFound(new { success = false, message = "No residents found." });

            return Ok(new { success = true, data = residents });
        }

        // GET api/user/admins
        [HttpGet("admins")]
        public IActionResult GetAdmins()
        {
            var admins = _db.User.Where(u => u.Role == "Admin").ToList();
            if (!admins.Any())
                return NotFound(new { success = false, message = "No admins found." });

            return Ok(new { success = true, data = admins });
        }

        // GET api/user/residents/5
        [HttpGet("residents/{id}")]
        public IActionResult GetResidentById(int id)
        {
            var resident = _db.User.FirstOrDefault(u => u.Id == id && u.Role == "Resident");
            if (resident == null)
                return NotFound(new { success = false, message = "Resident not found." });

            return Ok(new { success = true, data = resident });
        }

        // GET api/user/admins/5
        [HttpGet("admins/{id}")]
        public IActionResult GetAdminById(int id)
        {
            var admin = _db.User.FirstOrDefault(u => u.Id == id && u.Role == "Admin");
            if (admin == null)
                return NotFound(new { success = false, message = "Admin not found." });

            return Ok(new { success = true, data = admin });
        }

        // POST api/user/residents
        [HttpPost("residents")]
        public IActionResult CreateResident([FromBody] UserCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_db.User.Any(u => u.Email == dto.Email))
                return Conflict(new { success = false, message = "Email already exists." });

            var resident = new User
            {
                ResidentName = dto.ResidentName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                ApartmentInformation = dto.ApartmentInformation,
                Role = "Resident"
            };

            _db.User.Add(resident);
            _db.SaveChanges();

            return CreatedAtAction(nameof(GetResidentById), new { id = resident.Id }, new { success = true, id = resident.Id, message = "Resident created successfully." });
        }

        // POST api/user/admins
        [HttpPost("admins")]
        public IActionResult CreateAdmin([FromBody] UserCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_db.User.Any(u => u.Email == dto.Email))
                return Conflict(new { success = false, message = "Email already exists." });

            var admin = new User
            {
                ResidentName = dto.ResidentName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                ApartmentInformation = dto.ApartmentInformation,
                Role = "Admin"
            };

            _db.User.Add(admin);
            _db.SaveChanges();

            return CreatedAtAction(nameof(GetAdminById), new { id = admin.Id }, new { success = true, id = admin.Id, message = "Admin created successfully." });
        }

        // PUT api/user/residents/5
        [HttpPut("residents/{id}")]
        public IActionResult UpdateResident(int id, [FromBody] UserCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resident = _db.User.FirstOrDefault(u => u.Id == id && u.Role == "Resident");
            if (resident == null)
                return NotFound(new { success = false, message = "Resident not found." });

            resident.ResidentName = dto.ResidentName;
            resident.Email = dto.Email;
            resident.ApartmentInformation = dto.ApartmentInformation;

            if (!string.IsNullOrEmpty(dto.Password))
            {
                resident.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            _db.User.Update(resident);
            _db.SaveChanges();

            return Ok(new { success = true, message = "Resident updated successfully." });
        }

        // PUT api/user/admins/5
        [HttpPut("admins/{id}")]
        public IActionResult UpdateAdmin(int id, [FromBody] UserCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var admin = _db.User.FirstOrDefault(u => u.Id == id && u.Role == "Admin");
            if (admin == null)
                return NotFound(new { success = false, message = "Admin not found." });

            admin.ResidentName = dto.ResidentName;
            admin.Email = dto.Email;
            admin.ApartmentInformation = dto.ApartmentInformation;

            if (!string.IsNullOrEmpty(dto.Password))
            {
                admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            _db.User.Update(admin);
            _db.SaveChanges();

            return Ok(new { success = true, message = "Admin updated successfully." });
        }

        // DELETE api/user/residents/5
        [HttpDelete("residents/{id}")]
        public IActionResult DeleteResident(int id)
        {
            var resident = _db.User.FirstOrDefault(u => u.Id == id && u.Role == "Resident");
            if (resident == null)
                return NotFound(new { success = false, message = "Resident not found." });

            _db.User.Remove(resident);
            _db.SaveChanges();

            return Ok(new { success = true, message = "Resident deleted successfully." });
        }

        // DELETE api/user/admins/5
        [HttpDelete("admins/{id}")]
        public IActionResult DeleteAdmin(int id)
        {
            var admin = _db.User.FirstOrDefault(u => u.Id == id && u.Role == "Admin");
            if (admin == null)
                return NotFound(new { success = false, message = "Admin not found." });

            _db.User.Remove(admin);
            _db.SaveChanges();

            return Ok(new { success = true, message = "Admin deleted successfully." });
        }
    }
}

