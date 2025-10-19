
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.DTO.Auth;
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
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            string UserEmail = request.UserEmail ?? string.Empty;
            string UserPassword = request.UserPassword ?? string.Empty;
            var ReturnedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == UserEmail);
            bool isPasswordVerified = BCryptNet.Verify(UserPassword, ReturnedUser?.Password_hash ?? string.Empty);
            if (ReturnedUser == null || !isPasswordVerified)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }
            if (isPasswordVerified)
            {
                ReturnedUser.Last_Interaction_at = DateTime.UtcNow;
                _context.Users.Update(ReturnedUser);
                await _context.SaveChangesAsync();

                //Generate the JWT token with Returned user assigned above
                var token = GenerateJwtToken(ReturnedUser);
                return Ok(new
                {
                    access_token = token,
                    token_type = "Bearer",
                    expires_in = 7200, // 2 hours in seconds:
                });

            }
            return Unauthorized(new { message = "Invalid email or password" });

        }
    
        [HttpPost("register")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterDTO request)
        {
            int passwordMinLength = 6;
            if (request == null || request.UserEmail == null || request.UserPassword == null || request.FirstName == null || request.LastName == null)
            {
                return BadRequest(new { message = "Invalid request" });
            }
            string UserEmail = request.UserEmail.ToLower().Trim() ?? string.Empty;
            string UserPassword = request.UserPassword.Trim() ?? string.Empty;
            string FirstName = request.FirstName.Trim() ?? string.Empty;
            string LastName = request.LastName.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(UserEmail) || string.IsNullOrEmpty(UserPassword) || string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName))
            {
                return BadRequest(new { message = "All fields are required" });
            }
            if (await _context.Users.AnyAsync(u => u.Email == UserEmail))
            {
                return Conflict(new { message = "Email already in use" });
            }
            if (UserPassword.Length < passwordMinLength)
            {
                return BadRequest(new { message = $"Password must be at least {passwordMinLength} characters long" });
            }
            UserPassword = BCryptNet.HashPassword(UserPassword);
            try
            {
                var NewUser = new domain.Entities.User
                {
                    Email = UserEmail,
                    Password_hash = UserPassword,
                    First_name = FirstName,
                    Last_name = LastName,
                    Is_email_confirmed = false
                };
                _context.Users.Add(NewUser);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error creating user: {ex.Message}" });
            }


            return Ok(new { message = "User created successfully" });
        }

    private string GenerateJwtToken(domain.Entities.User user)
        {

            var keyStr = _config["Jwt:Key"];
            Console.WriteLine($"[JWT SIGN] Key len: {keyStr?.Length}, First8: {keyStr?[..Math.Min(8, keyStr!.Length)]}");

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Add all the claims based on the user's information and their role
            //Name and Email claims are the same - might need to investigate this further - possible bloat
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.User_id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("firstName", user.First_name),
                new Claim("lastName", user.Last_name),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
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