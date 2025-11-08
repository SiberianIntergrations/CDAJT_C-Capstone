using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using back_end.domain.Entities;
using back_end.domain;
using back_end.domain.DbContexts;
using back_end.domain.enums;

namespace back_end.domain.Seeders
{
    public class SessionOrderSeeder : ISeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SessionOrderSeeder> _logger;
        private readonly Random _rng = new();

        private Dictionary<string, (string DisplayName, string GivenName, string Surname)> _userSeedData;

        public SessionOrderSeeder(ApplicationDbContext context, ILogger<SessionOrderSeeder> logger)
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
            var bills = _context.Bills.ToList();
            if (!bills.Any()) throw new InvalidOperationException("No bills found. Seed bills first.");

            int created = 0;
            var userUsageCount = new Dictionary<string, int>();

            foreach (var bill in bills)
            {
                var session = _context.DiningSessions.FirstOrDefault(s => s.Session_Id == bill.Session_Id);
                if (session == null) continue;

                (int min, int max) waves = bill.Status switch
                {
                    BillStatus.Open       => (2, 4),
                    BillStatus.Closed     => (3, 5),
                    BillStatus.Cancelled  => (1, 2),
                    _ => (2, 4)
                };

                var numWaves = _rng.Next(waves.min, waves.max + 1);
                var time = session.Started_At.AddMinutes(_rng.Next(5, 16));

                var userOids = _userSeedData.Keys.ToList();

                for (int i = 0; i < numWaves; i++)
                {
                    var userOid = userOids[_rng.Next(userOids.Count)];
                    var userInfo = _userSeedData[userOid];
                    if (!userUsageCount.ContainsKey(userOid)) userUsageCount[userOid] = 0;
                    userUsageCount[userOid]++;

                    OrderStatus status;
                    if (session.Session_Id <= 5)
                    {
                        status = (i == numWaves - 1)
                            ? new[] { OrderStatus.Pending, OrderStatus.Processing, OrderStatus.Delivered }[_rng.Next(3)]
                            : OrderStatus.Delivered;
                    }
                    else status = OrderStatus.Delivered;

                    var order = new SessionOrder
                    {
                        session_id = bill.Session_Id,
                        Bill_Id = bill.Bill_Id,
                        User_Oid = userOid,
                        User_Name = $"{userInfo.GivenName} {userInfo.Surname}".Trim(),
                        Status = status,
                        Created_At = time,
                        Completed_At = status == OrderStatus.Delivered ? time.AddMinutes(_rng.Next(15, 31)) : null
                    };

                    _context.SessionOrders.Add(order);
                    created++;
                    time = time.AddMinutes(_rng.Next(15, 46));
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
                        var order = new SessionOrder
                        {
                            session_id = bills.FirstOrDefault()?.Session_Id ?? 1,
                            Bill_Id = bills.FirstOrDefault()?.Bill_Id ?? 1,
                            User_Oid = oid,
                            User_Name = $"{info.GivenName} {info.Surname}".Trim(),
                            Status = OrderStatus.Pending,
                            Created_At = DateTime.UtcNow.AddMinutes(_rng.Next(1, 60)),
                            Completed_At = null
                        };
                        _context.SessionOrders.Add(order);
                        created++;
                    }
                }
            }

            var pending = _context.SessionOrders.Count(o => o.Status == OrderStatus.Pending);
            var processing = _context.SessionOrders.Count(o => o.Status == OrderStatus.Processing);
            var delivered = _context.SessionOrders.Count(o => o.Status == OrderStatus.Delivered);

            _logger.LogInformation($"Created {created} session orders ({pending} pending, {processing} processing, {delivered} delivered)");
        }
    }
}
