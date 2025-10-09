using Microsoft.Extensions.Logging;
using back_end.Domain.Entitys;

namespace back_end.Domain.Seeders
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
                    Name = "Sushi Rolls",
                    Description = "Traditional and specialty sushi rolls",
                    AdultLimit = 12,
                    ChildLimit = 8,
                    SeniorLimit = 10,
                    TotLimit = 4
                },
                new Category
                {
                    Name = "Nigiri",
                    Description = "Hand-pressed sushi with fresh fish",
                    AdultLimit = 10,
                    ChildLimit = 6,
                    SeniorLimit = 8,
                    TotLimit = 3
                },
                new Category
                {
                    Name = "Sashimi",
                    Description = "Fresh sliced raw fish",
                    AdultLimit = 8,
                    ChildLimit = 4,
                    SeniorLimit = 6,
                    TotLimit = 2
                },
                new Category
                {
                    Name = "Appetizers",
                    Description = "Starters and small dishes",
                    AdultLimit = 6,
                    ChildLimit = 4,
                    SeniorLimit = 5,
                    TotLimit = 2
                },
                new Category
                {
                    Name = "Hot Dishes",
                    Description = "Cooked meals and hot specialties",
                    AdultLimit = 5,
                    ChildLimit = 3,
                    SeniorLimit = 4,
                    TotLimit = 2
                },
                new Category
                {
                    Name = "Desserts",
                    Description = "Sweet treats to end your meal",
                    AdultLimit = 2,
                    ChildLimit = 2,
                    SeniorLimit = 2,
                    TotLimit = 1
                }
            };

            _context.Categories.AddRange(categories);
            _context.SaveChanges();

            _logger.LogInformation($"Added {categories.Count} categories");
        }
    }
}