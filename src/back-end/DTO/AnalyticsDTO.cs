

using System.Text.Json.Serialization;

namespace back_end.DTO.Analytics
{
    public class BrowsingBehaviorDTO
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int ViewCount { get; set; }

        public int totalViewTimes { get; set; }
    }

    public class TableTurnoverDailyDTO
    {
        public int Party_size { get; set; }
        public string Day { get; set; } = string.Empty;
        public int AverageDuration { get; set; }

    }
    public class TableTurnOverMonthlyDTO
    {
        public int Party_size { get; set; }
        public string Month { get; set; } = string.Empty;
        public int AverageDuration { get; set; }
    }

    public class OrderTimingDTO
    {
        public int SessionId { get; set; }
        public int TimeToFirstOrder { get; set; }

    }

    public class TableTurnOverResponseDTO
    {
        public List<TableTurnoverDailyDTO> Daily { get; set; } = new List<TableTurnoverDailyDTO>();
        public List<TableTurnOverMonthlyDTO> Monthly { get; set; } = new List<TableTurnOverMonthlyDTO>();

    }

    public class DailyAverageTimingDTO
    {
        public string Day { get; set; } = string.Empty;
        public double AverageTimeToFirstOrder { get; set; }

    }

    public class OrderTimingResponseDTO
    {
        public List<OrderTimingDTO> OrderTimings { get; set; } = new List<OrderTimingDTO>();
        public List<DailyAverageTimingDTO> DailyAverages { get; set; } = new List<DailyAverageTimingDTO>();
    }
    public class TurnoverMetricDTO
    {
        public string? Period { get; set; }
        public int AverageDuration { get; set; }
        public int PartySize { get; set; }
    }

    public class ItemPerformanceDTO
    {
        [JsonPropertyName("item_id")]
        public int item_id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("total_units_sold")]
        public int TotalUnitsSold { get; set; }
    }
}