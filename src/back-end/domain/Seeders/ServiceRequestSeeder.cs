using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using back_end.Domain.Entitys;
using back_end.Domain.Enums;

namespace back_end.Domain.Seeders
{
    /// <summary>
    /// Seeds ServiceRequest per session participants and tables, with realistic timing/status.
    /// </summary>
    public class ServiceRequestSeeder : ISeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ServiceRequestSeeder> _logger;
        private readonly Random _rng = new();

        public ServiceRequestSeeder(ApplicationDbContext context, ILogger<ServiceRequestSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Seed()
        {
            var sessions = _context.DiningSessions.Where(s => s.SessionId <= 50).ToList();
            if (!sessions.Any()) throw new InvalidOperationException("No dining sessions found. Seed sessions first.");

            var staff = _context.Users.Where(u => u.Role == UserRole.Staff && u.UserId >= 2 && u.UserId <= 4).ToList();
            var customers = _context.Users.Where(u => u.Role == UserRole.Customer && u.UserId >= 5 && u.UserId <= 14).ToList();

            int created = 0;

            var common = new[]
            {
                "Need more water","Request chopsticks","Need napkins","Request soy sauce","Need wasabi",
                "Request ginger","Need spoons","Request check","Clean plates please","Need assistance"
            };

            foreach (var s in sessions)
            {
                var tables = _context.SessionTables.Where(st => st.SessionId == s.SessionId).Select(st => st.Table).ToList();
                if (!tables.Any()) continue;

                var sessionParticipants = _context.SessionParticipants
                    .Where(p => p.SessionId == s.SessionId && customers.Select(c => c.UserId).Contains(p.UserId))
                    .ToList();

                foreach (var p in sessionParticipants)
                {
                    var joined = EnsureUtc(p.JoinedAt);
                    var left = EnsureUtc(p.LeftAt);
                    var ended = EnsureUtc(s.EndedAt);
                    var now = DateTime.UtcNow;

                    int numReq = _rng.Next(0, 4); // 0–3

                    for (int i = 0; i < numReq; i++)
                    {
                        var sessionEnd = ended ?? now;
                        var maxTime = left ?? sessionEnd;
                        var durMin = Math.Max(5, (int)(maxTime - joined).TotalMinutes);
                        var createdAt = joined.AddMinutes(_rng.Next(5, durMin + 1));

                        var table = tables[_rng.Next(tables.Count)];

                        ServiceRequestStatus status;
                        if (s.SessionId <= 5) // active
                            status = new[] { ServiceRequestStatus.Pending, ServiceRequestStatus.Claimed, ServiceRequestStatus.Completed }[_rng.Next(3)];
                        else
                            status = ServiceRequestStatus.Completed;

                        int? claimedBy = null;
                        DateTime? claimedAt = null;
                        DateTime? completedAt = null;

                        if (status is ServiceRequestStatus.Claimed or ServiceRequestStatus.Completed)
                        {
                            claimedBy = staff[_rng.Next(staff.Count)].UserId;
                            claimedAt = createdAt.AddMinutes(_rng.Next(1, 6));
                            if (status == ServiceRequestStatus.Completed)
                                completedAt = claimedAt.Value.AddMinutes(_rng.Next(2, 11));
                        }

                        var req = new ServiceRequest
                        {
                            SessionId = s.SessionId,
                            TableId = table.TableId,
                            RequestedBy = p.UserId,
                            ClaimedBy = claimedBy,
                            Notes = common[_rng.Next(common.Length)],
                            Status = status,
                            CreatedAt = createdAt,
                            ClaimedAt = claimedAt,
                            CompletedAt = completedAt
                        };

                        _context.ServiceRequests.Add(req);
                        created++;

                        if (created % 100 == 0) _context.SaveChanges();
                    }
                }
            }

            _context.SaveChanges();

            var pending = _context.ServiceRequests.Count(r => r.Status == ServiceRequestStatus.Pending);
            var claimed = _context.ServiceRequests.Count(r => r.Status == ServiceRequestStatus.Claimed);
            var completed = _context.ServiceRequests.Count(r => r.Status == ServiceRequestStatus.Completed);

            _logger.LogInformation($"Created {created} service requests ({pending} pending, {claimed} claimed, {completed} completed)");
        }

        private static DateTime EnsureUtc(DateTime? dt) => (dt?.Kind ?? DateTimeKind.Utc) == DateTimeKind.Utc
            ? (dt ?? DateTime.UtcNow)
            : DateTime.SpecifyKind(dt ?? DateTime.UtcNow, DateTimeKind.Utc);
    }
}