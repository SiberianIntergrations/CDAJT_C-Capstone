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
                    Category_Name = "Sushi Rolls",
                    Description = "Traditional and specialty sushi rolls",
                    Adult_Limit = 12,
                    Child_Limit = 8,
                    Senior_Limit = 10,
                    Total_Limit = 4
                },
                new Category
                {
                    Category_Name = "Nigiri",
                    Description = "Hand-pressed sushi with fresh fish",
                    Adult_Limit = 10,
                    Child_Limit = 6,
                    Senior_Limit = 8,
                    Total_Limit = 3
                },
                new Category
                {
                    Category_Name = "Sashimi",
                    Description = "Fresh sliced raw fish",
                    Adult_Limit = 8,
                    Child_Limit = 4,
                    Senior_Limit = 6,
                    Total_Limit = 2
                },
                new Category
                {
                    Category_Name = "Appetizers",
                    Description = "Starters and small dishes",
                    Adult_Limit = 6,
                    Child_Limit = 4,
                    Senior_Limit = 5,
                    Total_Limit = 2
                },
                new Category
                {
                    Category_Name = "Hot Dishes",
                    Description = "Cooked meals and hot specialties",
                    Adult_Limit = 5,
                    Child_Limit = 3,
                    Senior_Limit = 4,
                    Total_Limit = 2
                },
                new Category
                {
                    Category_Name = "Desserts",
                    Description = "Sweet treats to end your meal",
                    Adult_Limit = 2,
                    Child_Limit = 2,
                    Senior_Limit = 2,
                    Total_Limit = 1
                }
            };

            _context.Categories.AddRange(categories);
            _context.SaveChanges();

            _logger.LogInformation($"Added {categories.Count} categories");
        }
    }
}