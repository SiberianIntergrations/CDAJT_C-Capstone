

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using back_end.domain;

namespace back_end.DTO.TableEntityDTOs
{
  public class TableEntityBaseDTO
  {
    [JsonPropertyName("table_number")]
    [Range(0, int.MaxValue)]
    public int Table_Number { get; set; }
    [JsonPropertyName("seat_count")]
    [Range(0, int.MaxValue)]
    public int Seat_Count { get; set; }
    [JsonPropertyName("is_active")]
    public bool Is_Active { get; set; } = true;

    public string? Qr_Code_Url { get; set; }

    public int Location_Id { get; set; }
  }

  public class TableEntityCreateDTO : TableEntityBaseDTO
  {

  }

  public class TableEntityUpdateDTO
  {
    [JsonPropertyName("table_number")]
    [Range(0, int.MaxValue)]
    public int? Table_Number { get; set; }
    [JsonPropertyName("seat_count")]
    [Range(0, int.MaxValue)]
    public int? Seat_Count { get; set; }
    [JsonPropertyName("is_active")]
    public bool? Is_Active { get; set; }

    public string? Qr_Code_Url { get; set; }
  }

  public class TableEntityResponseDTO : TableEntityBaseDTO
  {
    [JsonPropertyName("table_id")]
    public int Table_Id { get; set; }
  }

}