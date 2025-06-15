using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Models;
using MyApi.Data;
using api.Models.DTO;

namespace api.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reviews = await _context.Review
                .Include(r => r.Resident)
                .Include(r => r.Space)
                .ToListAsync();

            if (reviews == null || reviews.Count == 0)
                return NotFound("No se encontraron reseñas.");

            var response = reviews.Select(r => new
            {
                r.Id,
                r.Rating,
                r.Comment,
                Resident = new
                {
                    r.Resident.Id,
                    r.Resident.ResidentName,
                    r.Resident.Email,
                    r.Resident.ApartmentInformation
                },
                Space = r.Space == null ? null : new
                {
                    r.Space.Id,
                    r.Space.SpaceName,
                    r.Space.Capacity,
                    r.Space.Availability
                }
            });

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetById(int id)
        {
            var review = await _context.Review.FindAsync(id);
            if (review == null)
                return NotFound();

            return review;
        }
        [HttpGet("residents/{residentId}")]
        public async Task<IActionResult> findReviewsByUser(int residentId)
        {
            var reviews = await _context.Review
                .Include(r => r.Resident)
                .Include(r => r.Space)
                .Where(r => r.ResidentId == residentId)
                .ToListAsync();

            if (reviews == null || reviews.Count == 0)
                return NotFound($"No se encontraron reseñas para el residente con ID {residentId}");

            var response = reviews.Select(r => new
            {
                r.Id,
                r.Rating,
                r.Comment,
                Resident = new
                {
                    r.Resident.Id,
                    r.Resident.ResidentName,
                    r.Resident.Email,
                    r.Resident.ApartmentInformation
                },
                Space = r.Space == null ? null : new
                {
                    r.Space.Id,
                    r.Space.SpaceName,
                    r.Space.Capacity,
                    r.Space.Availability
                }
            });

            return Ok(response);
        }


        [HttpPost]
        public async Task<ActionResult<Review>> CreateReviews([FromBody] Review review)
        {
            _context.Review.Add(review);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = review.Id }, review);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReviews(int id)
        {
            var review = await _context.Review.FindAsync(id);
            if (review == null)
                return NotFound();

            _context.Review.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReviews(int id, [FromBody] UpdateReviewDto dto)
        {
            var existingReview = await _context.Review.FindAsync(id);
            if (existingReview == null)
                return NotFound();

            if (dto.Rating.HasValue)
                existingReview.Rating = dto.Rating.Value;

            if (!string.IsNullOrWhiteSpace(dto.Comment))
                existingReview.Comment = dto.Comment;

            if (dto.ResidentId.HasValue)
                existingReview.ResidentId = dto.ResidentId.Value;

            if (dto.SpaceId.HasValue)
                existingReview.SpaceId = dto.SpaceId.Value;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

