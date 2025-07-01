using api.Models;
using api.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;

namespace api.Controllers
{
    [Route("/api/v1/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public ReservationsController(ApplicationDbContext db)
        {
            _dbContext = db;
        }

    
        [HttpGet("findReservationsByUser/{residentId}")]
        public async Task<IActionResult> findReservationsByUser(int residentId)
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

      
        [HttpGet]
        public async Task<IActionResult> findReservations()
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
                    reservation.Space.ImageBase64, 
                    Rule = reservation.Space.SpaceRule == null ? null : new
                    {
                        reservation.Space.SpaceRule.Id,
                        reservation.Space.SpaceRule.Rule
                    }
                }
            });

            return Ok(new { success = true, data = response });
        }


      
        [HttpGet("findReservationsById/{id}")]
        public async Task<IActionResult> findReservationsById(int id)
        {
            var reservation = await _dbContext.Reservation
                .Include(r => r.Resident)
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
                    reservation.Space.ImageBase64,
                    Rule = reservation.Space.SpaceRule == null ? null : new
                    {
                        reservation.Space.SpaceRule.Id,
                        reservation.Space.SpaceRule.Rule
                    }
                }
            };

            return Ok(new { success = true, data = response });
        }


        [HttpPost]
        public async Task<IActionResult> Reservations([FromBody] ReservationDto reservationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (reservationDto.EndTime <= reservationDto.StartTime)
                return BadRequest(new { success = false, message = "EndTime must be after StartTime." });

            var space = await _dbContext.Space.FirstOrDefaultAsync(s => s.Id == reservationDto.SpaceId);
            if (space == null)
                return NotFound(new { success = false, message = "Space not found." });

            if (space.Capacity <= 0)
                return BadRequest(new { success = false, message = "No remaining capacity for this space." });

         
            var reservationDate = reservationDto.StartTime.Date;
            var startOfDay = reservationDate.AddHours(0);
            var endOfDay = reservationDate.AddDays(1).AddSeconds(-1);

            var dailyReservationCount = await _dbContext.Reservation
                .Where(r =>
                    r.ResidentId == reservationDto.ResidentId &&
                    r.SpaceId == reservationDto.SpaceId &&
                    r.StartTime >= startOfDay &&
                    r.StartTime <= endOfDay)
                .CountAsync();

            if (dailyReservationCount >= 5)
                return BadRequest(new { success = false, message = "You have reached the daily limit of 5 reservations for this space." });

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

            space.Capacity -= 1;
            _dbContext.Space.Update(space);

            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(findReservationsById), new { id = reservation.Id }, new { success = true, message = "Reservation created successfully." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Reservations(int id, [FromBody] UpdateReservationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.EndTime <= dto.StartTime)
                return BadRequest(new { success = false, message = "EndTime must be after StartTime." });

            var reservation = await _dbContext.Reservation.FirstOrDefaultAsync(r => r.Id == id);
            if (reservation == null)
                return NotFound(new { success = false, message = "Reservation not found." });

            var space = await _dbContext.Space.FirstOrDefaultAsync(s => s.Id == dto.SpaceId);
            if (space == null)
                return NotFound(new { success = false, message = "Target space not found." });

            var overlappingCount = await _dbContext.Reservation
                .Where(r => r.SpaceId == dto.SpaceId && r.Id != id &&
                            ((dto.StartTime >= r.StartTime && dto.StartTime < r.EndTime) ||
                             (dto.EndTime > r.StartTime && dto.EndTime <= r.EndTime) ||
                             (dto.StartTime <= r.StartTime && dto.EndTime >= r.EndTime)))
                .CountAsync();

            if (overlappingCount >= space.Capacity)
                return BadRequest(new { success = false, message = "No available capacity for the selected time slot." });

            reservation.StartTime = dto.StartTime;
            reservation.EndTime = dto.EndTime;
            reservation.ResidentId = dto.ResidentId;
            reservation.SpaceId = dto.SpaceId;

            _dbContext.Reservation.Update(reservation);
            await _dbContext.SaveChangesAsync();

            return Ok(new { success = true, message = "Reservation updated successfully." });
        }


        [HttpDelete("{id}")]
      
        public async Task<IActionResult> Reservations(int id)
        {
            var reservation = await _dbContext.Reservation.FindAsync(id);
            if (reservation == null)
                return NotFound();

            _dbContext.Reservation.Remove(reservation);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet("findReservationsByAvailableSlots")]
        public async Task<IActionResult> findReservationsByAvailableSlots(
     [FromQuery] int spaceId,
     [FromQuery] DateTime date,
     [FromQuery] int slotMinutes = 60)
        {
            var space = await _dbContext.Space.FirstOrDefaultAsync(s => s.Id == spaceId);
            if (space == null)
                return NotFound(new { success = false, message = "Space not found." });

            if (!space.Availability)
                return BadRequest(new { success = false, message = "Space is currently unavailable." });

            if (date.Date < DateTime.Today)
                return BadRequest(new { success = false, message = "Cannot view or make reservations for past dates." });

           
            var startOfDay = date.Date.AddHours(8);
            var endOfDay = date.Date.AddHours(20);

            var reservations = await _dbContext.Reservation
                .Where(r => r.SpaceId == spaceId &&
                            r.StartTime < endOfDay &&
                            r.EndTime > startOfDay)
                .ToListAsync();

            var availableSlots = new List<object>();
            var currentStart = startOfDay;

            while (currentStart < endOfDay)
            {
                var currentEnd = currentStart.AddMinutes(slotMinutes);
                if (currentEnd > endOfDay) break;

                var overlappingReservations = reservations
                    .Where(r => currentStart < r.EndTime && currentEnd > r.StartTime)
                    .ToList();

          
                if (overlappingReservations.Any())
                {
                    currentStart = currentStart.AddMinutes(slotMinutes);
                    continue;
                }

            
                availableSlots.Add(new
                {
                    StartTime = currentStart.ToString("yyyy-MM-ddTHH:mm:ss"),
                    EndTime = currentEnd.ToString("yyyy-MM-ddTHH:mm:ss")
                });

                currentStart = currentStart.AddMinutes(slotMinutes);
            }

            return Ok(new { success = true, data = availableSlots });
        }




    }
}
