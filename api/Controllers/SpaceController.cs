using api.Dtos;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpaceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SpaceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Fix for CS0266 and CS8629: Ensure proper handling of nullable int (int?) to int conversion
        // and null checks to avoid runtime issues.

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpaceDto>>> GetAllSpaces()
        {
            var spaces = await _context.Space
                .Select(s => new SpaceDto
                {
                    Id = s.Id,
                    SpaceName = s.SpaceName,
                    Capacity = s.Capacity,
                    Availability = s.Availability,
                    SpaceRuleId = s.SpaceRuleId ?? 0 // Explicitly handle null by providing a default value
                })
                .ToListAsync();

            return Ok(spaces);
        }

        // Método opcional para detectar espacios con SpaceRuleId inválidos (null o 0)
        [HttpGet("invalid-spaces")]
        public async Task<ActionResult<IEnumerable<SpaceDto>>> GetInvalidSpaces()
        {
            var invalidSpaces = await _context.Space
                .Where(s => s.SpaceRuleId == 0 || s.SpaceRuleId == null)
                .Select(s => new SpaceDto
                {
                    Id = s.Id,
                    SpaceName = s.SpaceName,
                    Capacity = s.Capacity,
                    Availability = s.Availability,
                    SpaceRuleId = s.SpaceRuleId ?? 0 // Explicitly handle null by providing a default value
                })
                .ToListAsync();

            if (invalidSpaces.Count == 0)
            {
                return Ok("No hay espacios con SpaceRuleId inválido o nulo.");
            }

            return Ok(invalidSpaces);
        }


        // GET: api/space/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SpaceDto>> GetSpace(int id)
        {
            var space = await _context.Space.FindAsync(id);

            if (space == null) return NotFound();

            if (space.SpaceRuleId == null || space.SpaceRuleId == 0)
            {
                return StatusCode(500, "El registro tiene SpaceRuleId inválido o nulo.");
            }

            var dto = new SpaceDto
            {
                Id = space.Id,
                SpaceName = space.SpaceName,
                Capacity = space.Capacity,
                Availability = space.Availability,
                SpaceRuleId = space.SpaceRuleId.Value // Explicitly access the value of the nullable type
            };

            return Ok(dto);
        }

        // POST: api/space
        [HttpPost]
        public async Task<ActionResult<SpaceDto>> CreateSpace(CreateSpaceDto dto)
        {
            if (dto.SpaceRuleId == null || dto.SpaceRuleId == 0)
            {
                return BadRequest("SpaceRuleId cannot be null or zero.");
            }

            var spaceRuleExists = await _context.SpaceRule.AnyAsync(sr => sr.Id == dto.SpaceRuleId.Value);
            if (!spaceRuleExists)
            {
                return BadRequest($"No existe SpaceRule con Id {dto.SpaceRuleId}");
            }

            var space = new Space
            {
                SpaceName = dto.SpaceName,
                Capacity = dto.Capacity,
                Availability = dto.Availability,
                SpaceRuleId = dto.SpaceRuleId.Value
            };

            _context.Space.Add(space);
            await _context.SaveChangesAsync();

            var resultDto = new SpaceDto
            {
                Id = space.Id,
                SpaceName = space.SpaceName,
                Capacity = space.Capacity,
                Availability = space.Availability,
                SpaceRuleId = space.SpaceRuleId.Value
            };

            return CreatedAtAction(nameof(GetSpace), new { id = space.Id }, resultDto);
        }

        // PUT: api/space/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSpace(int id, CreateSpaceDto dto)
        {
            var space = await _context.Space.FindAsync(id);
            if (space == null) return NotFound();

            if (dto.SpaceRuleId == null || dto.SpaceRuleId == 0)
            {
                return BadRequest("SpaceRuleId cannot be null or zero.");
            }

            var spaceRuleExists = await _context.SpaceRule.AnyAsync(sr => sr.Id == dto.SpaceRuleId.Value);
            if (!spaceRuleExists)
            {
                return BadRequest($"No existe SpaceRule con Id {dto.SpaceRuleId}");
            }

            space.SpaceName = dto.SpaceName;
            space.Capacity = dto.Capacity;
            space.Availability = dto.Availability;
            space.SpaceRuleId = dto.SpaceRuleId.Value;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/space/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpace(int id)
        {
            var space = await _context.Space.FindAsync(id);
            if (space == null) return NotFound();

            _context.Space.Remove(space);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
