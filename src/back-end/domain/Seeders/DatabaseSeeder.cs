using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using back_end.domain.Entities;
using back_end.domain.DbContexts;

namespace back_end.domain.Seeders
{
  /// <summary>
  /// Main database seeder orchestrator that coordinates all seeding operations.
  /// Handles seeding all tables with initial data in the correct dependency order.
  /// </summary>
  public class DatabaseSeeder
  {
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    private readonly CategorySeeder _categorySeeder;
    private readonly TagSeeder _tagSeeder;
    private readonly MenuSeeder _menuSeeder;
    private readonly UserSeeder _userSeeder;
    private readonly MenuItemSeeder _menuItemSeeder;
    private readonly TableSeeder _tableSeeder;
    private readonly TableGroupSeeder _tableGroupSeeder;
    private readonly LocationSeeder _locationSeeder;
    private readonly DiningSessionSeeder _diningSessionSeeder;
    private readonly SessionParticipantSeeder _sessionParticipantSeeder;
    private readonly BillSeeder _billSeeder;
    private readonly SessionOrderSeeder _sessionOrderSeeder;
    private readonly OrderItemSeeder _orderItemSeeder;
    private readonly ServiceRequestSeeder _serviceRequestSeeder;

    public DatabaseSeeder(
        ApplicationDbContext context,
        ILogger<DatabaseSeeder> logger,
        CategorySeeder categorySeeder,
        TagSeeder tagSeeder,
        MenuSeeder menuSeeder,
        UserSeeder userSeeder,
        MenuItemSeeder menuItemSeeder,
        TableSeeder tableSeeder,
        TableGroupSeeder tableGroupSeeder,
        LocationSeeder locationSeeder,
        DiningSessionSeeder diningSessionSeeder,
        SessionParticipantSeeder sessionParticipantSeeder,
        BillSeeder billSeeder,
        SessionOrderSeeder sessionOrderSeeder,
        OrderItemSeeder orderItemSeeder,
        ServiceRequestSeeder serviceRequestSeeder)
    {
      _context = context;
      _logger = logger;

      _categorySeeder = categorySeeder;
      _tagSeeder = tagSeeder;
      _menuSeeder = menuSeeder;
      _userSeeder = userSeeder;
      _menuItemSeeder = menuItemSeeder;
      _tableSeeder = tableSeeder;
      _tableGroupSeeder = tableGroupSeeder;
      _locationSeeder = locationSeeder;
      _diningSessionSeeder = diningSessionSeeder;
      _sessionParticipantSeeder = sessionParticipantSeeder;
      _billSeeder = billSeeder;
      _sessionOrderSeeder = sessionOrderSeeder;
      _orderItemSeeder = orderItemSeeder;
      _serviceRequestSeeder = serviceRequestSeeder;
    }

    /// <summary>Seeds all tables with initial data.</summary>
    public async Task SeedDatabase(bool reset = false)
    {
      try
      {
        if (reset)
        {
          ClearExistingData();
        }

        // Base data in dependency order
        _userSeeder.Seed(); _logger.LogInformation("Users seeded successfully");
        await _context.SaveChangesAsync();
        _categorySeeder.Seed(); _logger.LogInformation("Categories seeded successfully");
        await _context.SaveChangesAsync();
        _tagSeeder.Seed(); _logger.LogInformation("Tags seeded successfully");
        await _context.SaveChangesAsync();
        _menuSeeder.Seed(); _logger.LogInformation("Menus seeded successfully");
        await _context.SaveChangesAsync();
        _menuItemSeeder.Seed(); _logger.LogInformation("Menu items and assignments seeded successfully");
        await _context.SaveChangesAsync();
        _tableSeeder.Seed(); _logger.LogInformation("Tables seeded successfully");
        await _context.SaveChangesAsync();
        _tableGroupSeeder.Seed(); _logger.LogInformation("Table groups seeded successfully");
        await _context.SaveChangesAsync();
        _locationSeeder.Seed(); _logger.LogInformation("Locations seeded successfully");

        await _context.SaveChangesAsync();

        // Sessions then session-driven data
        _diningSessionSeeder.Seed(); _logger.LogInformation("Dining sessions seeded successfully");
        await _context.SaveChangesAsync();
        _sessionParticipantSeeder.Seed(); _logger.LogInformation("Session participants seeded successfully");
        await _context.SaveChangesAsync();
        _billSeeder.Seed(); _logger.LogInformation("Bills seeded successfully");
        await _context.SaveChangesAsync();
        _sessionOrderSeeder.Seed(); _logger.LogInformation("Session orders seeded successfully");
        await _context.SaveChangesAsync();
        _orderItemSeeder.Seed(); _logger.LogInformation("Order items seeded successfully");
        await _context.SaveChangesAsync();
        _serviceRequestSeeder.Seed(); _logger.LogInformation("Service requests seeded successfully");

        await _context.SaveChangesAsync();
        _logger.LogInformation("All data seeding completed successfully");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error during seeding");
        throw;
      }
    }

    /// <summary>Verifies that all tables have been seeded correctly.</summary>
    public bool VerifySeeding()
    {
      try
      {
        var categories = _context.Categories.Count();
        var tags = _context.Tags.Count();
        var menus = _context.Menus.Count();
        var menuItems = _context.MenuItems.Count();
        var assignments = _context.MenuItemAssignments.Count();
        var tables = _context.Tables.Count();
        var tableGroups = _context.TableGroups.Count();
        var users = _context.Users.Count();
        var diningSessions = _context.DiningSessions.Count();
        var activeSessions = _context.DiningSessions.Count(ds => ds.Ended_At == null);
        var participants = _context.SessionParticipants.Count();
        var bills = _context.Bills.Count();
        var sessionOrders = _context.SessionOrders.Count();
        var orderItems = _context.OrderItems.Count();
        var serviceRequests = _context.ServiceRequests.Count();

        // Log counts
        _logger.LogInformation($"Categories: {categories}");
        _logger.LogInformation($"Tags: {tags}");
        _logger.LogInformation($"Menus: {menus}");
        _logger.LogInformation($"Menu Items: {menuItems}");
        _logger.LogInformation($"Menu Item Assignments: {assignments}");
        _logger.LogInformation($"Tables: {tables}");
        _logger.LogInformation($"Table Groups: {tableGroups}");
        _logger.LogInformation($"Users: {users}");
        _logger.LogInformation($"Dining Sessions: {diningSessions} (Active: {activeSessions})");
        _logger.LogInformation($"SessionParticipants: {participants}");
        _logger.LogInformation($"Bills: {bills}");
        _logger.LogInformation($"SessionOrders: {sessionOrders}");
        _logger.LogInformation($"OrderItems: {orderItems}");
        _logger.LogInformation($"ServiceRequests: {serviceRequests}");

        var verification = new List<bool>
                {
                    categories >= 6,                   // At least 6 categories
                    tags >= 10,                        // At least 10 tags
                    menus >= 2,                        // At least 2 menus
                    menuItems >= 90,                   // At least 15 items per category
                    assignments >= menuItems * 2,      // Each item should be in both menus
                    tables == 34,                      // Exactly 34 tables
                    tableGroups == 4,                  // Exactly 4 table groups
                    users >= 13,                       // Admin + 3 staff + 10 customers (13 total)
                    diningSessions >= 50,              // At least 50 sessions
                    activeSessions == 5                // Exactly 5 active sessions
                };

        var allValid = verification.All(v => v);
        if (!allValid)
          _logger.LogWarning("Seeding verification failed. Some counts do not meet expected values.");

        return allValid;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error during verification");
        return false;
      }
    }

    /// <summary>Clears all existing data from the database.</summary>
    private void ClearExistingData()
    {
      _logger.LogInformation("Clearing existing data...");

      // Delete in reverse dependency order
      _context.ServiceRequests.RemoveRange(_context.ServiceRequests);
      _context.OrderItems.RemoveRange(_context.OrderItems);
      _context.SessionOrders.RemoveRange(_context.SessionOrders);
      _context.Bills.RemoveRange(_context.Bills);
      _context.SessionParticipants.RemoveRange(_context.SessionParticipants);
      _context.DiningSessions.RemoveRange(_context.DiningSessions);
      _context.TableGroups.RemoveRange(_context.TableGroups);
      _context.MenuItemAssignments.RemoveRange(_context.MenuItemAssignments);
      _context.MenuItems.RemoveRange(_context.MenuItems);
      _context.Menus.RemoveRange(_context.Menus);
      _context.Categories.RemoveRange(_context.Categories);
      _context.Tags.RemoveRange(_context.Tags);
      _context.Tables.RemoveRange(_context.Tables);
      _context.Users.RemoveRange(_context.Users);

      _context.SaveChanges();
      _logger.LogInformation("Existing data cleared");
    }
  }
}