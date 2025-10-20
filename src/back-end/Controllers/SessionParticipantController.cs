using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.domain.Entities;
using back_end.domain.enums;
using System.Linq.Expressions;
using back_end.DTO.MenuItems;
using Microsoft.AspNetCore.Authorization;
using back_end.DTO.MenuDTO;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using back_end.domain.Seeders;
using System.Security.Claims;
using back_end.DTO.SessionParticipantDTOs;

namespace back_end.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class SessionParticipantController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SessionParticipantController> _logger;

        public SessionParticipantController(ApplicationDbContext context, ILogger<SessionParticipantController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize]
        [HttpPost("join")]
        public async Task<IActionResult> JoinSession(
            int session_id
        )
        {
            try
            {
                var session = await _context.DiningSessions.FirstOrDefaultAsync(ds => ds.Session_Id == session_id);
                if (session is null)
                {
                    return NotFound("Dining Session was not found");
                }
                if (session.Ended_At.HasValue)
                {
                    return BadRequest("Can not Join a ended Session");
                }
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var foundUser = await _context.Users.FirstOrDefaultAsync(u => u.User_id == userId);

                var existingParticipant = await _context.SessionParticipants.FirstOrDefaultAsync(sp => sp.Session_Id == session_id && sp.User_Id == foundUser.User_id && !sp.Left_At.HasValue);
                if (existingParticipant != null)
                {
                    return BadRequest("You are already A participant");
                }
                var newParticipant = new SessionParticipant
                {
                    Session_Id = session_id,
                    User_Id = foundUser.User_id,
                    Joined_At = DateTime.UtcNow,
                    Left_At = null,

                };
                _context.SessionParticipants.Add(newParticipant);
                await _context.SaveChangesAsync();
                return Ok(new SessionParticipantResponseDTO
                {
                    Participant_Id = newParticipant.Participant_Id,
                    Session_Id = newParticipant.Session_Id,
                    User_Id = newParticipant.User_Id,
                    Joined_At = newParticipant.Joined_At,
                    Left_At = newParticipant.Left_At

                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Creating Session");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [Authorize]
        [HttpPost("leave")]
        public async Task<IActionResult> LeaveSession(
            int session_id
        )
        {
            try
            {
                var session = await _context.DiningSessions.FirstOrDefaultAsync(ds => ds.Session_Id == session_id);
                if (session is null)
                {
                    return NotFound("Dining Session was not found");
                }

                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var foundUser = await _context.Users.FirstOrDefaultAsync(u => u.User_id == userId);

                var existingParticipant = await _context.SessionParticipants.FirstOrDefaultAsync(sp => sp.Session_Id == session_id && sp.User_Id == foundUser.User_id && !sp.Left_At.HasValue);
                if(existingParticipant is null)
                {
                    return BadRequest("You are not a active participant to the session");
                }
                var activeOrders = await _context.SessionOrders.FirstOrDefaultAsync(so => so.session_id == session_id && so.User_Id == foundUser.User_id && so.Status != OrderStatus.Delivered);
                if (activeOrders != null)
                {
                    return BadRequest("You can not leave a active session with active orders");
                }
                existingParticipant.Left_At = DateTime.UtcNow;
                _context.SessionParticipants.Update(existingParticipant);
                await _context.SaveChangesAsync();
                return Ok("You have successfully left a session");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Creating Session");
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}