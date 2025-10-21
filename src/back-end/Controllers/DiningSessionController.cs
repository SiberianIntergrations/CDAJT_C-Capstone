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

    /// <summary>
    /// Creates a new dining session with a specified menu.
    /// </summary>
    /// <param name="sessionData">The session creation data containing the menu ID</param>
    /// <returns>
    /// An <see cref="IActionResult"/> containing the created <see cref="DiningSessionResponseDTO"/> object.
    /// Returns HTTP 200 (OK) with the created session details on success.
    /// Returns HTTP 404 (Not Found) if the menu doesn't exist.
    /// Returns HTTP 500 (Internal Server Error) if an exception occurs during creation.
    /// </returns>
    /// <response code="200">Returns the newly created dining session</response>
    /// <response code="404">If the menu is not found</response>
    /// <response code="500">If an internal error occurs while creating the session</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/diningsession/Create_Dinning_Session
    ///     {
    ///         "menu_Id": 123
    ///     }
    ///
    /// Creates a new dining session with the specified menu.
    /// The session is automatically marked as started with the current UTC timestamp.
    /// Tables and participants can be added after session creation.
    /// </remarks>
    [HttpPost("Create_Dinning_Session")]
    [ProducesResponseType(typeof(DiningSessionResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Retrieves a list of dining sessions filtered by their active status.
    /// </summary>
    /// <param name="act">Query parameters to filter sessions by active status</param>
    /// <returns>
    /// An <see cref="IActionResult"/> containing a collection of <see cref="DiningSessionResponseDTO"/> objects.
    /// Returns HTTP 200 (OK) with the list of filtered sessions sorted by start time (newest first).
    /// Returns HTTP 500 (Internal Server Error) if an exception occurs during retrieval.
    /// </returns>
    /// <response code="200">Returns the list of dining sessions filtered by active status</response>
    /// <response code="500">If an internal error occurs while retrieving sessions</response>
    /// <remarks>
    /// Sample requests:
    ///
    ///     GET /api/diningsession/get_list_dining_sessions?ActiveOnly=true
    ///     (Returns only active/ongoing sessions)
    ///     
    ///     GET /api/diningsession/get_list_dining_sessions?ActiveOnly=false
    ///     (Returns only ended sessions)
    ///
    /// Sessions are ordered by start time in descending order (newest first).
    /// Each session includes table numbers and participant count.
    /// </remarks>
    [HttpGet("get_list_dining_sessions")]
    [ProducesResponseType(typeof(IEnumerable<DiningSessionResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> list_dining_sessions([FromQuery] ListDiningSessionsRequestDTO act)
    {
      try
      {
        var query = _context.DiningSessions
            .Include(s => s.Tables)
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
          Table_Numbers = session.Tables.Select(t => t.table_number).ToList(),
          Active_Participants = session.Participants.Count()
        }).ToList();
        return Ok(Response);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
      }
    }

    /// <summary>
    /// Retrieves detailed information about a specific dining session.
    /// </summary>
    /// <param name="session_id">The unique identifier of the dining session</param>
    /// <returns>
    /// An <see cref="IActionResult"/> containing a <see cref="DiningSessionDetailDTO"/> object with session details.
    /// Returns HTTP 200 (OK) with the session details on success.
    /// Returns HTTP 404 (Not Found) if the session doesn't exist.
    /// Returns HTTP 500 (Internal Server Error) if an exception occurs during retrieval.
    /// </returns>
    /// <response code="200">Returns detailed information about the dining session</response>
    /// <response code="404">If the session is not found</response>
    /// <response code="500">If an internal error occurs while retrieving the session</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/diningsession/get_location/123
    ///
    /// Returns comprehensive session details including:
    /// - Session metadata (ID, menu, timestamps)
    /// - Table numbers
    /// - Participant counts
    /// - Order and bill statistics
    /// </remarks>
    [HttpGet("get_location/{session_id}")]
    [ProducesResponseType(typeof(DiningSessionDetailDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> get_dining_session(
        int session_id)
    {
      try
      {
        var session = await _context.DiningSessions.Include(s => s.Bills).FirstOrDefaultAsync(s => s.Session_Id == session_id);

        if (session == null)
        {
          return BadRequest($"Session:{session_id} Does not Exist");
        }
        int active_participants = await _context.SessionParticipants.Where(sp => sp.Session_Id == session_id).CountAsync();

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
          Table_Numbers = session.Tables.Select(t => t.table_number).ToList(),
          Active_Participants = active_participants,
          Orders_Count = session.Orders.Count(),
          Bills_Count = session.Bills.Count(),
          Active_Bill_Count = session.Bills.Select(b => b.Status == BillStatus.Open).Count(),
          Total_participant = active_participants
        });

      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
      }
    }

    /// <summary>
    /// Adds a table to an existing dining session.
    /// </summary>
    /// <param name="session_id">The unique identifier of the dining session</param>
    /// <param name="tableData">The table assignment data containing the table ID</param>
    /// <returns>
    /// An <see cref="IActionResult"/> containing the updated <see cref="DiningSessionResponseDTO"/> object.
    /// Returns HTTP 200 (OK) with the updated session details on success.
    /// Returns HTTP 400 (Bad Request) if the session has ended, has bills, or the table is unavailable.
    /// Returns HTTP 404 (Not Found) if the session doesn't exist.
    /// Returns HTTP 500 (Internal Server Error) if an exception occurs during the operation.
    /// </returns>
    /// <response code="200">Returns the updated dining session with the added table</response>
    /// <response code="400">If the session has ended, has bills, or the table is inactive/assigned to another session</response>
    /// <response code="404">If the session is not found</response>
    /// <response code="500">If an internal error occurs while adding the table to the session</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/diningsession/123/tables
    ///     {
    ///         "table_Id": 456
    ///     }
    ///
    /// Adds a table to an active dining session.
    /// Requirements:
    /// - Session must not have ended
    /// - Session must not have any bills created yet
    /// - Table must be active
    /// - Table must not be assigned to another active session
    /// </remarks>
    [HttpPost("{session_id}/tables")]
    [ProducesResponseType(typeof(DiningSessionResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> add_table_to_dining_session(int session_id, [FromBody] TableAssignmentDTO tableData)
    {
      try
      {
        var session = await _context.DiningSessions
                        .Include(d => d.Tables)
                        .Include(d => d.Bills)
                        .FirstOrDefaultAsync(s => s.Session_Id == session_id);

        //Check if the session exists
        if (session == null)
        {
          return NotFound($"Session: {session_id} Did not return any tables");
        }
        //Check if the session has already ended
        if (session.Ended_At != null)
        {
          return BadRequest($"Can not Modify a Ended Session: {session_id}");
        }
        //Check if a bill has been added to the session
        if (session.Bills.Any())
        {
          return BadRequest("Can not Add Tables to After Bill has been Created");
        }

        //Find the table that matches the inputted ID and check if it's active
        var table = await _context.Tables
                    .FirstOrDefaultAsync(t => t.Table_Id == tableData.Table_Id && t.is_active);

        //Check if the table exists and it's active
        if (table == null)
        {
          return BadRequest($"Table: {tableData.Table_Id} not found or inactive");
        }

        //Check if the table is assigned to another dining session
        var checkIfAlreadyActiveSession = await _context.DiningSessions
                                .AnyAsync(d => d.Ended_At == null && d.Session_Id != session_id && d.Tables
                                .Any(t => t.Table_Id == table.Table_Id));

        if (checkIfAlreadyActiveSession)
        {
          return BadRequest($"Table ID: {table.Table_Id} is currently in another Active Dining Session.");
        }

        if (!session.Tables.Any(t => t.Table_Id == table.Table_Id))
        {
          session.Tables.Add(table);
        }

        await _context.SaveChangesAsync();

        return Ok(new DiningSessionResponseDTO
        {
          Session_Id = session.Session_Id,
          Menu_Id = session.Menu_Id,
          Started_at = session.Started_At,
          Ended_at = session.Ended_At,
          First_Order_Time = session.First_Order_At,
          Table_Numbers = session.Tables.Select(tn => tn.table_number).ToList(),
          Active_Participants = await _context.SessionParticipants.CountAsync(s => s.Session_Id == session.Session_Id),
        });

      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
      }
    }

    /// <summary>
    /// Removes a table from a dining session.
    /// </summary>
    /// <param name="session_id">The unique identifier of the dining session</param>
    /// <param name="table_id">The unique identifier of the table to remove</param>
    /// <returns>
    /// An <see cref="IActionResult"/> indicating the result of the operation.
    /// Returns HTTP 200 (OK) with a success message when the table is removed.
    /// Returns HTTP 400 (Bad Request) if the session has open bills.
    /// Returns HTTP 404 (Not Found) if the session, table, or table-session association doesn't exist.
    /// Returns HTTP 500 (Internal Server Error) if an exception occurs during removal.
    /// </returns>
    /// <response code="200">Returns a success message when the table is removed from the session</response>
    /// <response code="400">If the session has open bills preventing table removal</response>
    /// <response code="404">If the session, table, or table-session association is not found</response>
    /// <response code="500">If an internal error occurs while removing the table from the session</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     DELETE /api/diningsession/123/Tables/456
    ///
    /// Removes the association between a table and a dining session.
    /// Tables cannot be removed from sessions that have open bills.
    /// </remarks>
    [HttpDelete("{session_id}/Tables/{table_id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> remove_table_from_session(
        int session_id,
        int table_id
    )
    {
      try
      {
        var session = await _context.DiningSessions.Where(ds => ds.Session_Id == session_id).FirstOrDefaultAsync();
        if (session == null)
        {
          return NotFound($"Session was not Found for Table:{table_id} tagged to session: {session_id}");
        }
        //Only check the status of the Bill
        if (session.Bills.Any(b => b.Status == BillStatus.Open))
        {
          return BadRequest($"Can not close a table and session with Open Bills");
        }

        var table = await _context.Tables.Where(te => te.Table_Id == table_id).FirstOrDefaultAsync();
        if (table == null)
        {
          return NotFound($"Table was not found");

        }
        if (session.Tables.Contains(table))
        {
          session.Tables.Remove(table);
          await _context.SaveChangesAsync();
          return Ok();
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

      /// <summary>
      /// Retrieves the active dining session ID for the current authenticated user.
      /// </summary>
      /// <returns>
      /// An <see cref="IActionResult"/> containing the active session ID.
      /// Returns HTTP 200 (OK) with the session ID on success.
      /// Returns HTTP 401 (Unauthorized) if the user is not authenticated.
      /// Returns HTTP 404 (Not Found) if no active session exists for the user.
      /// Returns HTTP 500 (Internal Server Error) if an exception occurs during retrieval.
      /// </returns>
      /// <response code="200">Returns the active session ID for the authenticated user</response>
      /// <response code="401">If the user is not authenticated</response>
      /// <response code="404">If no active session is found for the user</response>
      /// <response code="500">If an internal error occurs while retrieving the active session</response>
      /// <remarks>
      /// Sample request:
      ///
      ///     GET /api/diningsession/participants/active-session-id
      ///
      /// This endpoint requires authentication.
      /// Returns the session ID of the user's current active (not ended) dining session.
      /// A user can only have one active session at a time.
      /// </remarks>
      [Authorize]
      [HttpGet("participants/active-session-id")]
      [ProducesResponseType(StatusCodes.Status200OK)]
      [ProducesResponseType(StatusCodes.Status401Unauthorized)]
      [ProducesResponseType(StatusCodes.Status404NotFound)]
      [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Retrieves the menu ID associated with a specific dining session.
        /// </summary>
        /// <param name="session_id">The unique identifier of the dining session</param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing a <see cref="SessionMenuResponseDTO"/> object with the menu ID.
        /// Returns HTTP 200 (OK) with the menu ID on success.
        /// Returns HTTP 404 (Not Found) if the dining session doesn't exist.
        /// Returns HTTP 500 (Internal Server Error) if an exception occurs during retrieval.
        /// </returns>
        /// <response code="200">Returns the menu ID for the specified session</response>
        /// <response code="404">If the dining session is not found</response>
        /// <response code="500">If an internal error occurs while retrieving the session menu</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/diningsession/session-menu/123
        ///
        /// Returns the menu ID associated with the specified dining session.
        /// </remarks>
        [HttpGet("session-menu/{session_id}")]
        [ProducesResponseType(typeof(SessionMenuResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SessionMenuResponseDTO>> get_session_menu_id(int session_id)
    {
      try
      {
        var session = await _context.DiningSessions.Where(ds => ds.Session_Id == session_id).FirstOrDefaultAsync();
        if (session == null)
        {
          return NotFound("Dining Session was Not Found");
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