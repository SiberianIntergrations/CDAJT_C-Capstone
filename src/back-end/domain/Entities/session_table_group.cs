using System.ComponentModel.DataAnnotations.Schema;

namespace back_end.domain.Entities
{
  [Table("session_table_groups")]
  public class SessionTableGroup
  {
    [ForeignKey(nameof(DiningSession))]
    public int Session_Id { get; set; }

    [ForeignKey(nameof(TableGroup))]
    public int TableGroup_Id { get; set; }

    public DiningSession DiningSession { get; set; } = null!;
    public TableGroup TableGroup { get; set; } = null!;
  }
}