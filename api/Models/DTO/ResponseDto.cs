
namespace api.Models.DTO
{

    public class ResponseDto
    {
        public bool Success { get; set; } = false;
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
    }
}
