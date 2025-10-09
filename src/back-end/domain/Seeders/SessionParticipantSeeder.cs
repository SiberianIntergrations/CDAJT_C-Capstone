using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using back_end.domain.Entities;
using back_end.domain.Enums;

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
                .Where(s => s.SessionId >= 1 && s.SessionId <= 50)
                .ToList();

            if (!sessions.Any())
                throw new InvalidOperationException("No dining sessions found. Seed sessions first.");

            var customers = _context.Users
                .Where(u => u.Role == UserRole.Customer && u.UserId >= 5 && u.UserId <= 14)
                .ToList();
            var staff = _context.Users
                .Where(u => u.Role == UserRole.Staff && u.UserId >= 2 && u.UserId <= 4)
                .ToList();

            if (!customers.Any() || !staff.Any())
                throw new InvalidOperationException("Not enough users found. Seed users first.");

            var activeAssigned = new System.Collections.Generic.HashSet<int>();
            int created = 0;

            foreach (var s in sessions)
            {
                var sessionStart = s.StartedAt;
                var isActive = s.EndedAt == null;

                // Reset reuse tracking for ended sessions
                if (!isActive) activeAssigned.Clear();

                // staff (1 per session)
                var sessionStaff = staff[_rng.Next(staff.Count)];
                _context.SessionParticipants.Add(new SessionParticipant
                {
                    SessionId = s.SessionId,
                    UserId = sessionStaff.UserId,
                    JoinedAt = sessionStart,
                    LeftAt = s.EndedAt
                });
                created++;

                // customers 1–5
                var numCustomers = _rng.Next(1, 6);
                var pool = isActive
                    ? customers.Where(c => !activeAssigned.Contains(c.UserId)).ToList()
                    : customers;

                numCustomers = Math.Min(numCustomers, pool.Count);
                var chosen = pool.OrderBy(_ => _rng.Next()).Take(numCustomers).ToList();

                foreach (var c in chosen)
                {
                    if (isActive) activeAssigned.Add(c.UserId);

                    var joinTime = sessionStart.AddMinutes(_rng.Next(0, 31));
                    _context.SessionParticipants.Add(new SessionParticipant
                    {
                        SessionId = s.SessionId,
                        UserId = c.UserId,
                        JoinedAt = joinTime,
                        LeftAt = s.EndedAt
                    });
                    created++;
                }

                _context.SaveChanges(); // commit per session to keep tx small
            }

            var totalActive = _context.SessionParticipants.Count(p => p.LeftAt == null);
            var totalCompleted = _context.SessionParticipants.Count(p => p.LeftAt != null);
            _logger.LogInformation($"Created {created} session participants ({totalActive} active, {totalCompleted} completed)");
        }
    }
}