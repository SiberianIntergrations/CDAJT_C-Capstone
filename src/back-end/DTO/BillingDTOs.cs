using System.Text.Json.Serialization;

namespace back_end.DTO.bill
{
  public class CreateBill
  {
    [JsonPropertyName("bill_name")]
    public string? Bill_Name { get; set; }
    [JsonPropertyName("senior_count")]
    public int Senior_Count { get; set; }

    [JsonPropertyName("adult_count")]
    public int Adult_Count { get; set; }

    [JsonPropertyName("child_count")]
    public int Child_Count { get; set; }
    [JsonPropertyName("tot_count")]
    public int Tot_Count { get; set; }
  }

  public class BillResponse
  {
    [JsonPropertyName("bill_id")]
    public int Bill_Id { get; set; }

    [JsonPropertyName("session_id")]
    public int Session_Id { get; set; }

    [JsonPropertyName("bill_name")]
    public string Bill_Name { get; set; } = string.Empty;

    [JsonPropertyName("senior_count")]
    public int Senior_Count { get; set; }

    [JsonPropertyName("adult_count")]
    public int Adult_Count { get; set; }

    [JsonPropertyName("child_count")]
    public int Child_Count { get; set; }

    [JsonPropertyName("total_count")]
    public int Total_Count { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("created_at")]
    public DateTime Created_At { get; set; }

    [JsonPropertyName("closed_at")]
    public string? Closed_At { get; set; }

    [JsonPropertyName("table_numbers")]
    public List<int> Table_Numbers { get; set; } = new List<int>();
  }

}


