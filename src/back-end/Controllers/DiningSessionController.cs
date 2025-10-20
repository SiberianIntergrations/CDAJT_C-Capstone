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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data.SqlTypes;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace back_end.controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class DiningSessionController : Controller
  {
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;
    private readonly ILogger<DashBoardController> _logger;

    public DiningSessionController(ApplicationDbContext context, IConfiguration config, ILogger<DashBoardController> logger)
    {
      _context = context;
      _config = config;
      _logger = logger;
    }

    // Validation method for table assignment
    private void ValidateTableAssignment(DiningSession session)
    {
      if ((session.Table_Id.HasValue && session.TableGroup_Id.HasValue) ||
          (!session.Table_Id.HasValue && !session.TableGroup_Id.HasValue))
      {
        throw new ValidationException("DiningSession must have either Table_Id or TableGroup_Id, but not both.");
      }
    }

    [HttpPost("Create_Dinning_Session")]
    public async Task<IActionResult> create_dining_session([FromBody] CreateSessionRequestDTO sessionData)
    {
      try
      {
        var menu = await _context.Menus.Where(m => m.Menu_id == sessionData.Menu_Id).FirstOrDefaultAsync();
        if (menu == null)
        {
          return BadRequest("Menu Does not Exist");
        }

        var newSession = new DiningSession
        {
          Menu_Id = sessionData.Menu_Id,
          Started_At = DateTime.UtcNow
        };
        // TODO: Table_Id or TableGroup_Id should be set here

        _context.Add<DiningSession>(newSession);
        await _context.SaveChangesAsync();

        return Ok(new DiningSessionResponseDTO
        {
          Session_Id = newSession.Session_Id,
          Menu_Id = newSession.Menu_Id,
          Started_at = newSession.Started_At,
          Ended_at = newSession.Ended_At,
          First_Order_Time = newSession.First_Order_At,
          Table_Numbers = [],
          Active_Participants = 0
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
      }
    }

    [HttpGet("get_list_dining_sessions")]
    public async Task<IActionResult> list_dining_sessions([FromQuery] ListDiningSessionsRequestDTO act)
    {
      try
      {
        var query = _context.DiningSessions
            .Include(s => s.Table)
            .Include(s => s.TableGroup)
                .ThenInclude(tg => tg.Tables)
            .Include(s => s.Participants)
            .AsQueryable();

        if (act.ActiveOnly)
        {
          query = query.Where(ds => ds.Ended_At == null);
        }
        else
        {
          query = query.Where(ds => ds.Ended_At != null);
        }

        var sessions = await query.OrderByDescending(q => q.Started_At).ToListAsync();

        var Response = sessions.Select(session => new DiningSessionResponseDTO
        {
          Session_Id = session.Session_Id,
          Menu_Id = session.Menu_Id,
          Started_at = session.Started_At,
          Ended_at = session.Ended_At,
          First_Order_Time = session.First_Order_At,
          Table_Numbers = GetTableNumbers(session),
          Active_Participants = session.Participants.Count()
        }).ToList();

        return Ok(Response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
      }
    }

    // Helper method to get table numbers from either single table or table group
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

    [HttpGet("get_location/{session_id}")]
    public async Task<IActionResult> get_dining_session(int session_id)
    {
      try
      {
        var session = await _context.DiningSessions
            .Include(s => s.Bills)
            .Include(s => s.Table)
            .Include(s => s.TableGroup)
                .ThenInclude(tg => tg.Tables)
            .Include(s => s.Orders)
            .FirstOrDefaultAsync(s => s.Session_Id == session_id);

        if (session == null)
        {
          return BadRequest($"Session:{session_id} Does not Exist");
        }

        int active_participants = await _context.SessionParticipants
            .Where(sp => sp.Session_Id == session_id)
            .CountAsync();

        foreach (var returnedBill in session.Bills)
        {
          _logger.LogInformation($"Bill {returnedBill.Bill_Id}, status:{returnedBill.Status}");
        }

        return Ok(new DiningSessionDetailDTO
        {
          Session_Id = session.Session_Id,
          Menu_Id = session.Menu_Id,
          Started_at = session.Started_At,
          Ended_at = session.Ended_At,
          First_Order_Time = session.First_Order_At,
          Table_Numbers = GetTableNumbers(session),
          Active_Participants = active_participants,
          Orders_Count = session.Orders.Count(),
          Bills_Count = session.Bills.Count(),
          Active_Bill_Count = session.Bills.Count(b => b.Status == BillStatus.Open),
          Total_participant = active_participants
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
      }
    }

    [HttpPost("{session_id}/tables")]
    public async Task<IActionResult> add_table_to_dining_session(int session_id, [FromBody] TableAssignmentDTO tableData)
    {
      try
      {
        var session = await _context.DiningSessions
            .Include(d => d.Table)
            .Include(d => d.TableGroup)
            .Include(d => d.Bills)
            .FirstOrDefaultAsync(s => s.Session_Id == session_id);

        // Check if the session exists
        if (session == null)
        {
          return NotFound($"Session: {session_id} not found");
        }

        // Check if the session has already ended
        if (session.Ended_At != null)
        {
          return BadRequest($"Cannot modify an ended session: {session_id}");
        }

        // Check if a bill has been added to the session
        if (session.Bills.Any())
        {
          return BadRequest("Cannot add tables after a bill has been created");
        }

        // Check if session already has a table group assigned
        if (session.TableGroup_Id.HasValue)
        {
          return BadRequest("Cannot assign individual table to session with table group. Remove table group first.");
        }

        // Find the table that matches the inputted ID and check if it's active
        var table = await _context.Tables
            .FirstOrDefaultAsync(t => t.Table_Id == tableData.Table_Id && t.is_active);

        // Check if the table exists and it's active
        if (table == null)
        {
          return BadRequest($"Table: {tableData.Table_Id} not found or inactive");
        }

        // Check if the table is assigned to another dining session
        var checkIfAlreadyActiveSession = await _context.DiningSessions
            .AnyAsync(d => d.Ended_At == null &&
                          d.Session_Id != session_id &&
                          d.Table_Id == table.Table_Id);

        if (checkIfAlreadyActiveSession)
        {
          return BadRequest($"Table ID: {table.Table_Id} is currently in another active dining session.");
        }

        // Assign the table to the session
        session.Table_Id = table.Table_Id;
        session.Table = table;

        // Validate that only one assignment exists
        ValidateTableAssignment(session);

        await _context.SaveChangesAsync();

        return Ok(new DiningSessionResponseDTO
        {
          Session_Id = session.Session_Id,
          Menu_Id = session.Menu_Id,
          Started_at = session.Started_At,
          Ended_at = session.Ended_At,
          First_Order_Time = session.First_Order_At,
          Table_Numbers = new List<int> { table.table_number },
          Active_Participants = await _context.SessionParticipants.CountAsync(s => s.Session_Id == session.Session_Id),
        });
      }
      catch (ValidationException vex)
      {
        return BadRequest(vex.Message);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
      }
    }

    [HttpPost("{session_id}/table-groups")]
    public async Task<IActionResult> add_table_group_to_dining_session(int session_id, [FromBody] TableGroupAssignmentDTO tableGroupData)
    {
      try
      {
        var session = await _context.DiningSessions
            .Include(d => d.Table)
            .Include(d => d.TableGroup)
            .Include(d => d.Bills)
            .FirstOrDefaultAsync(s => s.Session_Id == session_id);

        // Check if the session exists
        if (session == null)
        {
          return NotFound($"Session: {session_id} not found");
        }

        // Check if the session has already ended
        if (session.Ended_At != null)
        {
          return BadRequest($"Cannot modify an ended session: {session_id}");
        }

        // Check if a bill has been added to the session
        if (session.Bills.Any())
        {
          return BadRequest("Cannot add table group after a bill has been created");
        }

        // Check if session already has an individual table assigned
        if (session.Table_Id.HasValue)
        {
          return BadRequest("Cannot assign table group to session with individual table. Remove table first.");
        }

        // Find the table group
        var tableGroup = await _context.TableGroups
            .Include(tg => tg.Tables)
            .FirstOrDefaultAsync(tg => tg.TableGroup_Id == tableGroupData.TableGroup_Id && tg.Is_Active);

        // Check if the table group exists and is active
        if (tableGroup == null)
        {
          return BadRequest($"Table group: {tableGroupData.TableGroup_Id} not found or inactive");
        }

        // Check if any tables in the group are assigned to another active session
        var tableIds = tableGroup.Tables.Select(t => t.Table_Id).ToList();
        var checkIfTablesInActiveSession = await _context.DiningSessions
            .AnyAsync(d => d.Ended_At == null &&
                          d.Session_Id != session_id &&
                          (d.Table_Id.HasValue && tableIds.Contains(d.Table_Id.Value) ||
                           d.TableGroup_Id.HasValue && d.TableGroup.Tables.Any(t => tableIds.Contains(t.Table_Id))));

        if (checkIfTablesInActiveSession)
        {
          return BadRequest($"One or more tables in table group {tableGroup.TableGroup_Id} are currently in another active dining session.");
        }

        // Assign the table group to the session
        session.TableGroup_Id = tableGroup.TableGroup_Id;
        session.TableGroup = tableGroup;

        // Validate that only one assignment exists
        ValidateTableAssignment(session);

        await _context.SaveChangesAsync();

        return Ok(new DiningSessionResponseDTO
        {
          Session_Id = session.Session_Id,
          Menu_Id = session.Menu_Id,
          Started_at = session.Started_At,
          Ended_at = session.Ended_At,
          First_Order_Time = session.First_Order_At,
          Table_Numbers = tableGroup.Tables.Select(t => t.table_number).ToList(),
          Active_Participants = await _context.SessionParticipants.CountAsync(s => s.Session_Id == session.Session_Id),
        });
      }
      catch (ValidationException vex)
      {
        return BadRequest(vex.Message);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
      }
    }

    [HttpDelete("{session_id}/Tables/{table_id}")]
    public async Task<IActionResult> remove_table_from_session(int session_id, int table_id)
    {
      try
      {
        var session = await _context.DiningSessions
            .Include(ds => ds.Bills)
            .Include(ds => ds.Table)
            .FirstOrDefaultAsync(ds => ds.Session_Id == session_id);

        if (session == null)
        {
          return NotFound($"Session was not found for session: {session_id}");
        }

        // Only check the status of the Bill
        if (session.Bills.Any(b => b.Status == BillStatus.Open))
        {
          return BadRequest($"Cannot close a table and session with open bills");
        }

        var table = await _context.Tables
            .FirstOrDefaultAsync(te => te.Table_Id == table_id);

        if (table == null)
        {
          return NotFound($"Table was not found");
        }

        // Check if this table is assigned to the session
        if (session.Table_Id == table_id)
        {
          session.Table_Id = null;
          session.Table = null;
          await _context.SaveChangesAsync();
          return Ok(new { message = "Table removed from session successfully" });
        }
        else
        {
          return NotFound($"Table was not found in the session: {session_id}");
        }
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
      }
    }

    [HttpDelete("{session_id}/table-groups/{table_group_id}")]
    public async Task<IActionResult> remove_table_group_from_session(int session_id, int table_group_id)
    {
      try
      {
        var session = await _context.DiningSessions
            .Include(ds => ds.Bills)
            .Include(ds => ds.TableGroup)
            .FirstOrDefaultAsync(ds => ds.Session_Id == session_id);

        if (session == null)
        {
          return NotFound($"Session was not found for session: {session_id}");
        }

        // Only check the status of the Bill
        if (session.Bills.Any(b => b.Status == BillStatus.Open))
        {
          return BadRequest($"Cannot remove table group from session with open bills");
        }

        // Check if this table group is assigned to the session
        if (session.TableGroup_Id == table_group_id)
        {
          session.TableGroup_Id = null;
          session.TableGroup = null;
          await _context.SaveChangesAsync();
          return Ok(new { message = "Table group removed from session successfully" });
        }
        else
        {
          return NotFound($"Table group was not found in the session: {session_id}");
        }
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
      }
    }

    [Authorize]
    [HttpGet("participants/active-session-id")]
    public async Task<IActionResult> GetActiveSessionId()
    {
      try
      {
        // Get current user ID from claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
          return Unauthorized(new { message = "User not authenticated" });
        }

        int userId = int.Parse(userIdClaim);

        var activeSessionId = await _context.DiningSessions
            .Join(
                _context.SessionParticipants,
                ds => ds.Session_Id,
                sp => sp.Session_Id,
                (ds, sp) => new { ds, sp })
            .Where(x => x.sp.User_Id == userId &&
                        x.ds.Ended_At == null)
            .Select(x => x.ds.Session_Id)
            .FirstOrDefaultAsync();

        if (activeSessionId == 0)
        {
          return NotFound(new { message = "No active session found" });
        }

        return Ok(new { session_id = activeSessionId });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error retrieving active session");
        return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
      }
    }

    [HttpGet("session-menu/{session_id}")]
    public async Task<ActionResult<SessionMenuResponseDTO>> get_session_menu_id(int session_id)
    {
      try
      {
        var session = await _context.DiningSessions
            .FirstOrDefaultAsync(ds => ds.Session_Id == session_id);

        if (session == null)
        {
          return NotFound("Dining Session was not found");
        }

        return Ok(new SessionMenuResponseDTO
        {
          Menu_Id = session.Menu_Id
        });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error retrieving active Menu Session");
        return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
      }
    }
  }
}