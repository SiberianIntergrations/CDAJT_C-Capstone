using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.domain.Entities;

namespace back_end.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AuthController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ApplicationContext context, ILogger<AuthController> logger)
        {
            _context = context;
            _logger = logger;
        }
        //GET api/auth/users
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            try
            {
                var user = await _context.Users.ToListAsync();
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Getting Users");
                return StatusCode(500, "Internal Server Error");
            }
            
        }
    }
}