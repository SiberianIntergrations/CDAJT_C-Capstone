using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.domain.Entities;

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
    }
}