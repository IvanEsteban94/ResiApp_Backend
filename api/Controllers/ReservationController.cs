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

        // GET api/reservation/user/5  => reservas por usuario
        [HttpGet("user/{residentId}")]
        public async Task<IActionResult> ReservationsByUser(int residentId)
        {
            var userReservations = await _dbContext.Reservation
                .Where(r => r.ResidentId == residentId)
                .Include(r => r.Resident)
                .Include(r => r.Space)
                    .ThenInclude(s => s.SpaceRule)
                .ToListAsync();

            if (!userReservations.Any())
            {
                return NotFound(new { success = false, message = "No reservations found for the specified user." });
            }

            var response = userReservations.Select(reservation => new
            {
                reservation.Id,
                reservation.StartTime,
                reservation.EndTime,
                Resident = new
                {
                    reservation.Resident.Id,
                    reservation.Resident.Email,
                    reservation.Resident.Role,
                    reservation.Resident.ResidentName,
                    reservation.Resident.ApartmentInformation
                },
                Space = new
                {
                    reservation.Space.Id,
                    reservation.Space.SpaceName,
                    reservation.Space.Capacity,
                    reservation.Space.Availability,
                    Rule = reservation.Space.SpaceRule == null ? null : new
                    {
                        reservation.Space.SpaceRule.Id,
                        reservation.Space.SpaceRule.Rule
                    }
                }
            });

            return Ok(new { success = true, data = response });
        }

        // GET api/reservation  => todas las reservas
        [HttpGet]
        public async Task<IActionResult> AllReservations()
        {
            var reservations = await _dbContext.Reservation
                .Include(r => r.Resident)
                .Include(r => r.Space)
                    .ThenInclude(s => s.SpaceRule)
                .ToListAsync();

            var response = reservations.Select(reservation => new
            {
                reservation.Id,
                reservation.StartTime,
                reservation.EndTime,
                Resident = new
                {
                    reservation.Resident.Id,
                    reservation.Resident.Email,
                    reservation.Resident.Role,
                    reservation.Resident.ResidentName,
                    reservation.Resident.ApartmentInformation
                },
                Space = new
                {
                    reservation.Space.Id,
                    reservation.Space.SpaceName,
                    reservation.Space.Capacity,
                    reservation.Space.Availability,
                    Rule = reservation.Space.SpaceRule == null ? null : new
                    {
                        reservation.Space.SpaceRule.Id,
                        reservation.Space.SpaceRule.Rule
                    }
                }
            });

            return Ok(new { success = true, data = response });
        }

        // GET api/reservation/5  => reserva por id
        [HttpGet("{id}")]
        public async Task<IActionResult> ReservationById(int id)
        {
            var reservation = await _dbContext.Reservation
                .Include(r => r.Resident) // Incluye el Resident
                .Include(r => r.Space)
                    .ThenInclude(s => s.SpaceRule)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return NotFound(new { success = false, message = "Reservation not found." });

            var response = new
            {
                reservation.Id,
                reservation.StartTime,
                reservation.EndTime,
                Resident = new
                {
                    reservation.Resident.Id,
                    reservation.Resident.Email,
                    reservation.Resident.Role,
                    reservation.Resident.ResidentName,
                    reservation.Resident.ApartmentInformation
                },
                Space = new
                {
                    reservation.Space.Id,
                    reservation.Space.SpaceName,
                    reservation.Space.Capacity,
                    reservation.Space.Availability,
                    Rule = reservation.Space.SpaceRule == null ? null : new
                    {
                        reservation.Space.SpaceRule.Id,
                        reservation.Space.SpaceRule.Rule
                    }
                }
            };

            return Ok(new { success = true, data = response });
        }

        // POST api/reservation  => crear reserva
        [HttpPost]
        public async Task<IActionResult> Reservation([FromBody] ReservationDto reservationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (reservationDto.EndTime <= reservationDto.StartTime)
                return BadRequest(new { success = false, message = "EndTime must be after StartTime." });

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

            return CreatedAtAction(nameof(ReservationById), new { id = reservation.Id }, new { success = true, message = "Reservation created successfully." });
        }

        // PUT api/reservation/5  => actualizar reserva
        [HttpPut("{id}")]
        public async Task<IActionResult> Reservation(int id, [FromBody] Reservation updatedReservation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (updatedReservation.EndTime <= updatedReservation.StartTime)
                return BadRequest(new { success = false, message = "EndTime must be after StartTime." });

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

        // DELETE api/reservation/5  => eliminar reserva (solo admin)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reservation(int id)
        {
            var reservation = await _dbContext.Reservation.FindAsync(id);
            if (reservation == null)
                return NotFound(new { success = false, message = "Reservation not found." });

            _dbContext.Reservation.Remove(reservation);
            await _dbContext.SaveChangesAsync();

            return Ok(new { success = true, message = "Reservation deleted successfully." });
        }

        // GET api/reservation/available-slots?spaceId=1&date=2025-06-15&residentId=2&slotMinutes=60
        [HttpGet("available-slots")]
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
                        StartTime = currentStart.ToString("yyyy-MM-ddTHH:mm:ss"),
                        EndTime = currentEnd.ToString("yyyy-MM-ddTHH:mm:ss")
                    });
                }

                currentStart = currentStart.AddMinutes(slotMinutes);
            }

            return Ok(new { success = true, data = availableSlots });
        }
    }
}
