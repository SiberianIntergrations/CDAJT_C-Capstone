using Microsoft.Extensions.Logging;
using back_end.domain.Entities;
using back_end.domain.DbContexts;

namespace back_end.domain.Seeders
{
  /// <summary>
  /// Seeder for table_groups table with initial data.
  /// Creates logical groupings of tables for larger parties.
  /// </summary>
  public class TableGroupSeeder : ISeeder
  {
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TableGroupSeeder> _logger;

    public TableGroupSeeder(ApplicationDbContext context, ILogger<TableGroupSeeder> logger)
    {
      _context = context;
      _logger = logger;
    }

    public void Seed()
    {
      var tables = _context.Tables.OrderBy(t => t.table_number).ToList();
      var tableGroups = new List<TableGroup>();

      // Create table group 1: Tables 1-3 (three 2-tops = 6 seats)
      var group1 = new TableGroup
      {
        Group_Name = "Small Party Area (Tables 1-3)",
        Is_Active = true,
        Created_At = DateTime.UtcNow
      };
      tableGroups.Add(group1);

      // Create table group 2: Tables 10-12 (three 4-tops = 12 seats)
      var group2 = new TableGroup
      {
        Group_Name = "Medium Party Area (Tables 10-12)",
        Is_Active = true,
        Created_At = DateTime.UtcNow
      };
      tableGroups.Add(group2);

      // Create table group 3: Tables 20-24 (five 4-tops = 20 seats)
      var group3 = new TableGroup
      {
        Group_Name = "Large Party Area (Tables 20-24)",
        Is_Active = true,
        Created_At = DateTime.UtcNow
      };
      tableGroups.Add(group3);

      // Create table group 4: Tables 30-34 (five 4-tops = 20 seats)
      var group4 = new TableGroup
      {
        Group_Name = "Banquet Area (Tables 30-34)",
        Is_Active = true,
        Created_At = DateTime.UtcNow
      };
      tableGroups.Add(group4);

      _context.TableGroups.AddRange(tableGroups);
      _context.SaveChanges();

      // Now assign tables to groups
      var group1Id = tableGroups[0].TableGroup_Id;
      var group2Id = tableGroups[1].TableGroup_Id;
      var group3Id = tableGroups[2].TableGroup_Id;
      var group4Id = tableGroups[3].TableGroup_Id;

      // Assign tables 1-3 to group 1
      foreach (var table in tables.Where(t => t.table_number >= 1 && t.table_number <= 3))
      {
        table.TableGroup_Id = group1Id;
      }

      // Assign tables 10-12 to group 2
      foreach (var table in tables.Where(t => t.table_number >= 10 && t.table_number <= 12))
      {
        table.TableGroup_Id = group2Id;
      }

      // Assign tables 20-24 to group 3
      foreach (var table in tables.Where(t => t.table_number >= 20 && t.table_number <= 24))
      {
        table.TableGroup_Id = group3Id;
      }

      // Assign tables 30-34 to group 4
      foreach (var table in tables.Where(t => t.table_number >= 30 && t.table_number <= 34))
      {
        table.TableGroup_Id = group4Id;
      }

      _context.SaveChanges();

      _logger.LogInformation($"Added {tableGroups.Count} table groups with tables assigned");
    }
  }
}
