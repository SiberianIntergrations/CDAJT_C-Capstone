using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using back_end.domain.Entities;
using back_end.domain.DbContexts;
using back_end.domain.enums;

namespace back_end.domain.Seeders
{
    /// <summary>
    /// Seeder for the service_request table with initial data.
    /// </summary>
    public class ServiceRequestSeeder : ISeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ServiceRequestSeeder> _logger;
        private Dictionary<string, (string DisplayName, string GivenName, string Surname)> _userSeedData;
        private readonly Random _rng = new(); // Added missing Random field

        public ServiceRequestSeeder(ApplicationDbContext context, ILogger<ServiceRequestSeeder> logger)
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
            var diningSession = _context.DiningSessions.FirstOrDefault();
            var table = _context.Tables.FirstOrDefault();

            if (diningSession == null || table == null)
            {
                _logger.LogWarning("Cannot seed ServiceRequest - missing DiningSession or TableEntity");
                return;
            }

            var userOids = _userSeedData.Keys.ToList();
            var userUsageCount = new Dictionary<string, int>();

            var serviceRequests = new List<ServiceRequest>();

            for (int i = 0; i < 3; i++)
            {
                var requesterOid = userOids[_rng.Next(userOids.Count)];
                var requesterInfo = _userSeedData[requesterOid];
                var claimerOid = userOids.Where(oid => oid != requesterOid).OrderBy(_ => _rng.Next()).FirstOrDefault();
                var claimerInfo = claimerOid != null && _userSeedData.ContainsKey(claimerOid) ? _userSeedData[claimerOid] : default;

                serviceRequests.Add(new ServiceRequest
                {
                    Session_Id = diningSession.Session_Id,
                    Table_Id = table.Table_Id,
                    Request_By_Oid = requesterOid,
                    Request_By_Name = $"{requesterInfo.GivenName} {requesterInfo.Surname}".Trim(),
                    Claimed_By_Oid = claimerOid,
                    Claimed_By_Name = claimerOid != null ? $"{claimerInfo.GivenName} {claimerInfo.Surname}".Trim() : null,
                    Notes = i == 0 ? "Need extra napkins" : i == 1 ? "Requesting water refill" : "Bill requested",
                    Status = i == 0 ? ServiceRequestStatus.Pending : i == 1 ? ServiceRequestStatus.Claimed : ServiceRequestStatus.Completed,
                    Created_At = DateTime.Now.AddMinutes(-i * 15),
                    Claimed_At = i == 1 ? DateTime.Now.AddMinutes(-10) : (i == 2 ? DateTime.Now.AddHours(-1).AddMinutes(-30) : null),
                    Completed_At = i == 2 ? DateTime.Now.AddHours(-1) : null
                });

                if (!userUsageCount.ContainsKey(requesterOid)) userUsageCount[requesterOid] = 0;
                userUsageCount[requesterOid]++;
                if (claimerOid != null)
                {
                    if (!userUsageCount.ContainsKey(claimerOid)) userUsageCount[claimerOid] = 0;
                    userUsageCount[claimerOid]++;
                }
            }

            _context.ServiceRequests.AddRange(serviceRequests);

            // Ensure every account is used at least twice
            foreach (var kvp in _userSeedData)
            {
                var oid = kvp.Key;
                var info = kvp.Value;
                if (!userUsageCount.ContainsKey(oid) || userUsageCount[oid] < 2)
                {
                    for (int i = userUsageCount.GetValueOrDefault(oid, 0); i < 2; i++)
                    {
                        var req = new ServiceRequest
                        {
                            Session_Id = diningSession?.Session_Id ?? 1,
                            Table_Id = table?.Table_Id ?? 1,
                            Request_By_Oid = oid,
                            Request_By_Name = $"{info.GivenName} {info.Surname}".Trim(),
                            Claimed_By_Oid = null,
                            Claimed_By_Name = null,
                            Notes = $"Seeded request for {info.DisplayName}",
                            Status = ServiceRequestStatus.Pending,
                            Created_At = DateTime.UtcNow.AddMinutes(_rng.Next(1, 60)),
                            Claimed_At = null,
                            Completed_At = null
                        };
                        _context.ServiceRequests.Add(req);
                    }
                }
            }

            _logger.LogInformation($"Added {serviceRequests.Count} service requests");
        }
    }
}
