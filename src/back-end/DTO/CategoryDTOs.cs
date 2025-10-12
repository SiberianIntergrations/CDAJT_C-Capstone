
using System.Text.Json.Serialization;

namespace back_end.DTO.Category
{
    public class CategoryDTOs
    {
        public record CategoryCreateDTO(string Name, string Description);
        public record CategoryUpdateDTO(string Name, string Description);
        public record CategoryReadDTO(int Id, string Name, string Description);
    }

    public class CreateCategory
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("adult_limit")]
        public int Adult_Limit { get; set; }
        [JsonPropertyName("child_limit")]
        public int Child_Limit { get; set; }
        [JsonPropertyName("senior_limit")]
        public int Senior_Limit { get; set; }

        [JsonPropertyName("total_limit")]
        public int Total_Limit { get; set; }
    }

    public class UpdateCategory
    {
            private int adult_Limit;
            private int child_Limit;
            private int senior_Limit;
            private int total_Limit;
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("adult_limit")]
        public int Adult_Limit
        {
            get => adult_Limit;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Adult_Limit), "Adult_Limit cannot be negative.");
                adult_Limit = value;
            }
        }

        [JsonPropertyName("total_limit")]
        public int Total_Limit
        {
            get => total_Limit;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Total_Limit), "Total_Limit cannot be negative.");
                total_Limit = value;
            }
        }

        [JsonPropertyName("child_limit")]
        public int Child_Limit { get => child_Limit; set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Child_Limit), "Child_Limit cannot be negative.");
                }
                if (value > Adult_Limit || value > Total_Limit)
                {
                    throw new ArgumentOutOfRangeException(nameof(Child_Limit), "Child_Limit cannot be greater than Adult_Limit or Total_Limit.");
                }
                Child_Limit = value;
            } }

        [JsonPropertyName("senior_limit")]
        public int Senior_Limit {        
        get => senior_Limit;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(Senior_Limit), "Senior_Limit cannot be negative.");
            if (value > Adult_Limit || value > Total_Limit)
                throw new ArgumentOutOfRangeException(nameof(Senior_Limit), "Senior_Limit cannot be greater than Adult_Limit or Total_Limit.");
            senior_Limit = value;
        } }

    }
}