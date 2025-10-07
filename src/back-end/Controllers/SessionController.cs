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
        private readonly ApplicationContext _context;
        private readonly ILogger<SessionController> _logger;

        public SessionController(ApplicationContext context, ILogger<SessionController> logger)
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
    }
}