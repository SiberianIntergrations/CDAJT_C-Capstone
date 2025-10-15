

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.domain.Entities;
using back_end.domain;
using System.Linq.Expressions;
using back_end.DTO.MenuItems;
using Microsoft.AspNetCore.Authorization;
using back_end.DTO.MenuDTO;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using back_end.domain.Seeders;

namespace back_end.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class MenuItemController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MenuController> _logger;

        public MenuItemController(ApplicationDbContext context, ILogger<MenuController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPost("/")]
        public async Task<IActionResult> Create_Menu_Item(
            MenuItemCreateDTO item_data
        )
        {
            try
            {
                if (await _context.MenuItems.AnyAsync(mi => mi.Name == item_data.Name))
                {
                    return Conflict("Menu Item already Exist");
                }
                Menu_Item newMenuItem = new Menu_Item
                {
                    Name = item_data.Name,
                    Description = item_data.Description ?? string.Empty,
                    Category_id = item_data.Category_Id,
                    image_url = item_data.Item_Image_Url,
                    Status = MenuItemStatus.Available
                };
                if (item_data.Tag_Ids.Count > 0)
                {
                    var tags = await _context.Tags.Where(t => item_data.Tag_Ids.Contains(t.tag_id)).ToListAsync();
                    foreach (var tag in tags)
                    {
                        newMenuItem.MenuItemTags.Add(new MenuItemTag
                        {
                            Tag = tag,
                            MenuItem = newMenuItem
                        });
                    }
                }
                _context.Add(newMenuItem);
                await _context.SaveChangesAsync();
                return Ok(new MenuItemResponseDTO
                {
                    Name = newMenuItem.Name,
                    Description = newMenuItem.Description,
                    Category_Id = newMenuItem.Category_id,
                    Item_Image_Url = newMenuItem.image_url,
                    Status = newMenuItem.Status,
                    Item_Id = newMenuItem.item_id,
                    Tags = newMenuItem.MenuItemTags.Select(t => new FullTagResponseDTO
                    {
                        Tag_Id = t.Tag.tag_id,
                        Name = t.Tag.tag_name,
                        Color_Code = t.Tag.tag_color
                    }).ToList(),
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active session");
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        
        
    }

}