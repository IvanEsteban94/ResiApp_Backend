﻿using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApi.Data;
using System.Linq;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly ApplicationDbContext _dbCongtext;

        public ReservationController(ApplicationDbContext db)
        {
            _dbCongtext = db;
        }

        [HttpGet("GetReservation")]
        public IActionResult GetReservations()
        {
            var reservations = _dbCongtext.Reservation.ToList();
            return Ok(new { success = true, data = reservations });
        }

        [HttpGet("GetReservation/{id}")]
        public IActionResult GetReservationById(int id)
        {
            var reservation = _dbCongtext.Reservation.FirstOrDefault(r => r.Id == id);
            if (reservation == null)
                return NotFound(new { success = false, message = "Reservation not found." });

            return Ok(new { success = true, data = reservation });
        }

        [HttpPost("CreateReservation")]
        public IActionResult CreateReservation([FromBody] Reservation reservation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _dbCongtext.Reservation.Add(reservation);
            _dbCongtext.SaveChanges();

            return CreatedAtAction(nameof(GetReservationById), new { id = reservation.Id }, new { success = true, message = "Reservation created successfully." });
        }

        [HttpPut("UpdateReservation/{id}")]
        public IActionResult UpdateReservation(int id, [FromBody] Reservation updatedReservation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reservation = _dbCongtext.Reservation.FirstOrDefault(r => r.Id == id);
            if (reservation == null)
                return NotFound(new { success = false, message = "Reservation not found." });

            reservation.StartTime = updatedReservation.StartTime;
            reservation.EndTime = updatedReservation.EndTime;
            reservation.ResidentId = updatedReservation.ResidentId;
            reservation.SpaceId = updatedReservation.SpaceId;

            _dbCongtext.Reservation.Update(reservation);
            _dbCongtext.SaveChanges();

            return Ok(new { success = true, message = "Reservation updated successfully." });
        }

        [HttpDelete("DeleteReservation/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteReservation(int id)
        {
            var reservation = _dbCongtext.Reservation.FirstOrDefault(r => r.Id == id);
            if (reservation == null)
                return NotFound(new { success = false, message = "Reservation not found." });

            _dbCongtext.Reservation.Remove(reservation);
            _dbCongtext.SaveChanges();

            return Ok(new { success = true, message = "Reservation deleted successfully." });
        }
    }
}
