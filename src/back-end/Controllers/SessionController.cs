using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.domain.Entities;

namespace back_end.Controllers
{
  [ApiController]
  [Route("api/[controller]")]

  public class SessionController : ControllerBase
  {
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SessionController> _logger;

    public SessionController(ApplicationDbContext context, ILogger<SessionController> logger)
    {
      _context = context;
      _logger = logger;
    }

    //GET api/session
    //Get all sessions
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DiningSession>>> GetAllSessions()
    {
      try
      {
        var sessions = await _context.DiningSessions.ToListAsync();
        return Ok(sessions);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Cannot get Dining Sessions");
        return StatusCode(500, "Internal Server Error");
      }
    }

    //GET: api/session/
    //Get a dining session's users, menu, orders and table assignments
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetSessionById(int id)
    {
      try
      {
        //Gather all the information for the inputted session
        var session = await _context.DiningSessions
                .Where(s => s.Session_Id == id)
                .Include(s => s.Menu)
                .Include(s => s.Participants)
                    .ThenInclude(se => se.User)
                .Include(s => s.Sessions)
                    .ThenInclude(ss => ss.Table)
                .Include(s => s.Orders)
                    .ThenInclude(so => so.OrderItems)
                .FirstOrDefaultAsync();

        //Check if the session exists
        if (session == null)
        {
          return NotFound(new { message = $"Can't find Session with ID: {id}" });
        }

        //Start creating an object that stores/organizes session information
        var sessionInfo = new
        {
          sessionID = session.Session_Id,
          menu = session.Menu.Name,
          start = session.Started_At,
          end = session.Ended_At,
          firstOrder = session.First_Order_At,
          active = session.Ended_At == null,

          //Get all the participants in the session with their information
          guests = session.Participants.Select(s => new
          {
            participantId = s.Participant_Id,
            userId = s.User_Id,
            userName = $"{s.User.First_name} {s.User.Last_name}",
            joinedAt = s.Joined_At,
            leftAt = s.Left_At
          }).ToList(),

          //Get information about each table assigned to each participant
          tables = session.Sessions.Select(s => new
          {
            tableId = s.Table_Id,
            tableNumber = s.Table.table_number,
            seatCount = s.Table.seat_count,
          }).ToList(),

          //Get all of the orders in the session and their status/completed/done
          orders = session.Orders.Select(s => new
          {
            orderID = s.Order_Id,
            userId = s.User_Id,
            status = s.Status.ToString(),
            itemCount = s.OrderItems.Count,
            createdAt = s.Created_At,
            completedAt = s.Completed_At
          }).ToList()
        };

        return Ok(sessionInfo);

      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"Can't get session for ID: {id}");
        return StatusCode(500, "Internal Server Error");
      }
    }

    //GET: api/session/active
    //Get all active dining sessions
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<object>>> GetActiveSessions()
    {
      try
      {
        //Get all current active sessions (ended at is null)
        //Include Menu, Participants, and tables
        var activeSessions = await _context.DiningSessions.Where(s => s.Ended_At == null)
                            .Include(s => s.Menu)
                            .Include(s => s.Participants)
                                .ThenInclude(se => se.User)
                            .Include(s => s.Sessions)
                                .ThenInclude(ss => ss.Table)
                            .ToListAsync();

        //Keep all key information
        //Session ID, Menu, Session Start time and # of Participants
        var sessionInfo = activeSessions.Select(s => new
        {
          sessionId = s.Session_Id,
          menuNAme = s.Menu.Name,
          startedAt = s.Started_At,
          participantCount = s.Participants.Count,
          //Get the table number and ID and add  up total orders for the session
          tables = s.Sessions.Select(se => new
          {
            tableNuber = se.Table.table_number,
            tableId = se.Table_Id
          }).ToList(),
          orderCount = s.Orders.Count
        }).ToList();

        return Ok(sessionInfo);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Can't get current active Dining Sessions");
        return StatusCode(500, "Internal Server Error");
      }
    }

    //GET: api/session/table
    //Get the session for a specific active table
    [HttpGet("table/{table}")]
    public async Task<ActionResult<object>> GetSessionByTable(int table)
    {
      try
      {
        var sessionTable = await _context.Tables.FindAsync(table);

        //Check if table exists
        if (sessionTable == null)
        {
          return NotFound(new { message = $"Can't find session with table number: {table}" });

        }

        //Find an active dining session connected to inputted table
        //Include Menu, Participants, and User data
        var sessionInfo = await _context.SessionTables
                    .Where(s => s.Table_Id == table)
                    .Include(s => s.DiningSession)
                        .ThenInclude(se => se.Menu)
                    .Include(s => s.DiningSession)
                        .ThenInclude(ss => ss.Participants)
                            .ThenInclude(ses => ses.User)
                    .Where(s => s.DiningSession.Ended_At == null)
                    .Select(s => s.DiningSession)
                    .FirstOrDefaultAsync();

        //Check if there's an active Dining Session
        if (sessionInfo == null)
        {
          return Ok(new
          {
            message = "No active session at this table",
            tableNumber = sessionTable.table_number,
            tableId = table,
            activeSession = false
          });
        }

        //Get Session, Menu Details and all users active at the table
        var sessionDetails = new
        {
          activeSession = true,
          sessionId = sessionInfo.Session_Id,
          menuName = sessionInfo.Menu.Name,
          menuId = sessionInfo.Menu_Id,
          startedAt = sessionInfo.Started_At,
          participants = sessionInfo.Participants.Where(s => s.Left_At == null)
                                                .Select(s => new
                                                {
                                                  userId = s.User_Id,
                                                  userName = $"{s.User.First_name} {s.User.Last_name}",
                                                  joinedAt = s.Joined_At
                                                }).ToList()
        };

        return Ok(sessionDetails);
      }

      catch (Exception ex)
      {
        _logger.LogError(ex, $"Can't get session for table ID: {table}");
        return StatusCode(500, "Internal Server Error");
      }
    }

    //GET: api/session/user/id/current
    //Get the active session for a user
    [HttpGet("user/{userId}/current")]
    public async Task<ActionResult<object>> GetCurrentSessionByUser(int userId)
    {
      try
      {
        var user = await _context.Users.FindAsync(userId);

        //Check if user exists
        if (user == null)
        {
          return NotFound(new { message = $"Can't find user with ID: {userId}" });
        }

        //Check for an active session that the user is currently in
        //Add Menu and Table information
        var session = await _context.SessionParticipants
                            .Where(s => s.User_Id == userId && s.Left_At == null)
                            .Include(s => s.DiningSession)
                                .ThenInclude(se => se.Menu)
                            .Include(s => s.DiningSession)
                                .ThenInclude(se => se.Sessions)
                                    .ThenInclude(ss => ss.Table)
                            .Where(s => s.DiningSession.Ended_At == null)
                            .Select(s => s.DiningSession)
                            .FirstOrDefaultAsync();

        //Check if the user is apart of an active session
        if (session == null)
        {
          return Ok(new
          {
            message = "Guest is not currently in an active dining session.",
            userId = user.User_id,
            userName = $"{user.First_name} {user.Last_name}",
            activeSession = false
          });
        }

        //Get Session ID, Menu information, tables assigned 
        //Then return the infomration for the active session
        //Should I add their name?
        var sessionDetails = new
        {
          activeSession = true,
          sessionId = session.Session_Id,
          menuName = session.Menu.Name,
          menuId = session.Menu_Id,
          startedAt = session.Started_At,
          tables = session.Sessions.Select(s => new
          {
            tableId = s.Table_Id,
            tableNumber = s.Table.table_number
          }).ToList()
        };

        return Ok(sessionDetails);
      }

      catch (Exception ex)
      {
        _logger.LogError(ex, $"Cant get session for user ID: {userId}");
        return StatusCode(500, "Internal Server Error");
      }
    }
  }
}