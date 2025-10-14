using Microsoft.Extensions.Logging;
using back_end.domain.Entities;
using back_end.domain.DbContexts;

namespace back_end.domain.Seeders
{
    /// <summary>
    /// Seeder for the dining_session table with initial data.
    /// Table assignments are handled separately in SessionTableSeeder.
    /// </summary>
    public class DiningSessionSeeder : ISeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DiningSessionSeeder> _logger;

        public DiningSessionSeeder(ApplicationDbContext context, ILogger<DiningSessionSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Seed()
        {
            // Guard: avoid duplicate seeding if reset=false
            if (_context.DiningSessions.Any())
            {
                _logger.LogInformation("Dining sessions already exist; skipping DiningSessionSeeder.");
                return;
            }

            // Retrieve the menu
            var menu = _context.Menus.FirstOrDefault(m => m.Name == "All Day Menu");
            if (menu == null)
            {
                throw new InvalidOperationException("All Day Menu not found. Please run menu seeder first.");
            }

            var generatedSessions = new List<DiningSessionData>
            {
                // ... (unchanged list of 50 entries) ...
            };

            var createdSessions = new List<DiningSession>();
            foreach (var sessionData in generatedSessions)
            {
                var newSession = new DiningSession
                {
                    Menu_Id = menu.Menu_id,
                    Started_At = sessionData.StartedAt,
                    Ended_At = sessionData.EndedAt,
                    First_Order_At = sessionData.FirstOrderTime
                };

                _context.DiningSessions.Add(newSession);
                createdSessions.Add(newSession);
            }

            _context.SaveChanges();

            int activeSessions = createdSessions.Count(s => s.Ended_At == null);
            int completedSessions = createdSessions.Count - activeSessions;

            _logger.LogInformation($"Created {createdSessions.Count} dining sessions ({activeSessions} active, {completedSessions} completed)");
        }

        private class DiningSessionData
        {
            public int MenuId { get; set; }
            public DateTime StartedAt { get; set; }
            public DateTime? EndedAt { get; set; }
            public DateTime FirstOrderTime { get; set; }
        }
    }
}