using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace back_end.domain.Entities
{
    [Table("menu_item_tag")]
    public class MenuItemTag
    {
        //TODO: Composite Keys need to updated and added.
        [Required]
        public int Menu_item_id { get; set; }

        [Required]
        public int Tag_id { get; set; }
    }
}