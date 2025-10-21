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

        public SessionOrderSeeder(ApplicationDbContext context, ILogger<SessionOrderSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Seed()
        {
            var bills = _context.Bills.ToList();
            if (!bills.Any()) throw new InvalidOperationException("No bills found. Seed bills first.");

            var customers = _context.Users.Where(u => u.Role == UserRoles.Customer).ToList();

            if (!customers.Any()) throw new InvalidOperationException("No customers found. Seed users first.");

            int created = 0;

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

                for (int i = 0; i < numWaves; i++)
                {
                    var customer = customers[_rng.Next(customers.Count)];

                    // Choose status based on allowed OrderStatus enums:
                    // Since your enum doesn't have Confirmed or Completed, I'll use Processing and Delivered instead.
                    OrderStatus status;

                    if (session.Session_Id <= 5) // Active sessions
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
                        User_Id = customer.User_id,
                        Status = status,
                        Created_At = time,
                        Completed_At = status == OrderStatus.Delivered ? time.AddMinutes(_rng.Next(15, 31)) : null
                    };

                    _context.SessionOrders.Add(order);
                    created++;

                    time = time.AddMinutes(_rng.Next(15, 46));
                }
            }

            //_context.SaveChanges();

            var pending = _context.SessionOrders.Count(o => o.Status == OrderStatus.Pending);
            var processing = _context.SessionOrders.Count(o => o.Status == OrderStatus.Processing);
            var delivered = _context.SessionOrders.Count(o => o.Status == OrderStatus.Delivered);

            _logger.LogInformation($"Created {created} session orders ({pending} pending, {processing} processing, {delivered} delivered)");
        }
    }
}
