using Microsoft.Extensions.Logging;
using back_end.domain.Entities;
using back_end.domain.DbContexts;

namespace back_end.domain.Seeders
{
    /// <summary>
    /// Seeder for the locations table with initial data.
    /// </summary>
    public class LocationSeeder : ISeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LocationSeeder> _logger;

        public LocationSeeder(ApplicationDbContext context, ILogger<LocationSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Seed()
        {
            var locations = new List<Locations>
            {
                new Locations
                {
                    Name = "North-West Branch",
                    Address_Primary = "13619 St Albert Trail NW",
                    City = "Edmonton",
                    Province = "Alberta",
                    Postal_Code = "T5L 5E8",
                    Phone_Number = "780-488-6610",
                    Created_At = new DateTime(2025, 10, 1)
                },
                new Locations
                {
                    Name = "West End Branch",
                    Address_Primary = "456 West Ave",
                    City = "Edmonton",
                    Province = "Alberta",
                    Postal_Code = "T5L 3R8",
                    Phone_Number = "780-555-0102",
                    Created_At = new DateTime(2024, 1, 1)
                },
                new Locations
                {
                    Name = "South Side Branch",
                    Address_Primary = "789 South Rd",
                    City = "Edmonton",
                    Province = "Alberta",
                    Postal_Code = "T6G 1M2",
                    Phone_Number = "780-555-0103",
                    Created_At = new DateTime(2024, 1, 1)
                }
            };

            _context.Locations.AddRange(locations);
            _context.SaveChanges();

            _logger.LogInformation($"Added {locations.Count} locations");
        }
    }
}
