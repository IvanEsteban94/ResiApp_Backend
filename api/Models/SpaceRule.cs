using System.Text.Json.Serialization;

namespace api.Models
{
    public class SpaceRule
    {
        public int Id { get; set; }
        public string Rule { get; set; } = string.Empty;

        [JsonIgnore]
        public List<Space> Spaces { get; set; } = new();
    }
}
