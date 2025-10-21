using Microsoft.Extensions.Logging;
using back_end.domain.Entities;
using back_end.domain.DbContexts;

namespace back_end.domain.Seeders
{
  public class DiningSessionSeeder : ISeeder
  {
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DiningSessionSeeder> _logger;

    public DiningSessionSeeder(ApplicationDbContext context, ILogger<DiningSessionSeeder> logger)
    {
      _context = context;
      _logger = logger;
    }

    public void Seed()
    {
      if (_context.DiningSessions.Any())
      {
        _logger.LogInformation("Dining sessions already seeded.");
        return;
      }

      var menu = _context.Menus.FirstOrDefault();
      if (menu == null)
      {
        _logger.LogWarning("No menus found. Cannot seed dining sessions without menus.");
        return;
      }

      var diningSessions = new List<DiningSession>
            {
                new DiningSession
                {
                    Menu_Id = menu.Menu_id,
                    Started_At = DateTime.Now.AddHours(-3),
                    // Ended_At = DateTime.Now.AddHours(-1),
                    Ended_At = null, // session ongoing
                    First_Order_At = DateTime.Now.AddHours(-2).AddMinutes(-30)
                },
                new DiningSession
                {
                    Menu_Id = menu.Menu_id,
                    Started_At = DateTime.Now.AddHours(-5),
                    Ended_At = null, // session ongoing
                    First_Order_At = DateTime.Now.AddHours(-4).AddMinutes(-45)
                }
            };

      _context.DiningSessions.AddRange(diningSessions);
      //_context.SaveChanges();

      _logger.LogInformation($"Seeded {diningSessions.Count} dining sessions.");
    }
  }
}
