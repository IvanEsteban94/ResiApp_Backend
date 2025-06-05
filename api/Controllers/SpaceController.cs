using api.Models;
using api.Models.DTO;
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

        [HttpGet("{id}")]
        public async Task<ActionResult<ReadSpaceDto>> GetSpace(int id)
        {
            var space = await _context.Space
                .Include(s => s.SpaceRules)
                .Include(s => s.SpaceRule)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (space == null)
                return NotFound();

            var result = new ReadSpaceDto
            {
                Id = space.Id,
                SpaceName = space.SpaceName,
                Capacity = space.Capacity,
                Availability = space.Availability,
                SpaceRules = space.SpaceRules?.Select(r => new ReadSpaceRuleDto
                {
                    Id = r.Id,
                    Rule = r.Rule
                }).ToList() ?? new List<ReadSpaceRuleDto>()
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSpace([FromBody] CreateSpaceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var spaceRule = await _context.SpaceRule.FindAsync(dto.SpaceRuleId);
            if (spaceRule == null)
                return NotFound(new { message = "SpaceRule no existe." });

            var space = new api.Models.Space
            {
                SpaceName = dto.SpaceName,
                Capacity = dto.Capacity,
                Availability = dto.Availability,
              
            };

            _context.Space.Add(space);
            await _context.SaveChangesAsync();

            var result = new ReadSpaceDto
            {
                Id = space.Id,
                SpaceName = space.SpaceName,
                Capacity = space.Capacity,
                Availability = space.Availability,
              
            };

            return CreatedAtAction(nameof(GetSpace), new { id = space.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSpace(int id, [FromBody] UpdateSpaceDto dto)
        {
            var space = await _context.Space.FindAsync(id);
            if (space == null)
                return NotFound();

            if (dto.SpaceName != null)
                space.SpaceName = dto.SpaceName;

            if (dto.Capacity.HasValue)
                space.Capacity = dto.Capacity.Value;

            if (dto.Availability.HasValue)
                space.Availability = dto.Availability.Value;

            if (dto.SpaceRuleId.HasValue)
            {
                var spaceRule = await _context.SpaceRule.FindAsync(dto.SpaceRuleId.Value);
                if (spaceRule == null)
                    return NotFound(new { message = "SpaceRule no existe." });

                space.SpaceRuleId = dto.SpaceRuleId.Value;
            }

            try
            {
                _context.Entry(space).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar en la base de datos.", detail = ex.Message });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpace(int id)
        {
            var space = await _context.Space.FindAsync(id);
            if (space == null)
                return NotFound();

            try
            {
                _context.Space.Remove(space);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar en la base de datos.", detail = ex.Message });
            }

            return Ok(new { success = true, message = "Space eliminado correctamente." });
        }
    }
}
