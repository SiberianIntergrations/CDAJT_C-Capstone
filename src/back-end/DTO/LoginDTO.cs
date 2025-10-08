using System.Text.Json.Serialization;

namespace back_end.DTO
{
    public class LoginDTO
    {
        [JsonPropertyName("email")]
        public string? UserEmail { get; set; }
        
        [JsonPropertyName("password")]
        public string? UserPassword { get; set; }
    }
}