

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using back_end.domain;
using back_end.domain.Entities;
using back_end.domain.enums;

namespace back_end.DTO.StaffDTOs
{
    public class StaffResponseDTO
    {
        [JsonPropertyName("created_at")]
        public DateTime Created_At { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }
        [JsonPropertyName("is_email_confirmed")]
        public bool IsEmailConfirmed { get; set; }

        [JsonPropertyName("last_transaction_at")]
        public DateTime LastTransactionAt { get; set; }
        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }
        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        [JsonPropertyName("role")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRoles Role { get; set; }

        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserStatus Status { get; set; }
        [JsonPropertyName("user_id")]
        public int User_Id { get; set; }

        [JsonPropertyName("location")]
        public string? Location { get; set; }

    }

    public class StaffUserUpdateDTO
    {
        [MaxLength(255), EmailAddress]
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [MaxLength(100)]
        [JsonPropertyName("first_name")]
        public string? First_name { get; set; }

        [MaxLength(100)]
        [JsonPropertyName("last_name")]
        public string? Last_name { get; set; }

        // allow moving between Staff/Admin only
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRoles? Role { get; set; }
        
        [JsonPropertyName("location")]
        public string? Location { get; set; }
    }

    public class CreateStaffDTO
    {
        [MaxLength(255), EmailAddress]
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [MaxLength(100)]
        [JsonPropertyName("first_name")]
        public string? First_name { get; set; }
        [JsonPropertyName("password")]
        public string? Password { get; set; }

        [MaxLength(100)]
        [JsonPropertyName("last_name")]
        public string? Last_name { get; set; }

        // allow moving between Staff/Admin only
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRoles? Role { get; set; }

        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserStatus Status { get; set; }

        [JsonPropertyName("location_id")]
        public int? Location_Id { get; set; }
    }

    public class StaffMemberChangePasswordDTO
    {
        [Required, MinLength(8), MaxLength(200)]
        [JsonPropertyName("password")]
        public string New_password { get; set; } = string.Empty;

        [Required, MinLength(8), MaxLength(200)]
        [JsonPropertyName("current_password")]
        public string Current_password { get; set; } = string.Empty;
    }
    
    public class StaffMemberUpdateDTO
    {
                [JsonPropertyName("created_at")]
        public DateTime Created_At { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }
        [JsonPropertyName("is_email_confirmed")]
        public bool IsEmailConfirmed { get; set; }

        [JsonPropertyName("last_transaction_at")]
        public DateTime LastTransactionAt { get; set; }
        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }
        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        [JsonPropertyName("role")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRoles Role { get; set; }

        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserStatus Status { get; set; }
        [JsonPropertyName("user_id")]
        public int User_Id { get; set; }

        [JsonPropertyName("location")]
        public string? Location { get; set; }
    }

    
}