using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using back_end.Domain.Entitys;

namespace back_end.Domain.Seeders
{
    /// <summary>
    /// Seeds the SessionTable association (DiningSession ↔ TableEntity).
    /// </summary>
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
            // Clear existing join rows to prevent duplicates
            _context.SessionTables.RemoveRange(_context.SessionTables);
            _context.SaveChanges();

            var sessions = _context.DiningSessions
                .Where(s => s.SessionId <= 50)
                .ToList();

            var allTables = _context.Tables.ToList();

            // Session-specific table counts
            var sessionTableCounts = new Dictionary<int, int>
            {
                {1,2},{2,2},{3,3},{4,2},{5,4},{6,5},{7,4},{8,5},{9,2},{10,4},
                {11,2},{12,3},{13,5},{14,4},{15,2},{16,3},{17,3},{18,4},{19,3},{20,3},
                {21,3},{22,3},{23,5},{24,4},{25,4},{26,5},{27,5},{28,3},{29,4},{30,2},
                {31,2},{32,3},{33,4},{34,5},{35,5},{36,5},{37,4},{38,2},{39,4},{40,5},
                {41,5},{42,4},{43,4},{44,3},{45,4},{46,3},{47,5},{48,5},{49,2},{50,3}
            };

            int tablesAssigned = 0;

            foreach (var session in sessions)
            {
                var wanted = sessionTableCounts.TryGetValue(session.SessionId, out var n) ? n : 2;

                // pick N distinct random tables
                var picked = allTables
                    .OrderBy(_ => _rng.Next())
                    .Take(Math.Min(wanted, allTables.Count))
                    .ToList();

                // If you have a Many-to-Many via join entity:
                foreach (var t in picked)
                {
                    _context.SessionTables.Add(new SessionTable
                    {
                        SessionId = session.SessionId,
                        TableId = t.TableId
                    });
                    tablesAssigned++;
                }
            }

            _context.SaveChanges();
            _logger.LogInformation($"Assigned {tablesAssigned} tables across {sessions.Count} dining sessions");
        }
    }
}