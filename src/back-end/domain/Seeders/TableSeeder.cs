using Microsoft.Extensions.Logging;
using back_end.domain.Entities;
using back_end.domain.DbContexts;

namespace back_end.domain.Seeders
{
    /// <summary>
    /// Seeder for the table_entity table with initial data.
    /// Creates 34 tables total:
    /// - 9 2-tops (tables 1-9)
    /// - 25 4-tops (tables 10-34)
    /// </summary>
    public class TableSeeder : ISeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TableSeeder> _logger;

        public TableSeeder(ApplicationDbContext context, ILogger<TableSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Seed()
        {
            var tables = new List<TableEntity>();

            // Add 9 2-tops (tables 1-9)
            for (int i = 1; i <= 9; i++)
            {
                tables.Add(new TableEntity
                {
                    table_number = i,
                    seat_count = 2,
                    is_active = true
                });
            }

            // Add 25 4-tops (tables 10-34)
            for (int i = 10; i <= 34; i++)
            {
                tables.Add(new TableEntity
                {
                    table_number = i,
                    seat_count = 4,
                    is_active = true
                });
            }

            _context.Tables.AddRange(tables);
            //_context.SaveChanges();

            _logger.LogInformation($"Added {tables.Count} tables (2-tops: tables 1-9, 4-tops: tables 10-34)");
        }
    }
}
