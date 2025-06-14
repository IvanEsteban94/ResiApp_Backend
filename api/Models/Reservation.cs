namespace api.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int ResidentId { get; set; }
        public User Resident { get; set; } = null!; // ✅ Relación directa

        public int SpaceId { get; set; }
        public Space Space { get; set; } = null!;
    }
}
