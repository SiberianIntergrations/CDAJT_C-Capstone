using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace back_end.domain.Entities
{
    [Table("Categories")]
    public class Category
    {
        [Key]

        public int Category_id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Category_Name { get; set; } = string.Empty;

        [Required]
        
        public string Description { get; set; } = string.Empty;

        public string? Image_Url { get; set; }

        [MaxLength(255)]
        public int Total_Views { get; set; } = 0;

        public int Total_View_Seconds { get; set; } = 0;

        public DateTime Last_Viewed_At { get; set; }

        public int Adult_Limit { get; set; } = 0;

        public int Child_Limit { get; set; } = 0;

        public int Senior_Limit { get; set; } = 0;

        public int Total_Limit { get; set; } = 0;

        public ICollection<Menu_Item> MenuItems { get; set; } = new List<Menu_Item>();
    }
}