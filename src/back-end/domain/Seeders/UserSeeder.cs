using Microsoft.Extensions.Logging;
using back_end.Domain.Enums;
using back_end.Domain.Entitys;

namespace back_end.Domain.Seeders
{
    /// <summary>
    /// Seeder for the users table with initial data.
    /// Creates 3 staff accounts and 10 customer accounts.
    /// Note: Admin account (user_id: 1) should be created separately.
    /// </summary>
    public class UserSeeder : ISeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserSeeder> _logger;

        public UserSeeder(ApplicationDbContext context, ILogger<UserSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Seed()
        {
            // Staff accounts
            var staffUsers = new List<User>
            {
                new User
                {
                    Email = "john.smith@sushitoshi.ca",
                    PasswordHash = HashPassword("StaffPass123!"),
                    FirstName = "John",
                    LastName = "Smith",
                    Role = UserRole.Staff,
                    Status = UserStatus.Active,
                    IsEmailVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    LastInteractionAt = DateTime.UtcNow
                },
                new User
                {
                    Email = "sarah.jones@sushitoshi.ca",
                    PasswordHash = HashPassword("StaffPass123!"),
                    FirstName = "Sarah",
                    LastName = "Jones",
                    Role = UserRole.Staff,
                    Status = UserStatus.Active,
                    IsEmailVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    LastInteractionAt = DateTime.UtcNow
                },
                new User
                {
                    Email = "michael.chen@sushitoshi.ca",
                    PasswordHash = HashPassword("StaffPass123!"),
                    FirstName = "Michael",
                    LastName = "Chen",
                    Role = UserRole.Staff,
                    Status = UserStatus.Active,
                    IsEmailVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    LastInteractionAt = DateTime.UtcNow
                }
            };

            // Customer accounts
            var customerUsers = new List<User>
            {
                new User
                {
                    Email = "emma.wilson@email.com",
                    PasswordHash = HashPassword("Customer123!"),
                    FirstName = "Emma",
                    LastName = "Wilson",
                    Role = UserRole.Customer,
                    Status = UserStatus.Active,
                    IsEmailVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    LastInteractionAt = DateTime.UtcNow
                },
                new User
                {
                    Email = "james.brown@email.com",
                    PasswordHash = HashPassword("Customer123!"),
                    FirstName = "James",
                    LastName = "Brown",
                    Role = UserRole.Customer,
                    Status = UserStatus.Active,
                    IsEmailVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    LastInteractionAt = DateTime.UtcNow
                },
                new User
                {
                    Email = "sophia.lee@email.com",
                    PasswordHash = HashPassword("Customer123!"),
                    FirstName = "Sophia",
                    LastName = "Lee",
                    Role = UserRole.Customer,
                    Status = UserStatus.Active,
                    IsEmailVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    LastInteractionAt = DateTime.UtcNow
                },
                new User
                {
                    Email = "oliver.taylor@email.com",
                    PasswordHash = HashPassword("Customer123!"),
                    FirstName = "Oliver",
                    LastName = "Taylor",
                    Role = UserRole.Customer,
                    Status = UserStatus.Active,
                    IsEmailVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    LastInteractionAt = DateTime.UtcNow
                },
                new User
                {
                    Email = "ava.garcia@email.com",
                    PasswordHash = HashPassword("Customer123!"),
                    FirstName = "Ava",
                    LastName = "Garcia",
                    Role = UserRole.Customer,
                    Status = UserStatus.Active,
                    IsEmailVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    LastInteractionAt = DateTime.UtcNow
                },
                new User
                {
                    Email = "william.miller@email.com",
                    PasswordHash = HashPassword("Customer123!"),
                    FirstName = "William",
                    LastName = "Miller",
                    Role = UserRole.Customer,
                    Status = UserStatus.Active,
                    IsEmailVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    LastInteractionAt = DateTime.UtcNow
                },
                new User
                {
                    Email = "mia.davis@email.com",
                    PasswordHash = HashPassword("Customer123!"),
                    FirstName = "Mia",
                    LastName = "Davis",
                    Role = UserRole.Customer,
                    Status = UserStatus.Active,
                    IsEmailVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    LastInteractionAt = DateTime.UtcNow
                },
                new User
                {
                    Email = "lucas.martinez@email.com",
                    PasswordHash = HashPassword("Customer123!"),
                    FirstName = "Lucas",
                    LastName = "Martinez",
                    Role = UserRole.Customer,
                    Status = UserStatus.Active,
                    IsEmailVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    LastInteractionAt = DateTime.UtcNow
                },
                new User
                {
                    Email = "isabella.anderson@email.com",
                    PasswordHash = HashPassword("Customer123!"),
                    FirstName = "Isabella",
                    LastName = "Anderson",
                    Role = UserRole.Customer,
                    Status = UserStatus.Active,
                    IsEmailVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    LastInteractionAt = DateTime.UtcNow
                },
                new User
                {
                    Email = "ethan.thomas@email.com",
                    PasswordHash = HashPassword("Customer123!"),
                    FirstName = "Ethan",
                    LastName = "Thomas",
                    Role = UserRole.Customer,
                    Status = UserStatus.Active,
                    IsEmailVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    LastInteractionAt = DateTime.UtcNow
                }
            };

            var allUsers = staffUsers.Concat(customerUsers).ToList();
            _context.Users.AddRange(allUsers);
            _context.SaveChanges();

            _logger.LogInformation($"Added {staffUsers.Count} staff users and {customerUsers.Count} customer users");
        }

        /// <summary>
        /// Hash password using BCrypt or ASP.NET Core Identity PasswordHasher.
        /// Replace this method with your actual password hashing implementation.
        /// </summary>
        private string HashPassword(string password) //**
        {
            // Option 1: Using BCrypt.Net-Next
            // return BCrypt.Net.BCrypt.HashPassword(password);

            // Option 2: Using ASP.NET Core Identity PasswordHasher
            // var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
            // return hasher.HashPassword(null, password);

            // Placeholder - Replace with actual implementation
            throw new NotImplementedException("Implement password hashing using BCrypt or ASP.NET Core Identity PasswordHasher");
        }
    }
}