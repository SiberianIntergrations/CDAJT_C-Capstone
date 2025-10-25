using Microsoft.Extensions.Logging;
using back_end.domain.Entities;
using back_end.domain.DbContexts;

namespace back_end.domain.Seeders
{
  /// <summary>
  /// Seeder for the table_entity table with initial data.
  /// Creates 34 tables total per location:
  /// - 9 2-tops (tables 1-9)
  /// - 25 4-tops (tables 10-34)
  /// </summary>
  public class TableSeeder : ISeeder
  {
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TableSeeder> _logger;

    public TableSeeder(ApplicationDbContext context, ILogger<TableSeeder> logger)
    {
      _context = context;
      _logger = logger;
    }

    public void Seed()
    {
      var locations = _context.Locations.ToList();

      if (!locations.Any())
      {
        _logger.LogWarning("Cannot seed tables: no locations found");
        return;
      }

      var tables = new List<TableEntity>();

      foreach (var location in locations)
      {
        // Add 9 2-tops (tables 1-9) per location
        for (int i = 1; i <= 9; i++)
        {
          tables.Add(new TableEntity
          {
            table_number = i,
            seat_count = 2,
            is_active = true,
            Location_Id = location.Location_Id
          });
        }

        // Add 25 4-tops (tables 10-34) per location
        for (int i = 10; i <= 34; i++)
        {
          tables.Add(new TableEntity
          {
            table_number = i,
            seat_count = 4,
            is_active = true,
            Location_Id = location.Location_Id
          });
        }
      }

      _context.Tables.AddRange(tables);

      _logger.LogInformation($"Added {tables.Count} tables across {locations.Count} locations (34 tables per location: 2-tops: tables 1-9, 4-tops: tables 10-34)");
    }
  }
}