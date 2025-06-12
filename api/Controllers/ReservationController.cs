using api.Models;
using api.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public ReservationController(ApplicationDbContext db)
        {
            _dbContext = db;
        }

        [HttpGet("GetReservation/{id}")]
        public async Task<IActionResult> GetReservationById(int id)
        {
            var reservation = await _dbContext.Reservation.FindAsync(id);
            if (reservation == null)
                return NotFound(new { success = false, message = "Reservation not found." });

            return Ok(new { success = true, data = reservation });
        }

        [HttpPost("CreateReservation")]
        public async Task<IActionResult> CreateReservation([FromBody] ReservationDto reservationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var space = await _dbContext.Space.FirstOrDefaultAsync(s => s.Id == reservationDto.SpaceId);
            if (space == null)
                return NotFound(new { success = false, message = "Space not found." });

            var overlappingReservationsCount = await _dbContext.Reservation
                .Where(r => r.SpaceId == reservationDto.SpaceId &&
                            ((reservationDto.StartTime >= r.StartTime && reservationDto.StartTime < r.EndTime) ||
                             (reservationDto.EndTime > r.StartTime && reservationDto.EndTime <= r.EndTime) ||
                             (reservationDto.StartTime <= r.StartTime && reservationDto.EndTime >= r.EndTime)))
                .CountAsync();

            if (overlappingReservationsCount >= space.Capacity)
                return BadRequest(new { success = false, message = "No available capacity for the selected time slot." });

            var reservation = new Reservation
            {
                StartTime = reservationDto.StartTime,
                EndTime = reservationDto.EndTime,
                ResidentId = reservationDto.ResidentId,
                SpaceId = reservationDto.SpaceId
            };

            await _dbContext.Reservation.AddAsync(reservation);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReservationById), new { id = reservation.Id }, new { success = true, message = "Reservation created successfully." });
        }

        [HttpPut("UpdateReservation/{id}")]
        public async Task<IActionResult> UpdateReservation(int id, [FromBody] Reservation updatedReservation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reservation = await _dbContext.Reservation.FirstOrDefaultAsync(r => r.Id == id);
            if (reservation == null)
                return NotFound(new { success = false, message = "Reservation not found." });

            var space = await _dbContext.Space.FirstOrDefaultAsync(s => s.Id == updatedReservation.SpaceId);
            if (space == null)
                return NotFound(new { success = false, message = "Target space not found." });

            var overlappingCount = await _dbContext.Reservation
                .Where(r => r.SpaceId == updatedReservation.SpaceId && r.Id != id &&
                            ((updatedReservation.StartTime >= r.StartTime && updatedReservation.StartTime < r.EndTime) ||
                             (updatedReservation.EndTime > r.StartTime && updatedReservation.EndTime <= r.EndTime) ||
                             (updatedReservation.StartTime <= r.StartTime && updatedReservation.EndTime >= r.EndTime)))
                .CountAsync();

            if (overlappingCount >= space.Capacity)
                return BadRequest(new { success = false, message = "No available capacity for the selected time slot." });

            reservation.StartTime = updatedReservation.StartTime;
            reservation.EndTime = updatedReservation.EndTime;
            reservation.ResidentId = updatedReservation.ResidentId;
            reservation.SpaceId = updatedReservation.SpaceId;

            _dbContext.Reservation.Update(reservation);
            await _dbContext.SaveChangesAsync();

            return Ok(new { success = true, message = "Reservation updated successfully." });
        }

        [HttpDelete("DeleteReservation/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _dbContext.Reservation.FindAsync(id);
            if (reservation == null)
                return NotFound(new { success = false, message = "Reservation not found." });

            _dbContext.Reservation.Remove(reservation);
            await _dbContext.SaveChangesAsync();

            return Ok(new { success = true, message = "Reservation deleted successfully." });
        }

        [HttpGet("GetAvailableSlots")]
        public async Task<IActionResult> GetAvailableSlots([FromQuery] int spaceId, [FromQuery] DateTime date, [FromQuery] int residentId, [FromQuery] int slotMinutes = 60)
        {
            var space = await _dbContext.Space.FirstOrDefaultAsync(s => s.Id == spaceId);
            if (space == null)
                return NotFound(new { success = false, message = "Space not found." });

            if (!space.Availability)
                return BadRequest(new { success = false, message = "Space is currently unavailable." });

            var startOfDay = date.Date.AddHours(8);
            var endOfDay = date.Date.AddHours(20);

            var reservations = await _dbContext.Reservation
                .Where(r => r.SpaceId == spaceId && r.StartTime.Date == date.Date)
                .ToListAsync();

            var availableSlots = new List<object>();
            var currentStart = startOfDay;

            while (currentStart.AddMinutes(slotMinutes) <= endOfDay)
            {
                var currentEnd = currentStart.AddMinutes(slotMinutes);

                var overlappingCount = reservations
                    .Count(r => currentStart < r.EndTime && currentEnd > r.StartTime);

                var residentHasReservation = reservations
                    .Any(r => r.ResidentId == residentId &&
                              currentStart < r.EndTime && currentEnd > r.StartTime);

                if (overlappingCount < space.Capacity && !residentHasReservation)
                {
                    availableSlots.Add(new
                    {
                        SpaceName = space.SpaceName,
                        TimeRange = $"{currentStart:hh\\:mm tt} to {currentEnd:hh\\:mm tt}"
                    });
                }

                currentStart = currentEnd;
            }

            return Ok(new { success = true, data = availableSlots });
        }
    }
}
