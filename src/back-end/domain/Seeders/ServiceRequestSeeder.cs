using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using back_end.domain.Entities;
using back_end.domain.DbContexts;
using back_end.domain.enums;
using Microsoft.EntityFrameworkCore;

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

        private readonly string[] _requestNotes = new[]
        {
            "Need water refill",
            "Need napkins",
            "Need utensils",
            "Need soy sauce",
            "Need wasabi",
            "Need ginger",
            "Ready for bill",
        };

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
            var activeSessions = _context.DiningSessions
                .Include(s => s.Table)
                .Include(s => s.TableGroup)
                    .ThenInclude(tg => tg.Tables)
                .Include(s => s.Participants)
                .Where(s => s.Ended_At == null)
                .ToList();

            if (!activeSessions.Any())
            {
                _logger.LogWarning("Cannot seed ServiceRequest - Missing DiningSession or Tables.");
                return;
            }

            var userOids = _userSeedData.Keys.ToList();
            var userUsageCount = new Dictionary<string, int>();
            var serviceRequests = new List<ServiceRequest>();
            int totalCreated = 0;

            // Create 0-3 service requests per active session
            foreach (var session in activeSessions)
            {
                // Determine which table(s) this session uses
                List<int> tableIds = new List<int>();
                
                if (session.Table_Id.HasValue)
                {
                    tableIds.Add(session.Table_Id.Value);
                }
                else if (session.TableGroup_Id.HasValue && session.TableGroup?.Tables != null)
                {
                    tableIds.AddRange(session.TableGroup.Tables.Select(t => t.Table_Id));
                }

                if (!tableIds.Any())
                {
                    _logger.LogWarning($"Session {session.Session_Id} has no tables, skipping");
                    continue;
                }

                // Get participants in this session to use as requesters
                var sessionParticipants = session.Participants
                    .Where(p => p.Left_At == null)
                    .Select(p => p.User_Oid)
                    .ToList();

                if (!sessionParticipants.Any())
                {
                    _logger.LogWarning($"Session {session.Session_Id} has no active participants, skipping");
                    continue;
                }

                // Create 0-3 requests for this session
                int numRequests = _rng.Next(0, 4);

                for (int i = 0; i < numRequests; i++)
                {
                    // Pick a random participant from THIS session as requester
                    var requesterOid = sessionParticipants[_rng.Next(sessionParticipants.Count)];
                    var requesterInfo = _userSeedData.ContainsKey(requesterOid) 
                        ? _userSeedData[requesterOid] 
                        : (DisplayName: "Unknown", GivenName: "Unknown", Surname: "User");

                    // Pick a different user (from any user) as potential claimer
                    var claimerOid = userOids.Where(oid => oid != requesterOid).OrderBy(_ => _rng.Next()).FirstOrDefault();
                    var claimerInfo = claimerOid != null && _userSeedData.ContainsKey(claimerOid) 
                        ? _userSeedData[claimerOid] 
                        : default;

                    var note = _requestNotes[_rng.Next(_requestNotes.Length)];

                    // Pick a random table from this session's tables
                    var tableId = tableIds[_rng.Next(tableIds.Count)];

                    var statusRoll = _rng.Next(100);
                    ServiceRequestStatus status;
                    DateTime? claimedAt;
                    DateTime? completedAt;
                    string actualClaimerOid;
                    string actualClaimerName;

                    if (statusRoll < 40) // 40% Pending
                    {
                        status = ServiceRequestStatus.Pending;
                        claimedAt = null;
                        completedAt = null;
                        actualClaimerOid = null;
                        actualClaimerName = null;
                    }
                    else if (statusRoll < 70) // 30% Claimed
                    {
                        status = ServiceRequestStatus.Claimed;
                        claimedAt = DateTime.UtcNow.AddMinutes(_rng.Next(-60, -5));
                        completedAt = null;
                        actualClaimerOid = claimerOid;
                        actualClaimerName = claimerOid != null ? $"{claimerInfo.GivenName} {claimerInfo.Surname}".Trim() : null;
                    }
                    else // 30% Completed
                    {
                        status = ServiceRequestStatus.Completed;
                        claimedAt = DateTime.UtcNow.AddMinutes(_rng.Next(-120, -60));
                        completedAt = claimedAt?.AddMinutes(_rng.Next(5, 30));
                        actualClaimerOid = claimerOid;
                        actualClaimerName = claimerOid != null ? $"{claimerInfo.GivenName} {claimerInfo.Surname}".Trim() : null;
                    }

                    serviceRequests.Add(new ServiceRequest
                    {
                        Session_Id = session.Session_Id,
                        Table_Id = tableId,
                        Request_By_Oid = requesterOid,
                        Request_By_Name = $"{requesterInfo.GivenName} {requesterInfo.Surname}".Trim(),
                        Claimed_By_Oid = actualClaimerOid,
                        Claimed_By_Name = actualClaimerName,
                        Notes = note,
                        Status = status,
                        Created_At = DateTime.UtcNow.AddMinutes(_rng.Next(-180, -1)),
                        Claimed_At = claimedAt,
                        Completed_At = completedAt
                    });

                    if (!userUsageCount.ContainsKey(requesterOid)) 
                        userUsageCount[requesterOid] = 0;
                    userUsageCount[requesterOid]++;
                    
                    if (actualClaimerOid != null)
                    {
                        if (!userUsageCount.ContainsKey(actualClaimerOid)) 
                            userUsageCount[actualClaimerOid] = 0;
                        userUsageCount[actualClaimerOid]++;
                    }

                    totalCreated++;
                }
            }

             _context.ServiceRequests.AddRange(serviceRequests);
            
            var totalPending = serviceRequests.Count(sr => sr.Status == ServiceRequestStatus.Pending);
            var totalClaimed = serviceRequests.Count(sr => sr.Status == ServiceRequestStatus.Claimed);
            var totalCompleted = serviceRequests.Count(sr => sr.Status == ServiceRequestStatus.Completed);
            
            _logger.LogInformation($"Created {totalCreated} service requests across {activeSessions.Count} sessions ({totalPending} pending, {totalClaimed} claimed, {totalCompleted} completed)");
        }
    }
}