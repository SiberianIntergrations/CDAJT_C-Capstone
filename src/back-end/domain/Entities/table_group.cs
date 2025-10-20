using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace back_end.domain.Entities
{
  [Table("table_groups")]
  public class TableGroup
  {
    [Key]
    public int TableGroup_Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Group_Name { get; set; } = string.Empty;

    public bool Is_Active { get; set; } = true;

    public DateTime Created_At { get; set; } = DateTime.Now;

    public ICollection<TableEntity> Tables { get; set; } = new List<TableEntity>();
    public ICollection<DiningSession> DiningSessions { get; set; } = new List<DiningSession>();
  }
}
