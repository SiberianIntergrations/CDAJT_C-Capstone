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
          // .Where(s => s.Session_Id <= 5)
          .Where(s => s.Ended_At == null)
          .OrderBy(s => s.Session_Id)
          .Take(10) // Limit to first 10 active sessions (5 per location)
          .ToList();
      
      if (!sessions.Any())
      {
        _logger.LogWarning("No active dining sessions found to create bills for.");
        return;
      }

      int created = 0;
      var userUsageCount = new Dictionary<string, int>();
      var userOids = _userSeedData.Keys.ToList();

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
          
        var numBillsForSession = _rng.Next(1, Math.Min(4, totalSeats + 1));

        for (int billIndex = 0; billIndex < numBillsForSession; billIndex++)
        {
          if (remaining <= 0) break;

          // Pick a random user as the bill owner
          var userOid = userOids[_rng.Next(userOids.Count)];
          var info = _userSeedData[userOid];

          var maxGuests = Math.Min(remaining, 6);
          var adult = _rng.Next(1, maxGuests + 1);
          var senior = _rng.Next(0, Math.Max(0, maxGuests - adult) + 1);
          var child = _rng.Next(0, Math.Max(0, maxGuests - adult - senior) + 1);
          var total = adult + senior + child;
          
          if (total == 0)
          {
              adult = 1;
              total = 1;
          }

          remaining -= total;

          BillStatus status;
          if (s.Ended_At != null) 
              status = BillStatus.Closed;
          else 
              status = s.Session_Id <= 5 ? BillStatus.Open
                                                : (new[] { BillStatus.Closed, BillStatus.Cancelled })[_rng.Next(2)];

          var tableNumbersStr = tableNumbers.Select(n => n.ToString());
          var billName = $"Table {string.Join(" & ", tableNumbersStr)} - Party of {total}";

          var participant = _context.SessionParticipants
              .FirstOrDefault(p => p.Session_Id == s.Session_Id && p.User_Oid == userOid);

          var createdAt = (participant?.Joined_At ?? s.Started_At).AddMinutes(_rng.Next(5, 31));

          // Track user usage
          if (!userUsageCount.ContainsKey(userOid))
            userUsageCount[userOid] = 0;
            userUsageCount[userOid]++;

            var bill = new Billing
            {
                Session_Id = s.Session_Id,
                Bill_Name = billName,
                Senior_Count = senior,
                Adult_Count = adult,
                Child_Count = child,
                Total_Count = total,
                Status = status,
                Created_At = createdAt,
                Closed_At = status == BillStatus.Closed ? s.Ended_At : null
            };

            _context.Bills.Add(bill);
            created++;
        }
      }
        var unusedUsers = _userSeedData.Keys.Where(oid => !userUsageCount.ContainsKey(oid)).ToList();
    
      if (unusedUsers.Any() && sessions.Any())
      {
        int sessionIndex = 0;
        
        foreach (var oid in unusedUsers)
        {
          // Rotate through sessions
          var session = sessions[sessionIndex % sessions.Count];
          sessionIndex++;
          
          var info = _userSeedData[oid];
          var name = (!string.IsNullOrEmpty(info.GivenName) || !string.IsNullOrEmpty(info.Surname)
              ? $"{info.GivenName} {info.Surname}".Trim()
              : info.DisplayName);

          var bill = new Billing
          {
              Session_Id = session.Session_Id,
              Bill_Name = $"Bill for {name}",
              Senior_Count = 0,
              Adult_Count = 1,
              Child_Count = 0,
              Total_Count = 1,
              Status = BillStatus.Open,
              Created_At = DateTime.UtcNow.AddMinutes(_rng.Next(1, 60)),
              Closed_At = null
          };
            
          _context.Bills.Add(bill);
          created++;
          }
        }

      var totalOpen = _context.Bills.Count(b => b.Status == BillStatus.Open);
      var totalClosed = _context.Bills.Count(b => b.Status == BillStatus.Closed);
      var totalCancelled = _context.Bills.Count(b => b.Status == BillStatus.Cancelled);
      _logger.LogInformation($"Created {created} bills ({totalOpen} open, {totalClosed} closed, {totalCancelled} cancelled)");
    }
  }
}