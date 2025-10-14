using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.domain.Entities;
using back_end.domain;
using System.Linq.Expressions;

namespace back_end.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class MenuController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MenuController> _logger;

        public MenuController(ApplicationDbContext context, ILogger<MenuController> logger)
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

        //GET: api/menu/{id}
        //Get a specific menu by its ID number
        [HttpGet("{id}")]
        public async Task<ActionResult<Menu>> GetMenuByID(int id)
        {
            try
            {
                var menu = await _context.Menus.FindAsync(id);

                //See if menu exists
                if (menu != null)
                {
                    return Ok(menu);
                }

                return NotFound(new { message = $"Can't find menu ID: {id}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error finding menu with ID {id}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        //GET: api/menu/{time}
        //Gets the correct menu for the current time or inputted time
        [HttpGet("time")]
        public async Task<ActionResult<IEnumerable<Menu>>> GetMenuByTime([FromQuery] string? time)
        {
            TimeOnly currentTime;

            try
            {
                //Check if time was given
                if (string.IsNullOrEmpty(time))
                {
                    currentTime = TimeOnly.FromDateTime(DateTime.Now);
                }
                //Check if time given is proper
                else if (!TimeOnly.TryParse(time, out currentTime))
                {
                    return BadRequest(new { message = "Imporper time given." });
                }
                //Get the correct menu for time slot
                //*note to self* Test this part more
                var currentMenu = await _context.Menus.Where(m => m.Is_active &&
                                            m.Start_time <= currentTime &&
                                            m.End_time >= currentTime)
                                        .ToListAsync();

                return Ok(currentMenu);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting the current Menu");
                return StatusCode(500, "Internal Server Error");
            }
        }

        //GET: api/menu/{menu}/items
        //Get items from a Menu
        //Display: price, category and diet tags
        [HttpGet("{menu}/item")]
        public async Task<ActionResult<IEnumerable<object>>> GetItemsByMenu(int menu)
        {
            try
            {
                var currentMenu = await _context.Menus.FindAsync(menu);

                //Check if menu exists
                if (currentMenu == null)
                {
                    return NotFound(new { message = $"Can't find Menu with ID:{menu}" });
                }
                else
                {
                    //Get all details, category and tags for all menu items
                    var itemInfo = await _context.MenuItemAssignments
                            .Where(m => m.Menu_Id == menu && m.Status == MenuItemStatus.Available)
                            .Include(m => m.MenuItem)
                                .ThenInclude(me => me.Category)
                            .Include(m => m.MenuItem)
                                .ThenInclude(me => me.MenuItemTags)
                                .ThenInclude(me => me.Tag)
                            .ToListAsync();

                    //Arrange details into a user friendly fashion
                    //Only takes tag name and colour
                    var menuItems = itemInfo.Select(m => new
                    {
                        itemId = m.Item_Id,
                        name = m.MenuItem.Name,
                        description = m.MenuItem.Description,
                        category = m.MenuItem.Category.Category_name,
                        price = m.Price,
                        imageURL = m.MenuItem.image_url,
                        tags = m.MenuItem.MenuItemTags.
                            Select(me => new
                            {
                                name = me.Tag.tag_name,
                                color = me.Tag.tag_color
                            }).ToList()
                    }).ToList();

                    return Ok(menuItems);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $" Can't get items for menu {menu}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        //GET: api/menu/location
        //Get menus available only at a specific location
        [HttpGet("location/{location}")]

        public async Task<ActionResult<IEnumerable<Menu>>> GetMenuByLocation(int location)
        {
            try
            {
                var locationID = await _context.Locations.FindAsync(location);

                //Check if location exists
                if (locationID == null)
                {

                    return NotFound(new { message = $"Can't find location with ID: {location}" });
                }

                //Get all the menus at the inputted location
                var menus = await _context.MenuLocations
                                .Where(m => m.Location_Id == location)
                                .Include(m => m.Menu)
                                .Where(m => m.Menu.Is_active)
                                .Select(m => m.Menu)
                                .ToListAsync();

                return Ok(menus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Can't get menus for location at ID: {location}");
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}