

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using back_end.domain;

namespace back_end.DTO.MenuItems
{
    public class MenuItemBaseDTO
    {
        [MinLength(1)]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Category_Id { get; set; }
        public string? Item_Image_Url { get; set; }
        //public bool is_add_on {get;set;}
        public MenuItemStatus status { get; set; } = MenuItemStatus.Available;
    }

    // public class MenuItemResponseDTO
    // {
    //     [JsonPropertyName("name")]
    //     public string? Name { get; set; }
    //     [JsonPropertyName("description")]
    //     public string? Description { get; set; }
    //     [JsonPropertyName("category_id")]
    //     public int Category_Id { get; set; }
    //     public string? Item_Image_Url { get; set; }
    //     [JsonPropertyName("status")]
    //     public MenuItemStatus? status { get; set; }

    //     public List<int>? Tag_Ids { get; set; }
    // }
    public class MenuItemCreateDTO : MenuItemBaseDTO
    {
        public List<int> Tag_Ids { get; set; } = [];
    }

    public class TagResponseDTO
    {
        [JsonPropertyName("tag_id")]
        public int Tag_Id { get; set; }
        public string Name { get; set; } = string.Empty;

    }
    public class FullTagResponseDTO : TagResponseDTO
    {
        [JsonPropertyName("color_code")]
        public string Color_Code { get; set; } = string.Empty;

    }

    public class MenuItemResponseDTO : MenuItemBaseDTO
    {
        [JsonPropertyName("item_id")]
        public int Item_Id { get; set; }
        [JsonPropertyName("tags")]
        public List<FullTagResponseDTO> Tags { get; set; } = [];
        [JsonPropertyName("created_at")]
        public DateTime? Created_At { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime? Updated_At { get; set; }
    }
    public class MenuItemDetailDTO
    {
        public string Category_Name { get; set; } = string.Empty;
        public List<string> Tag_Names { get; set; } = [];
        public decimal? Current_Price { get; set; }
        public int Total_Orders { get; set; }
    
    }
    
}