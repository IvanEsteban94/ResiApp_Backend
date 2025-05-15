namespace api.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }

        public int ResidentId { get; set; }
       
    }
}
