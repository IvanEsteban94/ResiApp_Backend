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
        private readonly ResponseDto _response;

        public ApiController(ApplicationDbContext db)
        {
            _db = db;
            _response = new ResponseDto();
        }

        [HttpGet("GetResident")]
        public IActionResult GetResident()
        {
            try
            {
                var residentList = _db.Resident.ToList();
                if (!residentList.Any())
                    return NotFound("No residents found.");

                _response.success = true;
                _response.data = residentList;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.success = false;
                _response.message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        [HttpPost("CreateResident")]
        public IActionResult CreateResident([FromBody] ResidentCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var resident = new Resident
                {
                    ResidentName = dto.ResidentName,
                    Email = dto.Email,
                    PasswordHash = dto.Password, 
                    ApartmentInformation = dto.ApartmentInformation
                };

                _db.Resident.Add(resident);
                _db.SaveChanges();

                _response.success = true;
                _response.Id = resident.Id;
                _response.message = "Resident created successfully.";
                return CreatedAtAction(nameof(GetResidentById), new { id = resident.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.success = false;
                _response.message = ex.Message;
                return StatusCode(500, _response);
            }
        }
        [HttpGet("GetResident/{id}")]
        public IActionResult GetResidentById(int id)
        {
            try
            {
                var resident = _db.Resident.Find(id);
                if (resident == null)
                    return NotFound("Resident not found.");

                _response.success = true;
                _response.data = new List<Resident> { resident }; // Wrap the single resident in a list
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.success = false;
                _response.message = ex.Message;
                return StatusCode(500, _response);
            }
        }

        // Puedes aplicar los mismos cambios y limpieza al resto de los endpoints.
    }
}
