using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using back_end.domain.Entities;
using back_end.domain.Enums;

namespace back_end.domain.Seeders
{
    /// <summary>
    /// Seeds SessionOrder waves per bill, status by session/bill state.
    /// </summary>
    public class SessionOrderSeeder : ISeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SessionOrderSeeder> _logger;
        private readonly Random _rng = new();

        public SessionOrderSeeder(ApplicationDbContext context, ILogger<SessionOrderSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Seed()
        {
            var bills = _context.Bills.ToList();
            if (!bills.Any()) throw new InvalidOperationException("No bills found. Seed bills first.");

            var customers = _context.Users
                .Where(u => u.Role == UserRole.Customer && u.UserId >= 5 && u.UserId <= 14)
                .ToList();
            if (!customers.Any()) throw new InvalidOperationException("No customers found. Seed users first.");

            int created = 0;

            foreach (var bill in bills)
            {
                var session = _context.DiningSessions.FirstOrDefault(s => s.SessionId == bill.SessionId);
                if (session == null) continue;

                (int min, int max) waves = bill.Status switch
                {
                    BillStatus.Open       => (2, 4),
                    BillStatus.Closed     => (3, 5),
                    BillStatus.Cancelled  => (1, 2),
                    _ => (2, 4)
                };

                var numWaves = _rng.Next(waves.min, waves.max + 1);
                var time = session.StartedAt.AddMinutes(_rng.Next(5, 16));

                for (int i = 0; i < numWaves; i++)
                {
                    var customer = customers[_rng.Next(customers.Count)];

                    OrderStatus status;
                    if (session.SessionId <= 5) // Active sessions
                    {
                        status = (i == numWaves - 1)
                            ? new[] { OrderStatus.Pending, OrderStatus.Confirmed, OrderStatus.Completed }[_rng.Next(3)]
                            : OrderStatus.Completed;
                    }
                    else status = OrderStatus.Completed;

                    var order = new SessionOrder
                    {
                        SessionId = bill.SessionId,
                        BillId = bill.BillId,
                        UserId = customer.UserId,
                        Status = status,
                        CreatedAt = time,
                        CompletedAt = status == OrderStatus.Completed ? time.AddMinutes(_rng.Next(15, 31)) : null
                    };

                    _context.SessionOrders.Add(order);
                    created++;

                    time = time.AddMinutes(_rng.Next(15, 46));
                }
            }

            _context.SaveChanges();

            var pending = _context.SessionOrders.Count(o => o.Status == OrderStatus.Pending);
            var confirmed = _context.SessionOrders.Count(o => o.Status == OrderStatus.Confirmed);
            var completed = _context.SessionOrders.Count(o => o.Status == OrderStatus.Completed);

            _logger.LogInformation($"Created {created} session orders ({pending} pending, {confirmed} confirmed, {completed} completed)");
        }
    }
}