﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Models;
using MyApi.Data;
using api.Models.DTO;

namespace api.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> findReviews()
        {
            var reviews = await _context.Review
                .Include(r => r.Resident)
                .Include(r => r.Space)
                .ToListAsync();

            if (reviews == null || reviews.Count == 0)
                return NotFound("No reviews found");

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

        [HttpGet("findReviewsById/{id}")]
        public async Task<ActionResult<Review>> findReviewsById(int id)
        {
            var review = await _context.Review.FindAsync(id);
            if (review == null)
                return NotFound();

            return review;
        }

        [HttpGet("findReviewsByUser/{residentId}")]
        public async Task<IActionResult> findReviewsByUser(int residentId)
        {
            var reviews = await _context.Review
                .Include(r => r.Resident)
                .Include(r => r.Space)
                .Where(r => r.ResidentId == residentId)
                .ToListAsync();

            if (reviews == null || reviews.Count == 0)
                return NotFound($"No reviews found for Resident with ID {residentId}");

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
        public async Task<ActionResult<Review>> CreateReview([FromBody] CreateReviewDto dto)
        {
            if (dto.ResidentId == 0)
                return BadRequest("ResidentId cannot be zero.");

            var residentExists = await _context.User.AnyAsync(u => u.Id == dto.ResidentId);
            if (!residentExists)
                return BadRequest($"No resident with ID found {dto.ResidentId}");

            if (dto.SpaceId == null || dto.SpaceId == 0)
                return BadRequest("SpaceId cannot be null or zero.");

            var spaceExists = await _context.Space.AnyAsync(s => s.Id == dto.SpaceId.Value);
            if (!spaceExists)
                return BadRequest($"No space found with ID {dto.SpaceId.Value}");

            var review = new Review
            {
                Rating = dto.Rating,
                Comment = dto.Comment,
                ResidentId = dto.ResidentId,
                SpaceId = dto.SpaceId.Value
            };

            _context.Review.Add(review);
            await _context.SaveChangesAsync();

            
            return CreatedAtAction(nameof(findReviewsById), new { id = review.Id }, new
            {
                review.Id,
                review.Rating,
                review.Comment,
                review.ResidentId,
                review.SpaceId
            });
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> Reviews(int id)
        {
            var review = await _context.Review.FindAsync(id);
            if (review == null)
                return NotFound();

            _context.Review.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Reviews(int id, [FromBody] UpdateReviewDto dto)
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
