using api.Dtos;

namespace api.Models.DTO
{
    public class ReservationDetailsDto
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public UserDto User { get; set; }
        public SpaceDto Space { get; set; }
    }
}
