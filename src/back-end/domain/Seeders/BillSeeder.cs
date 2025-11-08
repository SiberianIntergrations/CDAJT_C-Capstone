using System;
using System.Linq;
using System.Collections.Generic;
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

    private Dictionary<string, (string DisplayName, string GivenName, string Surname)> _userSeedData;

    public BillSeeder(ApplicationDbContext context, ILogger<BillSeeder> logger)
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
          .Include(s => s.Table)
          .Include(s => s.TableGroup)
              .ThenInclude(tg => tg.Tables)
          .Where(s => s.Session_Id <= 5)
          .ToList();

      int created = 0;
      var userUsageCount = new Dictionary<string, int>();

      foreach (var s in sessions)
      {
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
            continue;
          }

          var remaining = totalSeats;
          var userOids = _userSeedData.Keys.ToList();

          foreach (var userOid in userOids)
          {
            var numBills = _rng.Next(1, 4); // 1-3 bills per participant
            var info = _userSeedData[userOid];
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
                  .FirstOrDefault(p => p.Session_Id == s.Session_Id && p.User_Oid == userOid);

              var createdAt = (participant?.Joined_At ?? s.Started_At).AddMinutes(_rng.Next(5, 31));

              if (!userUsageCount.ContainsKey(userOid)) userUsageCount[userOid] = 0;
              userUsageCount[userOid]++;

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
                Closed_At = status == BillStatus.Closed ? s.Ended_At : null,
                // If Billing entity supports User_Oid/User_Name, set them here:
                // User_Oid = userOid,
                // User_Name = $"{info.GivenName} {info.Surname}".Trim()
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

      // Ensure every account is used at least twice
      foreach (var kvp in _userSeedData)
      {
          var oid = kvp.Key;
          var info = kvp.Value;
          var name = (!string.IsNullOrEmpty(info.GivenName) || !string.IsNullOrEmpty(info.Surname)
            ? $"{info.GivenName} {info.Surname}".Trim()
            : info.DisplayName);
          if (!userUsageCount.ContainsKey(oid) || userUsageCount[oid] < 2)
          {
            for (int i = userUsageCount.GetValueOrDefault(oid, 0); i < 2; i++)
            {
              var bill = new Billing
              {
                Session_Id = sessions.FirstOrDefault()?.Session_Id ?? 1,
                Bill_Name = $"Seeded Bill for {name}",
                Senior_Count = 0,
                Adult_Count = 1,
                Child_Count = 0,
                Total_Count = 0,
                Status = BillStatus.Open,
                Created_At = DateTime.UtcNow.AddMinutes(_rng.Next(1, 60)),
                Closed_At = null,
                // User_Oid = oid,
                // User_Name = name
              };
              _context.Bills.Add(bill);
              created++;
            }
          }
      }
    }
  }
}
