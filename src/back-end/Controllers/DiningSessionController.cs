using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.DTO.Auth;
using Microsoft.AspNetCore.Mvc;
using back_end.DTO.Analytics;
using back_end.domain.Entities;
using back_end.domain;
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

        [HttpGet("get_location/{session_id}")]
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

        [HttpPost("{session_id}/tables")]
        public async Task<IActionResult> add_table_to_dining_session(int session_id, [FromBody] TableAssignmentDTO tableData)
        {
            try
            {
                var session = await _context.DiningSessions
                                .Include(d=>d.Tables)
                                .Include(d=>d.Bills)
                                .FirstOrDefaultAsync(s=>s.Session_Id == session_id);

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

                if(!session.Tables.Any(t => t.Table_Id == table.Table_Id))
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

        [HttpDelete("{session_id}/Tables/{table_id}")]
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
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active Menu Session");
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
    }
}