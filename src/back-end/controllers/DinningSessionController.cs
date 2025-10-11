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
    public class DinningSessionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<DashBoardController> _logger;

        public DinningSessionController(ApplicationDbContext context, IConfiguration config, ILogger<DashBoardController> logger)
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
        public async Task<IActionResult>list_dining_sessions([FromBody] DiningSessionResponseDTO listOfSessions, bool ActiveOnly = false)
        {
            try
            {
                var query = _context.DiningSessions
                    .Include(s => s.Tables )
                    .Include(s => s.Participants)
                    .AsQueryable();
                if (ActiveOnly == true)
                {
                    query = query.Where(ds => ds.Ended_At == null);
                }
                var sessions = await query.OrderByDescending(q => q.Started_At).ToListAsync();
                var Response = sessions.Select(session => new DiningSessionResponseDTO
                {
                    Session_Id = session.Session_Id,
                    Menu_Id = session.Menu_Id,
                    Started_at = session.Started_At,
                    Ended_at = session.Ended_At,
                    First_Order_Time = session.Started_At,
                    Table_Numbers = session.Tables.Select(t => t.table_number).ToList(),
                    Active_Participants = session.Participants.Count()
                }).ToList();
                return Ok(Response);

            }
            catch( Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }

    }

}