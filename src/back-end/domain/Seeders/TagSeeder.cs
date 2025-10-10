using Microsoft.Extensions.Logging;
using back_end.domain.Entities;

namespace back_end.domain.Seeders
{
    /// <summary>
    /// Seeder for the tags table with initial data.
    /// </summary>
    public class TagSeeder : ISeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TagSeeder> _logger;

        public TagSeeder(ApplicationDbContext context, ILogger<TagSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Seed()
        {
            var tags = new List<Tag>
            {
            new Tag { Name = "Spicy", ColorCode = "#99afee" },
            new Tag { Name = "Raw", ColorCode = "#feb3b4" },
            new Tag { Name = "Vegetarian", ColorCode = "#f1b6c3" },
            new Tag { Name = "Vegan", ColorCode = "#d891f1" },
            new Tag { Name = "Gluten-Free", ColorCode = "#f1a8b9" },
            new Tag { Name = "Cooked", ColorCode = "#ddbdf5" },
            new Tag { Name = "Popular", ColorCode = "#bafaf4" },
            new Tag { Name = "Chef's Special", ColorCode = "#d7f9d4" },
            new Tag { Name = "Contains Shellfish", ColorCode = "#f4f1ad" },
            new Tag { Name = "Contains Nuts", ColorCode = "#eff6d4" }
        };

            _context.Tags.AddRange(tags);
            _context.SaveChanges();

            _logger.LogInformation($"Added {tags.Count} tags");
        }
    }
}