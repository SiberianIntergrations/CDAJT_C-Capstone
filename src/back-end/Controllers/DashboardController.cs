using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.DTO.Auth;
using Microsoft.AspNetCore.Mvc;
using back_end.DTO.Analytics;
using back_end.domain.Entities;
using back_end.domain;
using back_end.domain.enums;
using back_end.DTO.DashBoardDTOs;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Security.Cryptography;

namespace back_end.controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class DashBoardController : Controller
  {
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;
    private readonly ILogger<DashBoardController> _logger;

    public DashBoardController(ApplicationDbContext context, IConfiguration config, ILogger<DashBoardController> logger)
    {
      _context = context;
      _config = config;
      _logger = logger;
    }

    // Helper method to get table numbers from session
    private List<int> GetTableNumbers(DiningSession session)
    {
      if (session.Table != null)
      {
        return new List<int> { session.Table.table_number };
      }
      else if (session.TableGroup != null && session.TableGroup.Tables != null)
      {
        return session.TableGroup.Tables.Select(t => t.table_number).ToList();
      }
      return new List<int>();
    }

    // Helper method to count tables in session
    private int GetTableCount(DiningSession session)
    {
      if (session.Table_Id.HasValue)
      {
        return 1;
      }
      else if (session.TableGroup_Id.HasValue && session.TableGroup != null)
      {
        return session.TableGroup.Tables?.Count() ?? 0;
      }
      return 0;
    }

    /// <summary>
    /// Retrieves detailed information about all active dining sessions for dashboard display.
    /// </summary>
    /// <returns>
    /// An <see cref="IActionResult"/> containing a collection of <see cref="DashBoardSessionsDTO"/> objects.
    /// Returns HTTP 200 (OK) with the list of active sessions on success.
    /// Returns HTTP 404 (Not Found) if no active sessions exist.
    /// Returns HTTP 500 (Internal Server Error) if an exception occurs during retrieval.
    /// </returns>
    /// <response code="200">Returns the list of active dining sessions with detailed information</response>
    /// <response code="404">If no active dining sessions are found</response>
    /// <response code="500">If an internal error occurs while retrieving sessions</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/diningsession/sessions
    ///
    /// Returns comprehensive information about each active session including:
    /// - Session metadata (ID, menu, timestamps)
    /// - Table numbers
    /// - Active participant count
    /// - Associated bills with details
    /// - Whether the session can be closed (no open bills)
    /// 
    /// Sessions are ordered by start time (newest first).
    /// </remarks>
    [HttpGet("sessions")]
    public async Task<IActionResult> GetDashBoardSessions()
    {
      try
      {
        var sessions = await _context.DiningSessions
            .Include(ds => ds.Table)
            .Include(ds => ds.TableGroup)
                .ThenInclude(tg => tg.Tables)
            .Include(ds => ds.Bills)
            .Where(ds => ds.Ended_At == null)
            .OrderByDescending(ds => ds.Started_At)
            .ToListAsync();

        if (sessions == null || sessions.Count == 0)
        {
          return NotFound(new { message = "No active dining sessions found." });
        }

        var dashBoardSessions = new List<object>();
        foreach (var session in sessions)
        {
          bool has_open_bill = session.Bills.Any(b => b.Status == BillStatus.Open);
          bool Is_Closable = !has_open_bill;

          var active_participants = await _context.SessionParticipants
              .Where(sp => sp.Session_Id == session.Session_Id && sp.Left_At == null)
              .CountAsync();

          var dashBoard_session = new DashBoardSessionsDTO
          {
            Session_Id = session.Session_Id,
            Menu_Id = session.Menu_Id,
            Started_At = session.Started_At,
            Ended_At = session.Ended_At,
            First_Order_At = session.First_Order_At,
            Table_Numbers = GetTableNumbers(session),
            Active_Participants = active_participants,
            Bills = session.Bills.Select(b => new DashBoardBillDTO
            {
              Bill_Id = b.Bill_Id,
              Bill_Name = b.Bill_Name,
              Status = b.Status,
              Created_At = b.Created_At,
              Total_Guests = b.Total_Count
            }).ToList(),
            Bill_Count = session.Bills.Count(),
            Is_Closable = Is_Closable
          };
          dashBoardSessions.Add(dashBoard_session);
        }
        return Ok(dashBoardSessions);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
      }
    }


    /// <summary>
    /// Retrieves dashboard summary statistics for active dining sessions.
    /// </summary>
    /// <returns>
    /// An <see cref="IActionResult"/> containing a <see cref="DashBoardSummaryDTO"/> object with summary statistics.
    /// Returns HTTP 200 (OK) with the dashboard summary on success.
    /// Returns HTTP 500 (Internal Server Error) if an exception occurs during retrieval.
    /// </returns>
    /// <response code="200">Returns dashboard summary statistics</response>
    /// <response code="500">If an internal error occurs while retrieving the summary</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/diningsession/summary
    ///
    /// Returns real-time statistics including:
    /// - Total number of active (not ended) sessions
    /// - Total number of tables currently in use
    /// - Total number of open bills in active sessions
    /// - Total number of active participants (who haven't left)
    /// </remarks>
    [HttpGet("summary")]
    public async Task<IActionResult> GetDashBoardSummary()
    {
      try
      {
        var active_sessions = await _context.DiningSessions
            .Include(ds => ds.Table)
            .Include(ds => ds.TableGroup)
                .ThenInclude(tg => tg.Tables)
            .Where(ds => ds.Ended_At == null)
            .ToListAsync();

        int total_active_sessions = active_sessions.Count();

        // Count total tables in use - sum of individual tables and tables in groups
        int total_total_tables_in_use = active_sessions.Sum(session => GetTableCount(session));

        int total_active_bills = await _context.Bills
            .Join(
                _context.DiningSessions,
                bill => bill.Session_Id,
                session => session.Session_Id,
                (bill, session) => new { bill, session })
            .Where(x => x.session.Ended_At == null &&
                        x.bill.Status == BillStatus.Open)
            .CountAsync();

        int total_active_participants = await _context.SessionParticipants
            .Where(sp => sp.Left_At == null)
            .CountAsync();

        return Ok(new DashBoardSummaryDTO
        {
          Total_Active_Sessions = total_active_sessions,
          Total_Tables_In_Use = total_total_tables_in_use,
          Total_Active_Bills = total_active_bills,
          Total_ActiveParticipants = total_active_participants
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
      }
    }
  }
}