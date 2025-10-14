using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using back_end.domain.DbContexts;
using back_end.domain.Entities;

namespace back_end.domain.Seeders
{
    public class SessionTableSeeder : ISeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SessionTableSeeder> _logger;
        private readonly Random _rng = new();

        public SessionTableSeeder(ApplicationDbContext context, ILogger<SessionTableSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Seed()
        {
            var sessions = _context.DiningSessions
                .OrderByDescending(s => s.Started_At)
                .Take(50)
                .ToList();

            if (!sessions.Any())
            {
                _logger.LogInformation("SessionTableSeeder: no sessions found.");
                return;
            }

            var allTables = _context.Tables.ToList();
            if (!allTables.Any())
            {
                _logger.LogInformation("SessionTableSeeder: no tables found.");
                return;
            }

            var existing = _context.SessionTables
                .Select(st => new { st.Session_Id, st.Table_Id })
                .AsEnumerable()                           
                .Select(x => (x.Session_Id, x.Table_Id))
                .ToHashSet();

            int added = 0;

            foreach (var s in sessions)
            {
                int desired = _rng.Next(2, 6);
                var picked = allTables
                    .OrderBy(_ => _rng.Next())
                    .Take(Math.Min(desired, allTables.Count))
                    .ToList();

                foreach (var t in picked)
                {
                    var key = (s.Session_Id, t.Table_Id);
                    if (existing.Contains(key)) continue;

                    _context.SessionTables.Add(new Sessions
                    {
                        Session_Id = s.Session_Id,
                        Table_Id = t.Table_Id
                    });

                    existing.Add(key);
                    added++;
                }
            }

            if (added > 0)
            {
                _context.SaveChanges();
                _logger.LogInformation("SessionTableSeeder: added {Count} links.", added);
            }
            else
            {
                _logger.LogInformation("SessionTableSeeder: no links to add.");
            }
        }
    }
}
