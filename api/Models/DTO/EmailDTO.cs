namespace SendEmail.Models
{
    public class EmailDTO
    {
        
            public string To { get; set; }       // Email del admin
            public string Subject { get; set; }
            public string Body { get; set; }
            public string Role { get; set; }     // admin o residente
            public string? Cc { get; set; }      // Email del residente si aplica
      

    }
}