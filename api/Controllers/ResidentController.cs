using api.Models;
using api.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using MyApi.Data;
using System.Linq;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ApiController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("GetResident")]
        public IActionResult GetResident()
        {
            var residentList = _db.User.Where(u => u.Role == "Resident").ToList();
            if (!residentList.Any())
                return NotFound(new { success = false, message = "No residents found." });

            return Ok(new { success = true, data = residentList });
        }

        [HttpGet("GetResident/{id}")]
        public IActionResult GetResidentById(int id)
        {
            var resident = _db.User.FirstOrDefault(u => u.Id == id && u.Role == "Resident");
            if (resident == null)
                return NotFound(new { success = false, message = "Resident not found." });

            return Ok(new { success = true, data = resident });
        }

        [HttpPost("CreateResident")]
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

        [HttpPut("UpdateResident/{id}")]
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

        [HttpDelete("DeleteResident/{id}")]
        public IActionResult DeleteResident(int id)
        {
            var resident = _db.User.FirstOrDefault(u => u.Id == id && u.Role == "Resident");
            if (resident == null)
                return NotFound(new { success = false, message = "Resident not found." });

            _db.User.Remove(resident);
            _db.SaveChanges();

            return Ok(new { success = true, message = "Resident deleted successfully." });
        }
    }
}
