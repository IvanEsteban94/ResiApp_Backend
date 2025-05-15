
namespace api.Models.DTO
{
    public class ResponseDto
    {
        internal List<Resident> data;

        public int Id { get; set; }
        public string resident { get; set; } = "";
        public string email { get; set; } = "";
        public string password { get; set; } = "";
        public string apartmentInformation { get; set; } = "";
        public string message { get; set; } = "";
        public bool success { get; set; } = false;
    }
}
