using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using back_end.domain;

namespace back_end.DTO.DashBoardDTOs
{
    public class TableAssignmentDTO
    {
        [JsonPropertyName("table_id")]
        [Required(ErrorMessage = "Table Id is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Table ID Must be Positive")]
        public int Table_Id { get; set; }
    }

    public class CreateSessionRequestDTO
    {
        [JsonPropertyName("menu_id")]
        [Required(ErrorMessage = "Menu Id is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Menu Id Bust be Positive")]
        public int Menu_Id { get; set; }

    }

    public class DiningSessionResponseDTO
    {
        public int Session_Id { get; set; }
        public int Menu_Id { get; set; }
        public DateTime Started_at { get; set; }
        public DateTime? Ended_at { get; set; }
        public DateTime? First_Order_Time { get; set; }
        public List<int> Table_Numbers { get; set; } = new List<int>();
        public int Active_Participants { get; set; }
    }

    class DiningSessionDetailDTO : DiningSessionResponseDTO
    {
        public int Orders_Count { get; set; }
        public int Bills_Count { get; set; }
        public int Total_participant { get; set; }

        public int Active_Bill_Count { get; set; }


    }

    public class SessionIdResponseDTO
    {
        public int Session_Id { get; set; }


    }
    
    public class SessionMenuResponseDTO
    {
        public int Menu_Id { get; set; }
    }
}
