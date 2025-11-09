using Microsoft.Extensions.Logging;
using back_end.domain.Entities;
using back_end.domain.DbContexts;

namespace back_end.domain.Seeders
{
  /// <summary>
  /// Seeder for dining_sessions table.
  /// Creates mix of active and historical sessions with either individual tables or table groups per location.
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
      var locations = _context.Locations.ToList();
      var menus = _context.Menus.ToList();

      if (!locations.Any() || !menus.Any())
      {
        _logger.LogWarning("Cannot seed dining sessions: missing locations or menus");
        return;
      }

      var allSessions = new List<DiningSession>();
      var now = DateTime.UtcNow;

      foreach (var location in locations)
      {
        var tables = _context.Tables
            .Where(t => t.Location_Id == location.Location_Id && t.TableGroup_Id == null)
            .ToList(); // Individual tables not in groups
        var tableGroups = _context.TableGroups
            .Where(tg => tg.Location_Id == location.Location_Id)
            .ToList();

        if (!tables.Any() && !tableGroups.Any())
        {
          _logger.LogWarning($"Cannot seed dining sessions for location {location.Name}: no tables or groups found");
          continue;
        }

        var sessions = new List<DiningSession>();

        // Create 10 active sessions (mix of individual tables and groups) per location
        for (int i = 1; i <= 10; i++)
        {
          var startTime = now.AddHours(-_rng.Next(1, 4));
          var menu = menus[_rng.Next(menus.Count)];

          DiningSession session;

          // 70% individual tables, 30% table groups
          if (_rng.NextDouble() < 0.7 && tables.Any())
          {
            var table = tables[_rng.Next(tables.Count)];
            session = new DiningSession
            {
              Menu_Id = menu.Menu_id,
              Location_Id = location.Location_Id,
              Started_At = startTime,
              First_Order_At = startTime.AddMinutes(_rng.Next(5, 20)),
              Ended_At = null,
              Table_Id = table.Table_Id
            };
          }
          else if (tableGroups.Any())
          {
            var group = tableGroups[_rng.Next(tableGroups.Count)];
            session = new DiningSession
            {
              Menu_Id = menu.Menu_id,
              Location_Id = location.Location_Id,
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

        // Create 40 historical sessions (mix of individual tables and groups) per location
        for (int i = 11; i <= 50; i++)
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
              Location_Id = location.Location_Id,
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
              Location_Id = location.Location_Id,
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

        allSessions.AddRange(sessions);

        var activeCount = sessions.Count(s => s.Ended_At == null);
        var historicalCount = sessions.Count(s => s.Ended_At != null);
        _logger.LogInformation($"Added {sessions.Count} dining sessions for {location.Name} ({activeCount} active, {historicalCount} historical)");
      }

      _context.DiningSessions.AddRange(allSessions);

      var totalActive = allSessions.Count(s => s.Ended_At == null);
      var totalHistorical = allSessions.Count(s => s.Ended_At != null);
      _logger.LogInformation($"Total: {allSessions.Count} dining sessions across {locations.Count} locations ({totalActive} active, {totalHistorical} historical)");
    }
  }
}