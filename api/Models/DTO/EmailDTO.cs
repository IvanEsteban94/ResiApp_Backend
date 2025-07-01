namespace SendEmail.Models
{
    public class EmailDTO
    {

  

       
        public List<string> To { get; set; } = new();


        public List<string>? Cc { get; set; } = new();

        public string Subject { get; set; }
        public string Body { get; set; }

      
        public string Role { get; set; }


    }
}