namespace api.Models.DTO
{
    public class UpdateReservationDto
    {
        public int ResidentId { get; set; }
        public int SpaceId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
