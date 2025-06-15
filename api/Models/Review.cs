namespace api.Models
{
    public class Review
    {
        
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = null!;

        public int ResidentId { get; set; }  // FK hacia User
        public User Resident { get; set; } = null!;  // Propiedad de navegación

        public int SpaceId { get; set; }    // FK hacia Space
        public Space Space { get; set; } = null!;    // Propiedad de navegación
    }
}
