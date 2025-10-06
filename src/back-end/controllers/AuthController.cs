
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.DTO;
using Microsoft.AspNetCore.Mvc;
using BCryptNet = BCrypt.Net.BCrypt;


namespace back_end.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IConfiguration _config;

        public AuthController(ApplicationContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {

            string UserEmail = request.UserEmail ?? string.Empty;
            string UserPassword = request.UserPassword ?? string.Empty;
            UserPassword = BCryptNet.HashPassword(UserPassword);
            var ReturnedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == UserEmail);

            if (ReturnedUser == null || ReturnedUser.Password_hash != UserPassword)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }
            if (ReturnedUser.Password_hash == UserPassword)
            {
                return Ok(new { message = "Login successful" });
            }
            return Unauthorized(new { message = "Invalid email or password" });
            
        }
    }
}