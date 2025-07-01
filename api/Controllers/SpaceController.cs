using api.Dtos;
using api.Models;
using api.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;

namespace api.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class SpacesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SpacesController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpaceDto>>> FindSpaces()
        {
            var spaces = await _context.Space
                .Include(s => s.SpaceRule)
                .Select(s => new SpaceDto
                {
                    Id = s.Id,
                    SpaceName = s.SpaceName,
                    Capacity = s.Capacity,
                    Availability = s.Availability,
                    ImageBase64 = s.ImageBase64,
                    SpaceRules = s.SpaceRule != null
                        ? new List<SpaceRule>
                        {
                    new SpaceRule
                    {
                        Id = s.SpaceRule.Id,
                        Rule = s.SpaceRule.Rule
                    }
                        }
                        : new List<SpaceRule>()
                })
                .ToListAsync();

            return Ok(spaces);
        }




        [HttpGet("findSpacesById/{id}")]
        public async Task<ActionResult<SpaceDto>> findSpacesById(int id)
        {
            var space = await _context.Space
                .Include(s => s.SpaceRule)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (space == null) return NotFound();

            var dto = new SpaceDto
            {
                Id = space.Id,
                SpaceName = space.SpaceName,
                Capacity = space.Capacity,
                Availability = space.Availability,
                ImageBase64 = space.ImageBase64,
                SpaceRules = space.SpaceRule != null
                    ? new List<SpaceRule>
                    {
                new SpaceRule
                {
                    Id = space.SpaceRule.Id,
                    Rule = space.SpaceRule.Rule
                }
                    }
                    : new List<SpaceRule>()
            };

            return Ok(dto);
        }



        // POST: api/space
        [HttpPost]
        public async Task<ActionResult<SpaceDto>> Spaces(CreateSpaceDto dto)
        {
            if (dto.SpaceRuleId == null || dto.SpaceRuleId == 0)
            {
                return BadRequest("SpaceRuleId cannot be null or zero.");
            }

            var spaceRuleExists = await _context.SpaceRule.AnyAsync(sr => sr.Id == dto.SpaceRuleId.Value);
            if (!spaceRuleExists)
            {
                return BadRequest($"There is no SpaceRule with Id {dto.SpaceRuleId}");
            }

            var space = new Space
            {
                SpaceName = dto.SpaceName,
                Capacity = dto.Capacity,
                Availability = dto.Availability,
                SpaceRuleId = dto.SpaceRuleId.Value,
                ImageBase64 = dto.ImageBase64 
            };

            _context.Space.Add(space);
            await _context.SaveChangesAsync();

            var resultDto = new SpaceDto
            {
                Id = space.Id,
                SpaceName = space.SpaceName,
                Capacity = space.Capacity,
                Availability = space.Availability,
                ImageBase64 = space.ImageBase64, 
                SpaceRules = new List<SpaceRule>()
            };

            return CreatedAtAction(nameof(findSpacesById), new { id = space.Id }, resultDto);
        }


        // PUT: api/space/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Spaces(int id, CreateSpaceDto dto)
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
                return BadRequest($"There is no SpaceRule with Id {dto.SpaceRuleId}");
            }

            space.SpaceName = dto.SpaceName;
            space.Capacity = dto.Capacity;
            space.Availability = dto.Availability;
            space.SpaceRuleId = dto.SpaceRuleId.Value;
            space.ImageBase64 = dto.ImageBase64; 

            await _context.SaveChangesAsync();

            return NoContent();
        }


        // DELETE: api/space/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Spaces(int id)
        {
            var space = await _context.Space.FindAsync(id);
            if (space == null) return NotFound();

            _context.Space.Remove(space);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
