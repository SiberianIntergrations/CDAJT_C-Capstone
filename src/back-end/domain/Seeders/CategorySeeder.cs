using Microsoft.Extensions.Logging;
using back_end.domain.Entities;
using back_end.domain.DbContexts;

namespace back_end.domain.Seeders
{
    /// <summary>
    /// Seeder for the categories table with initial data.
    /// </summary>
    public class CategorySeeder : ISeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategorySeeder> _logger;

        public CategorySeeder(ApplicationDbContext context, ILogger<CategorySeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Seed()
        {
            var categories = new List<Category>
            {
                new Category
                {
                    Category_name = "Sushi Rolls",
                    Description = "Traditional and specialty sushi rolls",
                    adult_limit = 12,
                    child_limit = 8,
                    senior_limit = 10,
                    total_limit = 4
                },
                new Category
                {
                    Category_name = "Nigiri",
                    Description = "Hand-pressed sushi with fresh fish",
                    adult_limit = 10,
                    child_limit = 6,
                    senior_limit = 8,
                    total_limit = 3
                },
                new Category
                {
                    Category_name = "Sashimi",
                    Description = "Fresh sliced raw fish",
                    adult_limit = 8,
                    child_limit = 4,
                    senior_limit = 6,
                    total_limit = 2
                },
                new Category
                {
                    Category_name = "Appetizers",
                    Description = "Starters and small dishes",
                    adult_limit = 6,
                    child_limit = 4,
                    senior_limit = 5,
                    total_limit = 2
                },
                new Category
                {
                    Category_name = "Hot Dishes",
                    Description = "Cooked meals and hot specialties",
                    adult_limit = 5,
                    child_limit = 3,
                    senior_limit = 4,
                    total_limit = 2
                },
                new Category
                {
                    Category_name = "Desserts",
                    Description = "Sweet treats to end your meal",
                    adult_limit = 2,
                    child_limit = 2,
                    senior_limit = 2,
                    total_limit = 1
                }
            };

            _context.Categories.AddRange(categories);
            //_context.SaveChanges();

            // _logger.LogInformation($"Added {categories.Count} categories");
        }
    }
}
