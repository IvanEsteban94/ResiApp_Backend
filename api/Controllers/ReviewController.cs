using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ReservationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // CREATE - Solo Residente
        [HttpPost]
        [Authorize(Policy = "OnlyResidente")]
        public async Task<IActionResult> CreateReservation([FromBody] Reservation reservation)
        {
            _context.Reservation.Add(reservation);
            await _context.SaveChangesAsync();
            return Ok(reservation);
        }

        // READ - Solo Residente
        [HttpGet("{id}")]
        [Authorize(Policy = "OnlyResidente")]
        public async Task<ActionResult<Reservation>> ReadReservation(int id)
        {
            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
                return NotFound();
            return Ok(reservation);
        }

        // UPDATE - Solo Admin
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateReservation(int id, [FromBody] Reservation updatedReservation)
        {
            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
                return NotFound();

            reservation.StartTime = updatedReservation.StartTime;
            reservation.EndTime = updatedReservation.EndTime;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE - Solo Admin
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
                return NotFound();

            _context.Reservation.Remove(reservation);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
