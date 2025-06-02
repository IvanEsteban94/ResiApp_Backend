using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Models;
using MyApi.Data;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetAll()
        {
            return await _context.Review.ToListAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetById(int id)
        {
            var review = await _context.Review.FindAsync(id);
            if (review == null)
                return NotFound();

            return review;
        }

 
        [HttpPost]
        public async Task<ActionResult<Review>> CreateReview([FromBody] Review review)
        {
            _context.Review.Add(review);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = review.Id }, review);
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] Review updatedReview)
        {
            if (id != updatedReview.Id)
                return BadRequest();

            var existingReview = await _context.Review.FindAsync(id);
            if (existingReview == null)
                return NotFound();

            existingReview.Rating = updatedReview.Rating;
            existingReview.Comment = updatedReview.Comment;
            existingReview.ResidentId = updatedReview.ResidentId;
            existingReview.SpaceId = updatedReview.SpaceId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

       
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Review.FindAsync(id);
            if (review == null)
                return NotFound();

            _context.Review.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
