using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace back_end.domain.Entities
{
    [Table("menu_item")]
    public class Menu_Item
    {
        [Key]
        public int item_id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        //TODO: This will need to be a foreign key to the menu table
        public int Category_id { get; set; }
        public string? image_url { get; set; }

        public MenuItemStatus Status { get; set; } = MenuItemStatus.Available;

    }
}