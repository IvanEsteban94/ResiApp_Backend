using api.Models;
using Microsoft.AspNetCore.Mvc;
using MyApi.Data;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReservationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] Reservation reservation)
        {
            _context.Reservation.Add(reservation);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(ReadReservation), new { id = reservation.Id }, reservation);
        }

        // READ (por id)
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> ReadReservation(int id)
        {
            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
                return NotFound();

            return Ok(reservation);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReservation(int id, [FromBody] Reservation updated)
        {
            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
                return NotFound();

            reservation.StartTime = updated.StartTime;
            reservation.EndTime = updated.EndTime;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE
        [HttpDelete("{id}")]
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
