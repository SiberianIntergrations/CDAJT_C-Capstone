using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using back_end.domain.Entities;
using back_end.domain;
using back_end.domain.DbContexts;

namespace back_end.domain.Seeders
{
    /// <summary>
    /// Seeds SessionParticipant with staff and customers per session.
    /// </summary>
    public class SessionParticipantSeeder : ISeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SessionParticipantSeeder> _logger;
        private readonly Random _rng = new();

        public SessionParticipantSeeder(ApplicationDbContext context, ILogger<SessionParticipantSeeder> logger)
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
                throw new InvalidOperationException("No dining sessions found. Seed sessions first.");

            var customers = _context.Users
                .Where(u => u.Role == UserRoles.Customer)
                .ToList();

            var staff = _context.Users
                .Where(u => u.Role == UserRoles.Staff)
                .ToList();

            if (!customers.Any() || !staff.Any())
                throw new InvalidOperationException("Not enough users found. Seed users first.");

            var activeAssigned = new HashSet<int>();
            int created = 0;

            foreach (var s in sessions)
            {
                var sessionStart = s.Started_At; // adjust if different
                var isActive = s.Ended_At == null;

                if (!isActive) activeAssigned.Clear();

                var sessionStaff = staff[_rng.Next(staff.Count)];
                _context.SessionParticipants.Add(new SessionParticipant
                {
                    Session_Id = s.Session_Id,
                    User_Id = sessionStaff.User_id,
                    Joined_At = sessionStart,
                    Left_At = s.Ended_At
                });
                created++;

                var numCustomers = _rng.Next(1, 6);
                var pool = isActive
                    ? customers.Where(c => !activeAssigned.Contains(c.User_id)).ToList()
                    : customers;

                numCustomers = Math.Min(numCustomers, pool.Count);
                var chosen = pool.OrderBy(_ => _rng.Next()).Take(numCustomers).ToList();

                foreach (var c in chosen)
                {
                    if (isActive) activeAssigned.Add(c.User_id);

                    var joinTime = sessionStart.AddMinutes(_rng.Next(0, 31));
                    _context.SessionParticipants.Add(new SessionParticipant
                    {
                        Session_Id = s.Session_Id,
                        User_Id = c.User_id,
                        Joined_At = joinTime,
                        Left_At = s.Ended_At
                    });
                    created++;
                }

                //_context.SaveChanges();
                _logger.LogInformation($"SAVE CHANGES completed. User #: {_context.Users.Count()}");
            }

            var totalActive = _context.SessionParticipants.Count(p => p.Left_At == null);
            var totalCompleted = _context.SessionParticipants.Count(p => p.Left_At != null);
            _logger.LogInformation($"Created {created} session participants ({totalActive} active, {totalCompleted} completed)");
        }

    }
}