namespace api.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int ResidentId { get; set; }
        public int SpaceId { get; set; }

        // Relación de navegación
        public Space Space { get; set; }
    }
}
