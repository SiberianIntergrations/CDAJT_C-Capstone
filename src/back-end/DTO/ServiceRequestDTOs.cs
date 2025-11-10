using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using back_end.domain.enums;

namespace back_end.DTO.ServiceRequestDTOs
{
    public class ServiceRequestCreateDTO
    {
        [JsonPropertyName("notes")]
        public string? Notes { get; set; }

        [JsonPropertyName("request_by_oid")]
        public string? Request_By_Oid { get; set; }

        [JsonPropertyName("request_by_name")]
        public string? Request_By_Name { get; set; }

    }

    public class ServiceRequestResponseDTO
    {
        [JsonPropertyName("request_id")]
        public int Request_Id { get; set; }
        [JsonPropertyName("session_id")]
        public int Session_Id { get; set; }
        [JsonPropertyName("table_id")]
        public int Table_Id { get; set; }
        [JsonPropertyName("request_by_oid")]
        public string Request_By_Oid { get; set; } = string.Empty;
        [JsonPropertyName("request_by_name")]
        public string Request_By_Name { get; set; } = string.Empty;
        [JsonPropertyName("claimed_by_oid")]
        public string? Claimed_By_Oid { get; set; }
        [JsonPropertyName("claimed_by_name")]
        public string? Claimed_By_Name { get; set; }
        [JsonPropertyName("notes")]
        public string? Notes { get; set; }
        [JsonPropertyName("status")]
        public ServiceRequestStatus Status { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime Created_At { get; set; }
        [JsonPropertyName("claimed_at")]
        public DateTime? Claimed_At { get; set; }
        [JsonPropertyName("completed_at")]
        public DateTime? Completed_At    { get; set; }
        [JsonPropertyName("table_number")]
        public int Table_Number{ get; set; }
    }
    
}