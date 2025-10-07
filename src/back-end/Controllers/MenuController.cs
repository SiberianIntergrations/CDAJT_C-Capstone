using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.domain.Entities;

namespace back_end.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class MenuController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<MenuController> _logger;

        public MenuController(ApplicationContext context, ILogger<MenuController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //GET: api/menu
        //Get all Menus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Menu>>> GetAllMenu()
        {
            try
            {
                var menu = await _context.Menus
                    .Where(x => x.Is_active)
                    .ToListAsync();

                return Ok(menu);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all Menus");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}