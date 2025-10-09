using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using back_end.Domain.Entitys;
using back_end.Domain.Enums;

namespace back_end.Domain.Seeders
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
                .Where(s => s.SessionId <= 50)
                .ToList();

            var customers = _context.Users
                .Where(u => u.Role == UserRole.Customer && u.UserId >= 5 && u.UserId <= 14)
                .ToList();

            int created = 0;

            foreach (var s in sessions)
            {
                var participantIds = _context.SessionParticipants
                    .Where(p => p.SessionId == s.SessionId && customers.Select(c => c.UserId).Contains(p.UserId))
                    .Select(p => p.UserId)
                    .ToList();

                if (!participantIds.Any()) continue;

                var tables = _context.SessionTables
                    .Where(st => st.SessionId == s.SessionId)
                    .Select(st => st.Table)
                    .ToList();

                var totalSeats = tables.Sum(t => t.SeatCount);
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
                        if (s.EndedAt != null) status = BillStatus.Closed;
                        else status = s.SessionId <= 5 ? BillStatus.Open
                                                       : (new[] { BillStatus.Closed, BillStatus.Cancelled })[_rng.Next(2)];

                        var tableNumbers = tables.Select(t => t.TableNumber.ToString());
                        var name = $"Table{string.Join(" & ", tableNumbers)} - Party of {total}";

                        var participant = _context.SessionParticipants
                            .FirstOrDefault(p => p.SessionId == s.SessionId && p.UserId == pid);

                        var createdAt = (participant?.JoinedAt ?? s.StartedAt).AddMinutes(_rng.Next(5, 31));

                        var bill = new Bill
                        {
                            SessionId = s.SessionId,
                            BillName = name,
                            SeniorCount = senior,
                            AdultCount = adult,
                            ChildCount = child,
                            TotCount = tot,
                            Status = status,
                            CreatedAt = createdAt,
                            ClosedAt = status == BillStatus.Closed ? s.EndedAt : null
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