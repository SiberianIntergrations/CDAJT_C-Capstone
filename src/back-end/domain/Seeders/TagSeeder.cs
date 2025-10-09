using Microsoft.Extensions.Logging;
using back_end.Domain.Entitys;

namespace back_end.Domain.Seeders
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
                new Tag { Name = "Spicy" },
                new Tag { Name = "Raw" },
                new Tag { Name = "Vegetarian" },
                new Tag { Name = "Vegan" },
                new Tag { Name = "Gluten-Free" },
                new Tag { Name = "Cooked" },
                new Tag { Name = "Popular" },
                new Tag { Name = "Chef's Special" },
                new Tag { Name = "Contains Shellfish" },
                new Tag { Name = "Contains Nuts" }
            };

            _context.Tags.AddRange(tags);
            _context.SaveChanges();

            _logger.LogInformation($"Added {tags.Count} tags");
        }
    }
}