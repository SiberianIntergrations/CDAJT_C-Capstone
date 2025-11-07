using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using back_end.domain.Entities;
using back_end.domain;
using back_end.domain.DbContexts;
using back_end.domain.enums;

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

        private Dictionary<string, (string DisplayName, string GivenName, string Surname)> _userSeedData;

        public SessionParticipantSeeder(ApplicationDbContext context, ILogger<SessionParticipantSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void SetUserSeedData(Dictionary<string, (string DisplayName, string GivenName, string Surname)> userSeedData)
        {
            _userSeedData = userSeedData;
        }

        public void Seed()
        {
            var sessions = _context.DiningSessions
                .OrderByDescending(s => s.Started_At)
                .Take(50)
                .ToList();

            if (!sessions.Any())
                throw new InvalidOperationException("No dining sessions found. Seed sessions first.");

            var userUsageCount = new Dictionary<string, int>();
            int created = 0;

            foreach (var s in sessions)
            {
                var sessionStart = s.Started_At;
                var isActive = s.Ended_At == null;

                // Pick a random staff from CSV
                var staffOids = _userSeedData.Keys.ToList();
                var staffOid = staffOids[_rng.Next(staffOids.Count)];
                var staffInfo = _userSeedData[staffOid];
                if (!userUsageCount.ContainsKey(staffOid)) userUsageCount[staffOid] = 0;
                userUsageCount[staffOid]++;
                _context.SessionParticipants.Add(new SessionParticipant
                {
                    Session_Id = s.Session_Id,
                    User_Oid = staffOid,
                    User_Name = $"{staffInfo.GivenName} {staffInfo.Surname}".Trim(),
                    Joined_At = sessionStart,
                    Left_At = s.Ended_At
                });
                created++;

                var numCustomers = _rng.Next(1, 6);
                var customerOids = staffOids.Where(oid => oid != staffOid).OrderBy(_ => _rng.Next()).Take(numCustomers).ToList();

                foreach (var customerOid in customerOids)
                {
                    var customerInfo = _userSeedData[customerOid];
                    if (!userUsageCount.ContainsKey(customerOid)) userUsageCount[customerOid] = 0;
                    userUsageCount[customerOid]++;
                    var joinTime = sessionStart.AddMinutes(_rng.Next(0, 31));
                    _context.SessionParticipants.Add(new SessionParticipant
                    {
                        Session_Id = s.Session_Id,
                        User_Oid = customerOid,
                        User_Name = $"{customerInfo.GivenName} {customerInfo.Surname}".Trim(),
                        Joined_At = joinTime,
                        Left_At = s.Ended_At
                    });
                    created++;
                }
            }

            // Ensure every account is used at least twice
            foreach (var kvp in _userSeedData)
            {
                var oid = kvp.Key;
                var info = kvp.Value;
                if (!userUsageCount.ContainsKey(oid) || userUsageCount[oid] < 2)
                {
                    for (int i = userUsageCount.GetValueOrDefault(oid, 0); i < 2; i++)
                    {
                        _context.SessionParticipants.Add(new SessionParticipant
                        {
                            Session_Id = sessions.FirstOrDefault()?.Session_Id ?? 1,
                            User_Oid = oid,
                            User_Name = $"{info.GivenName} {info.Surname}".Trim(),
                            Joined_At = DateTime.UtcNow.AddMinutes(_rng.Next(1, 60)),
                            Left_At = null
                        });
                        created++;
                    }
                }
            }

            var totalActive = _context.SessionParticipants.Count(p => p.Left_At == null);
            var totalCompleted = _context.SessionParticipants.Count(p => p.Left_At != null);
            _logger.LogInformation($"Created {created} session participants ({totalActive} active, {totalCompleted} completed)");
        }

    }
}