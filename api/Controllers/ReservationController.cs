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
    [Route("/api/v1/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public ReservationsController(ApplicationDbContext db)
        {
            _dbContext = db;
        }

        // GET api/reservation/user/5  => reservas por usuario
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

        // GET api/reservation  => todas las reservas
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
        [HttpGet("findReservationsById/{id}")]
        public async Task<IActionResult> findReservationsById(int id)
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
        public async Task<IActionResult> Reservations([FromBody] ReservationDto reservationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (reservationDto.EndTime <= reservationDto.StartTime)
                return BadRequest(new { success = false, message = "EndTime must be after StartTime." });

            var space = await _dbContext.Space.FirstOrDefaultAsync(s => s.Id == reservationDto.SpaceId);
            if (space == null)
                return NotFound(new { success = false, message = "Space not found." });

            // ❗ Validar si queda capacidad
            if (space.Capacity <= 0)
                return BadRequest(new { success = false, message = "No remaining capacity for this space." });

            // Verificar solapamiento de horarios
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

            // 🔻 Reducir capacidad en 1
            space.Capacity -= 1;
            _dbContext.Space.Update(space);

            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(findReservationsById), new { id = reservation.Id }, new { success = true, message = "Reservation created successfully." });
        }

        // PUT api/reservation/5  => actualizar reserva
        [HttpPut("{id}")]
        public async Task<IActionResult> Reservations(int id, [FromBody] Reservation updatedReservation)
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
      [FromQuery] int residentId,
      [FromQuery] int slotMinutes = 60)
        {
            const int MAX_RESERVATIONS_PER_DAY = 2;

            var space = await _dbContext.Space.FirstOrDefaultAsync(s => s.Id == spaceId);
            if (space == null)
                return NotFound(new { success = false, message = "Space not found." });

            if (!space.Availability)
                return BadRequest(new { success = false, message = "Space is currently unavailable." });

            if (date.Date < DateTime.Today)
                return BadRequest(new { success = false, message = "Cannot view or make reservations for past dates." });

            // Definimos el rango horario para ese día: de 8:00 a 20:00
            var startOfDay = date.Date.AddHours(8);
            var endOfDay = date.Date.AddHours(20);

            // Traemos todas las reservas que se solapan con este rango de horas y espacio
            var reservations = await _dbContext.Reservation
                .Where(r => r.SpaceId == spaceId &&
                           r.StartTime < endOfDay &&
                           r.EndTime > startOfDay)
                .ToListAsync();

            // Contamos cuántas reservas tiene el residente para ese día
            int residentDailyReservations = reservations.Count(r => r.ResidentId == residentId);

            if (residentDailyReservations >= MAX_RESERVATIONS_PER_DAY)
                return Ok(new { success = true, data = new List<object>() });

            var availableSlots = new List<object>();
            var currentStart = startOfDay;

            while (currentStart < endOfDay)
            {
                var currentEnd = currentStart.AddMinutes(slotMinutes);
                if (currentEnd > endOfDay) break;

                // Revisamos reservas que se solapan con el slot actual
                var overlappingReservations = reservations
                    .Where(r => currentStart < r.EndTime && currentEnd > r.StartTime)
                    .ToList();

                // Si la cantidad de reservas para ese slot alcanza la capacidad, no está disponible
                if (overlappingReservations.Count >= space.Capacity)
                {
                    currentStart = currentStart.AddMinutes(slotMinutes);
                    continue;
                }

                // Si el residente ya tiene una reserva en ese slot, la descartamos para no repetir
                bool residentHasReservation = overlappingReservations.Any(r => r.ResidentId == residentId);
                if (residentHasReservation)
                {
                    currentStart = currentStart.AddMinutes(slotMinutes);
                    continue;
                }

                // Slot disponible, agregamos a la lista
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
