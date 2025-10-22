using Microsoft.Extensions.Logging;
using back_end.domain.Entities;
using back_end.domain.DbContexts;

namespace back_end.domain.Seeders
{
  /// <summary>
  /// Seeder for dining_sessions table.
  /// Creates mix of active and historical sessions with either individual tables or table groups.
  /// </summary>
  public class DiningSessionSeeder : ISeeder
  {
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DiningSessionSeeder> _logger;
    private readonly Random _rng = new();

    public DiningSessionSeeder(ApplicationDbContext context, ILogger<DiningSessionSeeder> logger)
    {
      _context = context;
      _logger = logger;
    }

    public void Seed()
    {
      var menus = _context.Menus.ToList();
      var tables = _context.Tables.Where(t => t.TableGroup_Id == null).ToList(); // Individual tables not in groups
      var tableGroups = _context.TableGroups.ToList();

      if (!menus.Any() || (!tables.Any() && !tableGroups.Any()))
      {
        _logger.LogWarning("Cannot seed dining sessions: missing menus or tables/groups");
        return;
      }

      var sessions = new List<DiningSession>();
      var now = DateTime.UtcNow;

      // Create 5 active sessions (mix of individual tables and groups)
      for (int i = 1; i <= 5; i++)
      {
        var startTime = now.AddHours(-_rng.Next(1, 4));
        var menu = menus[_rng.Next(menus.Count)];

        DiningSession session;

        if (i <= 3 && tables.Any())
        {
          // First 3 sessions: individual tables
          var table = tables[_rng.Next(tables.Count)];
          session = new DiningSession
          {
            Menu_Id = menu.Menu_id,
            Started_At = startTime,
            First_Order_At = startTime.AddMinutes(_rng.Next(5, 20)),
            Ended_At = null,
            Table_Id = table.Table_Id
          };
        }
        else if (tableGroups.Any())
        {
          // Last 2 sessions: table groups
          var group = tableGroups[_rng.Next(tableGroups.Count)];
          session = new DiningSession
          {
            Menu_Id = menu.Menu_id,
            Started_At = startTime,
            First_Order_At = startTime.AddMinutes(_rng.Next(5, 20)),
            Ended_At = null,
            TableGroup_Id = group.TableGroup_Id
          };
        }
        else
        {
          continue;
        }

        sessions.Add(session);
      }

      // Create 45 historical sessions (mix of individual tables and groups)
      for (int i = 6; i <= 50; i++)
      {
        var daysAgo = _rng.Next(1, 90);
        var startTime = now.AddDays(-daysAgo).AddHours(_rng.Next(9, 20));
        var duration = _rng.Next(45, 180);
        var menu = menus[_rng.Next(menus.Count)];

        DiningSession session;

        // 70% individual tables, 30% table groups
        if (_rng.NextDouble() < 0.7 && tables.Any())
        {
          var table = tables[_rng.Next(tables.Count)];
          session = new DiningSession
          {
            Menu_Id = menu.Menu_id,
            Started_At = startTime,
            First_Order_At = startTime.AddMinutes(_rng.Next(5, 20)),
            Ended_At = startTime.AddMinutes(duration),
            Table_Id = table.Table_Id
          };
        }
        else if (tableGroups.Any())
        {
          var group = tableGroups[_rng.Next(tableGroups.Count)];
          session = new DiningSession
          {
            Menu_Id = menu.Menu_id,
            Started_At = startTime,
            First_Order_At = startTime.AddMinutes(_rng.Next(5, 20)),
            Ended_At = startTime.AddMinutes(duration),
            TableGroup_Id = group.TableGroup_Id
          };
        }
        else
        {
          continue;
        }

        sessions.Add(session);
      }

      _context.DiningSessions.AddRange(sessions);

      var activeCount = sessions.Count(s => s.Ended_At == null);
      var historicalCount = sessions.Count(s => s.Ended_At != null);
      _logger.LogInformation($"Added {sessions.Count} dining sessions ({activeCount} active, {historicalCount} historical)");
    }
  }
}
