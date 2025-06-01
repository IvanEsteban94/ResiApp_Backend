namespace api.Models.DTO
{
    public class SpaceRUle
    {
        public int Id { get; set; }
        public string Rule { get; set; }

        // Clave foránea
        public int SpaceId { get; set; }
        public Space Space { get; set; }
    }
}
