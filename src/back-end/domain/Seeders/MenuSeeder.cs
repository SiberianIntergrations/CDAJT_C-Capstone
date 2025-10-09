using Microsoft.Extensions.Logging;
using back_end.domain.Entities;

namespace back_end.domain.Seeders
{
    /// <summary>
    /// Seeder for the menu table with initial data.
    /// </summary>
    public class MenuSeeder : ISeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MenuSeeder> _logger;

        public MenuSeeder(ApplicationDbContext context, ILogger<MenuSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Seed()
        {
            var menus = new List<Menu>
            {
                new Menu
                {
                    Name = "All Day Menu",
                    Description = "Our complete selection of dishes available all day",
                    StartTime = new TimeSpan(11, 0, 0),  // 11:00 AM
                    EndTime = new TimeSpan(22, 0, 0),    // 10:00 PM
                    IsActive = true
                },
                new Menu
                {
                    Name = "Lunch Special",
                    Description = "Special lunch menu with selected items",
                    StartTime = new TimeSpan(11, 0, 0),  // 11:00 AM
                    EndTime = new TimeSpan(15, 0, 0),    // 3:00 PM
                    IsActive = true
                }
            };

            _context.Menus.AddRange(menus);
            _context.SaveChanges();

            _logger.LogInformation($"Added {menus.Count} menus");
        }
    }
}