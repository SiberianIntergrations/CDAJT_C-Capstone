using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.domain.Entities;
using back_end.domain.enums;
using System.Linq.Expressions;
using back_end.DTO.MenuItems;
using Microsoft.AspNetCore.Authorization;
using back_end.DTO.MenuDTO;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using back_end.domain.Seeders;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using System.Formats.Asn1;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc.Routing;

namespace back_end.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class MenuLocationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MenuLocationController> _logger;
        private readonly IWebHostEnvironment _env;

        public MenuLocationController(ApplicationDbContext context, ILogger<MenuLocationController> logger, IWebHostEnvironment env)
        {
            _context = context;
            _logger = logger;
            _env = env;
        }

        [HttpPut("menu/{menu_id}/location/{location_id}")]
        public async Task<IActionResult> AddLocationToMenu(
            [FromRoute] int menu_id = -1,
            [FromRoute] int location_id = -1
        )
        {
            try
            {
                if (menu_id <= 0 || location_id <= 0)
                {
                    return BadRequest("menu_id and location_id must be positive integers.");
                }
                if (!_context.Menus.Any(m => menu_id == m.Menu_id))
                {
                    return NotFound("The menu location was not found");
                }
                if (!_context.Locations.Any(l => l.Location_Id == location_id))
                {
                    return NotFound("The location was not found.");
                }
                var menuLocation = await _context.MenuLocations.FirstOrDefaultAsync(ml => ml.Menu_Id == menu_id && ml.Location_Id == location_id);
                if (menuLocation is null)
                {
                    menuLocation = new MenuLocations
                    {
                        Menu_Id = menu_id,
                        Location_Id = location_id
                    };
                    _context.MenuLocations.Add(menuLocation);
                    await _context.SaveChangesAsync();
                }
                return Ok();

            }
            catch (Exception error)
            {
                _logger.LogError(error, "Can not Assign a menu location");
                return BadRequest(error);
            }
        }
        
        [HttpDelete("menu/{menu_id}/location/{location_id}")]
        public async Task<IActionResult> DeleteLocationToMenu(
        [FromRoute] int menu_id,
        [FromRoute] int location_id
        ){
            try
            {
                if (menu_id < 1)
                {
                    return BadRequest("No menu location");
                }
                if(location_id <1)
                {
                    return BadRequest("Menu location is Missing");
                }
                
                var menuLocation = await _context.MenuLocations.FirstOrDefaultAsync(ml => ml.Menu_Id == menu_id && ml.Location_Id == location_id);
                if (menuLocation is null)
                {
                    return NotFound("Menu and Menu Location were not found");

                };
                if (_context.OrderItems.Any(oi => oi.Menu_Id == menu_id && (oi.Order_Item_Status == OrderStatus.Pending || oi.Order_Item_Status == OrderStatus.Processing)))
                {
                    return BadRequest("Could not process request, there are open orders on this menu location.");
                }
                _context.MenuLocations.Remove(menuLocation);
                await _context.SaveChangesAsync();
                return Ok("Menu location was Removed");
                
            }
            catch (Exception Error)
            {
                _logger.LogError(Error, "Can not Assign a menu location");
                return BadRequest(Error);
            }
                
        
        }

        
    }
    


}