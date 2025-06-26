namespace SendEmail.Models
{
    public class EmailDTO
    {

  

        // 2. Lista de destinatarios
        public List<string> To { get; set; } = new();

        // 3. Lista de copias (opcional)
        public List<string>? Cc { get; set; } = new();

        public string Subject { get; set; }
        public string Body { get; set; }

        // "admin" o "residente"
        public string Role { get; set; }


    }
}