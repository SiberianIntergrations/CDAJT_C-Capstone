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

        [HttpGet("sessions")]
        public async Task<IActionResult> GetDashBoardSessions()
        {
            try
            {
                var sessions = await _context.DiningSessions.Where(ds => ds.Ended_At.Equals(null)).OrderByDescending(ds => ds.Started_At).ToListAsync();
                if (sessions == null || sessions.Count() == 0)
                {
                    return NotFound(new { message = "No active dining sessions found." });
                }

                var dashBoardSessions = new List<object>();
                foreach(var session in sessions)
                {
                    bool has_open_bill = session.Bills.Any(b => b.Status == BillStatus.Open);
                    bool Is_Closable = !has_open_bill;

                    var active_participants = _context.SessionParticipants.Where(sp => sp.Session_Id == session.Session_Id && sp.Left_At == null).Count();

                    var dashBoard_session = new DashBoardSessionsDTO
                    {
                        Session_Id = session.Session_Id,
                        Menu_Id = session.Menu_Id,
                        Started_At = session.Started_At,
                        Ended_At = session.Ended_At,
                        First_Order_At = session.First_Order_At,
                        Table_Numbers = session.Tables.Select(t => t.Table_Number).ToList(),
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
        [HttpGet("summary")]
        public async Task<IActionResult> GetDashBoardSummary()
        {
            try
            {
                var active_sessions = await _context.DiningSessions.Where(ds => ds.Ended_At == null).ToListAsync();
                int total_active_sessions = active_sessions.Count();
                int total_total_tables_in_use = active_sessions.Sum(session => session.Tables.Count());
                int total_active_bills = await _context.Bills
                    .Join(
                        _context.DiningSessions,
                        bill => bill.Session_Id,
                        session => session.Session_Id,
                        (bill, session) => new { bill, session })
                    .Where(x => x.session.Ended_At == null &&
                                x.bill.Status == BillStatus.Open)
                    .CountAsync();
                int total_active_participants = await _context.SessionParticipants.Where(sp => sp.Left_At == null).CountAsync();

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