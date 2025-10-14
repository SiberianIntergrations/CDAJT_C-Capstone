using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using back_end.domain.Entities;
using back_end.domain;
using back_end.domain.DbContexts;

namespace back_end.domain.Seeders
{
    /// <summary>
    /// Seeds bills with guest counts/status, tied to assigned tables and participants.
    /// </summary>
    public class BillSeeder : ISeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BillSeeder> _logger;
        private readonly Random _rng = new();

        public BillSeeder(ApplicationDbContext context, ILogger<BillSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Seed()
        {
            var sessions = _context.DiningSessions
                .Where(s => s.Session_Id <= 50)
                .ToList();

            var customers = _context.Users
                .Where(u => u.Role == UserRoles.Customer && u.User_id >= 5 && u.User_id <= 14)
                .ToList();

            int created = 0;

            foreach (var s in sessions)
            {
                var participantIds = _context.SessionParticipants
                    .Where(p => p.Session_Id == s.Session_Id && customers.Select(c => c.User_id).Contains(p.User_Id))
                    .Select(p => p.User_Id)
                    .ToList();

                if (!participantIds.Any()) continue;

                var tables = _context.SessionTables
                    .Where(st => st.Session_Id == s.Session_Id)
                    .Select(st => st.Table)
                    .ToList();

                var totalSeats = tables.Sum(t => t.Seat_Count);
                var remaining = totalSeats;

                foreach (var pid in participantIds)
                {
                    var numBills = _rng.Next(1, 4); // 1–3
                    for (int i = 0; i < numBills; i++)
                    {
                        if (remaining <= 0) break;

                        var maxGuests = Math.Min(remaining, 6);
                        var adult = _rng.Next(1, maxGuests + 1);
                        var senior = _rng.Next(0, Math.Max(0, maxGuests - adult) + 1);
                        var child = _rng.Next(0, Math.Max(0, maxGuests - adult - senior) + 1);
                        var tot = _rng.Next(0, Math.Max(0, maxGuests - adult - senior - child) + 1);
                        var total = adult + senior + child + tot;
                        if (total == 0) continue;

                        remaining -= total;

                        BillStatus status;
                        if (s.Ended_At != null) status = BillStatus.Closed;
                        else status = s.Session_Id <= 5 ? BillStatus.Open
                                                       : (new[] { BillStatus.Closed, BillStatus.Cancelled })[_rng.Next(2)];

                        var tableNumbers = tables.Select(t => t.Table_Number.ToString());
                        var name = $"Table{string.Join(" & ", tableNumbers)} - Party of {total}";

                        var participant = _context.SessionParticipants
                            .FirstOrDefault(p => p.Session_Id == s.Session_Id && p.User_Id == pid);

                        var createdAt = (participant?.Joined_At ?? s.Started_At).AddMinutes(_rng.Next(5, 31));

                        var bill = new Billing
                        {
                            Session_Id = s.Session_Id,
                            Bill_Name = name,
                            Senior_Count = senior,
                            Adult_Count = adult,
                            Child_Count = child,
                            Total_Count = tot,
                            Status = status,
                            Created_At = createdAt,
                            Closed_At = status == BillStatus.Closed ? s.Ended_At : null
                        };

                        _context.Bills.Add(bill);
                        created++;
                    }
                }
            }

            _context.SaveChanges();

            var totalOpen = _context.Bills.Count(b => b.Status == BillStatus.Open);
            var totalClosed = _context.Bills.Count(b => b.Status == BillStatus.Closed);
            var totalCancelled = _context.Bills.Count(b => b.Status == BillStatus.Cancelled);
            _logger.LogInformation($"Created {created} bills ({totalOpen} open, {totalClosed} closed, {totalCancelled} cancelled)");
        }
    }
}