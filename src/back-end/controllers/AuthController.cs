
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.DTO;
using Microsoft.AspNetCore.Mvc;
using BCryptNet = BCrypt.Net.BCrypt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


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
            //UserPassword = BCryptNet.HashPassword(UserPassword);
            var ReturnedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == UserEmail);

            if (ReturnedUser == null || ReturnedUser.Password_hash != UserPassword)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }
            if (ReturnedUser.Password_hash == UserPassword)
            {
                var token = GenerateJwtToken(ReturnedUser.Email);
                return Ok(new
                {
                    access_token = token,
                    token_type = "Bearer",
                    expires_in = 7200, // 2 hours in seconds:
                });

            }
            return Unauthorized(new { message = "Invalid email or password" });

        }


    private string GenerateJwtToken(string username)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Email, username),
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                int.Parse(_config["Jwt:ExpiresInMinutes"] ?? "60")),
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    }
}