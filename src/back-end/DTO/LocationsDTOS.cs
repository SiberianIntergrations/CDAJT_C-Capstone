using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using back_end.domain;
using Microsoft.AspNetCore.SignalR;

namespace back_end.DTO.LocationDTOs
{
    public class LocationBaseDTO
    {
        [MaxLength(125)]
        [MinLength(1)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(75)]
        [MinLength(1)]
        public string Address_One { get; set; } = string.Empty;
        [MaxLength(75)]
        public string Address_Two { get; set; } = string.Empty;
        [MaxLength(50)]
        [MinLength(1)]
        public string City { get; set; } = string.Empty;
        [MaxLength(20)]
        [MinLength(1)]
        public string Province { get; set; } = string.Empty;
        [MaxLength(8)]
        [MinLength(1)]
        public string Postal_Code { get; set; } = string.Empty;
        [MaxLength(11)]
        [MinLength(10)]
        public string Phone_Number { get; set; } = string.Empty;

    }

    public class LocationCreateDTO : LocationBaseDTO
    {

    }

    public class LocationUpdateDTO : LocationBaseDTO { }

    public class LocationResponseDTO : LocationBaseDTO
    {
        public int Location_Id { get; set; }
    }

    public class MenuLocationCreateDTO
    {
        [Required]
        public int Menu_id { get; set; }
    }

}