

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using back_end.domain;

namespace back_end.DTO.SessionParticipantDTOs
{
    public class SessionParticipantResponseDTO
    {
        [JsonPropertyName("participant")]
        public int Participant_Id { get; set; }
        [JsonPropertyName("session_id")]
        public int Session_Id { get; set; }
        [JsonPropertyName("user_id")]
        public int User_Id { get; set; }
        [JsonPropertyName("joined_at")]
        public DateTime Joined_At { get; set; }
        [JsonPropertyName("Left_At")]
        public DateTime? Left_At { get; set; }
    }
    
}