using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using back_end.domain.Entities;
using back_end.domain;
using back_end.domain.DbContexts;
using back_end.domain.enums;
using Microsoft.EntityFrameworkCore;

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
          .Include(s => s.Table)
          .Include(s => s.TableGroup)
              .ThenInclude(tg => tg.Tables)
          .Where(s => s.Session_Id <= 5)
          .ToList();

      var customers = _context.Users
          .Where(u => u.Role == UserRoles.Customer)
          .ToList();

      int created = 0;

      foreach (var s in sessions)
      {
          var customerIds = customers.Select(c => c.User_id).ToList();
          
          var participantIds = _context.SessionParticipants
              .Where(p => p.Session_Id == s.Session_Id 
                  && p.User_Id.HasValue 
                  && customerIds.Contains(p.User_Id.Value))
              .Select(p => p.User_Id.Value)
              .ToList();

        if (!participantIds.Any()) continue;

        // Get total seats from either individual table or table group
        int totalSeats;
        List<int> tableNumbers;

        if (s.Table_Id.HasValue && s.Table != null)
        {
          totalSeats = s.Table.seat_count;
          tableNumbers = new List<int> { s.Table.table_number };
        }
        else if (s.TableGroup_Id.HasValue && s.TableGroup != null)
        {
          totalSeats = s.TableGroup.Tables.Sum(t => t.seat_count);
          tableNumbers = s.TableGroup.Tables.Select(t => t.table_number).ToList();
        }
        else
        {
          continue; // Skip sessions without table assignment
        }

        var remaining = totalSeats;

        foreach (var pid in participantIds)
        {
          var numBills = _rng.Next(1, 4); // 1-3 bills per participant
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

            var tableNumbersStr = tableNumbers.Select(n => n.ToString());
            var name = $"Table{string.Join(" & ", tableNumbersStr)} - Party of {total}";

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

      var totalOpen = _context.Bills.Count(b => b.Status == BillStatus.Open);
      var totalClosed = _context.Bills.Count(b => b.Status == BillStatus.Closed);
      var totalCancelled = _context.Bills.Count(b => b.Status == BillStatus.Cancelled);
      _logger.LogInformation($"Created {created} bills ({totalOpen} open, {totalClosed} closed, {totalCancelled} cancelled)");
    }
  }
}
