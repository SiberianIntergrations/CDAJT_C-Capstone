using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.domain.Entities;
using back_end.DTO.Category;
using Microsoft.AspNetCore.Http.Features;

namespace back_end.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class CategoryController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ApplicationContext context, ILogger<CategoryController> logger)
        {
            _context = context;
            _logger = logger;
        }
        //GET api/category
        //Get all categoeies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCategories()
        {
            try
            {
                var category = await _context.Categories.ToListAsync();
                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get all Categories");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategory category)
        {
            try
            {
                var newCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Category_name == category.Name);
                if (newCategory != null)
                {
                    return Conflict("Category with the same name already exists.");
                }

                var categoryEntity = new Category
                {
                    Category_name = category.Name ?? string.Empty,
                    Description = category.Description ?? string.Empty,
                    adult_limit = category.Adult_Limit,
                    child_limit = category.Child_Limit,
                    senior_limit = category.Senior_Limit,
                    total_limit = category.Total_Limit,
                    last_viewed_at = DateTime.UtcNow
                };

                _context.Categories.Add(categoryEntity);
                await _context.SaveChangesAsync();

                return Ok(categoryEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot create Category");
                return StatusCode(500, "Internal Server Error");
            }


        }

        [HttpGet("get_category{_categoryId}")]
        public async Task<IActionResult> GetCategoryById(int _categoryId)
        {
            try
            {
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Category_id == _categoryId);

                if (category == null)
                {
                    return NotFound("Category not found.");
                }

                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get Category by ID");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("update_category{_categoryId}")]
        public async Task<IActionResult> UpdateCategory(int _categoryId, UpdateCategory updatedCategory)
        {
            try
            {
                var existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Category_id == _categoryId);

                if (existingCategory == null)
                {
                    return NotFound("Category not found.");
                }
                existingCategory.Category_name = updatedCategory.Name ?? existingCategory.Category_name;
                existingCategory.Description = updatedCategory.Description ?? existingCategory.Description;
                existingCategory.adult_limit = updatedCategory.Adult_Limit;
                existingCategory.child_limit = updatedCategory.Child_Limit;
                existingCategory.senior_limit = updatedCategory.Senior_Limit;
                existingCategory.total_limit = updatedCategory.Total_Limit;
                existingCategory.last_viewed_at = DateTime.UtcNow;
                _context.Categories.Update(existingCategory);
                await _context.SaveChangesAsync();
                return Ok(existingCategory);


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot update Category");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("delete_category{_categoryId}")]
        public async Task<IActionResult> DeleteCategory(int _categoryId)
        {
            try
            {
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Category_id == _categoryId);

                if (category == null)
                {
                    return NotFound("Category not found.");
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return Ok("Category deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot delete Category");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("get_menu_by_category{_categoryId}")]
        public async Task<IActionResult> GetMenuItemsByCategory(int _categoryId, int skip = 0, int limit = 10)
        {
            try
            {
                var menuItems = await _context.MenuItems
                    .Where(m => m.Category_id == _categoryId)
                    .Skip(skip)
                    .Take(limit)
                    .ToListAsync();

                if (menuItems == null || menuItems.Count == 0)
                {
                    return NotFound("No menu items found for the specified category.");
                }

                return Ok(menuItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get Menu Items by Category ID");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("track_viewed{_categoryId}")]
        public async Task<IActionResult> TrackCategoryView(int _categoryId, [FromQuery(Name = "view_seconds")] int viewSeconds)
        {
            try
            {
                if (viewSeconds < 0)
                {
                    return BadRequest("view_seconds must be a non-negative integer.");
                }
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Category_id == _categoryId);

                if (category == null)
                {
                    return NotFound("Category not found.");
                }

                category.last_viewed_at = DateTime.UtcNow;
                category.total_view_seconds += viewSeconds;
                category.total_views += 1;
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
                return Ok("Category view tracked successfully.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot track Category view");
                return StatusCode(500, "Internal Server Error");
            }
        }

        //This endpoint is in the python project with notes that it needs parameters to be added to menu item model, comment out as 
        // it is not being used and would cause errors if called but included for future implementation.
        
        // [HttpPut("reorder-items{_categoryId}")]
        

        // public async Task<IActionResult> ReorderMenuItems(int categoryId, [FromBody] List<int> itemIds)
        // {
        //     if (itemIds == null || itemIds.Count == 0)
        //         return BadRequest("itemIds cannot be empty.");

        //     if (itemIds.Count != itemIds.Distinct().Count())
        //         return BadRequest("itemIds contains duplicates.");

        //     var categoryExists = await _context.Categories
        //         .AsNoTracking()
        //         .AnyAsync(c => c.Category_id == categoryId);

        //     if (!categoryExists)
        //         return NotFound("Category not found.");

        //     var fetchedItems = await _context.MenuItems
        //         .Where(mi => mi.Category_id == categoryId && itemIds.Contains(mi.item_id))
        //         .ToListAsync();

        //     if (fetchedItems.Count != itemIds.Count)
        //         return BadRequest("One or more itemIds are invalid or not in this category.");

        //     // Map ItemId -> index (0-based; use idx + 1 if you want 1-based)
        //     var orderMap = itemIds
        //         .Select((id, idx) => new { id, idx })
        //         .ToDictionary(x => x.id, x => x.idx);

        //     foreach (var item in fetchedItems)
        //         item.DisplayOrder = orderMap[item.item_id];

        //     try
        //     {
        //         await _context.SaveChangesAsync();
        //         return Ok(new { message = "Menu items reordered successfully", updated = fetchedItems.Count });
        //     }
        //     catch (DbUpdateException)
        //     {
        //         // If you added a unique index on (CategoryId, DisplayOrder), concurrency could trigger this
        //         return Conflict("Could not reorder due to a constraint/concurrency issue. Please retry.");
        //     }
        // }

        
    }
}