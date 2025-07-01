namespace api.Models
{
    public class Review
    {
        
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = null!;

        public int ResidentId { get; set; } 
        public User Resident { get; set; } = null!;  

        public int SpaceId { get; set; }    
        public Space Space { get; set; } = null!;   
    }
}
