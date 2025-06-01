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

        // POST api/Space
        [HttpPost]
        public async Task<IActionResult> CreateSpace([FromBody] SpaceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var space = new Models.Space
            {
                SpaceName = dto.SpaceName,
                Capacity = dto.Capacity,
                Availability = dto.Availability
            };

            _context.Space.Add(space);
            await _context.SaveChangesAsync();

            var result = new ReadSpaceDto
            {
                Id = space.Id,
                SpaceName = space.SpaceName,
                Capacity = space.Capacity,
                Availability = space.Availability,
                SpaceRules = new List<ReadSpaceRuleDto>()
            };

            return CreatedAtAction(nameof(ReadSpace), new { id = space.Id }, result);
        }

        // POST api/Space/rules
        [HttpPost("rules")]
        public async Task<IActionResult> CreateSpaceRule([FromBody] CreateSpaceRuleDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var space = await _context.Space.FindAsync(dto.SpaceId);
            if (space == null)
                return NotFound(new { message = "El espacio no existe." });

            var rule = new SpaceRule
            {
                Rule = dto.Rule,
                SpaceId = dto.SpaceId,
                Space = space
            };

            _context.SpaceRule.Add(rule);
            await _context.SaveChangesAsync();

            var result = new ReadSpaceRuleDto
            {
                Id = rule.Id,
                Rule = rule.Rule
            };

            return CreatedAtAction(nameof(ReadSpace), new { id = rule.Id }, result);
        }

        // GET api/Space/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ReadSpaceDto>> ReadSpace(int id)
        {
            var space = await _context.Space
                .Include(s => s.SpaceRules)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (space == null)
                return NotFound();

            var result = new ReadSpaceDto
            {
                Id = space.Id,
                SpaceName = space.SpaceName,
                Capacity = space.Capacity,
                Availability = space.Availability,
                SpaceRules = space.SpaceRules.Select(r => new ReadSpaceRuleDto
                {
                    Id = r.Id,
                    Rule = r.Rule
                }).ToList()
            };

            return Ok(result);
        }

        // PUT api/Space/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSpace(int id, [FromBody] SpaceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var space = await _context.Space.FindAsync(id);
            if (space == null)
                return NotFound();

            space.SpaceName = dto.SpaceName;
            space.Capacity = dto.Capacity;
            space.Availability = dto.Availability;

            _context.Entry(space).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/Space/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpace(int id)
        {
            var space = await _context.Space.FindAsync(id);
            if (space == null)
                return NotFound(new { success = false, message = "Space not found." });

            _context.Space.Remove(space);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Space deleted successfully." });
        }

        // GET api/Space
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadSpaceDto>>> GetAllSpaces()
        {
            var spaces = await _context.Space
                .Include(s => s.SpaceRules)
                .ToListAsync();

            var result = spaces.Select(space => new ReadSpaceDto
            {
                Id = space.Id,
                SpaceName = space.SpaceName,
                Capacity = space.Capacity,
                Availability = space.Availability,
                SpaceRules = space.SpaceRules.Select(r => new ReadSpaceRuleDto
                {
                    Id = r.Id,
                    Rule = r.Rule
                }).ToList()
            });

            return Ok(result);
        }
    }
}
