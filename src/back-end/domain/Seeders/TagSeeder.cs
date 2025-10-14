using Microsoft.Extensions.Logging;
using back_end.domain.Entities;
using back_end.domain.DbContexts;

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
                new Tag { tag_name = "Spicy", tag_color = "#99afee" },
                new Tag { tag_name = "Raw", tag_color = "#feb3b4" },
                new Tag { tag_name = "Vegetarian", tag_color = "#f1b6c3" },
                new Tag { tag_name = "Vegan", tag_color = "#d891f1" },
                new Tag { tag_name = "Gluten-Free", tag_color = "#f1a8b9" },
                new Tag { tag_name = "Cooked", tag_color = "#ddbdf5" },
                new Tag { tag_name = "Popular", tag_color = "#bafaf4" },
                new Tag { tag_name = "Chef's Special", tag_color = "#d7f9d4" },
                new Tag { tag_name = "Contains Shellfish", tag_color = "#f4f1ad" },
                new Tag { tag_name = "Contains Nuts", tag_color = "#eff6d4" }
            };


            _context.Tags.AddRange(tags);
            //_context.SaveChanges();

            _logger.LogInformation($"Added {tags.Count} tags");
        }
    }
}