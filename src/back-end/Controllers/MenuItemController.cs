

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
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using System.Formats.Asn1;
using System.Security.Cryptography.X509Certificates;

namespace back_end.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class MenuItemController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MenuController> _logger;
        private readonly IWebHostEnvironment _env;

        public MenuItemController(ApplicationDbContext context, ILogger<MenuController> logger, IWebHostEnvironment env)
        {
            _context = context;
            _logger = logger;
            _env = env;
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
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
        [Authorize]
        [HttpGet("/item_id")]
        public async Task<IActionResult> Get_Menu_Item(
            int item_id
        )
        {
            try
            {
                var item = _context.MenuItems.Include(mi => mi.MenuItemTags).FirstOrDefaultAsync(mi => mi.item_id == item_id);
                if (item == null)
                {
                    return NotFound(" There was not menu Item with the information provided");
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active session");
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> List_Menu_Items(

        )
        {
            try
            {
                var allItems = await _context.MenuItems.Include(mi => mi.MenuItemTags).ToListAsync();
                return Ok(allItems);


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active session");
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPut("/{item_id}")]
        public async Task<IActionResult> Update_Menu_Item(
            int item_id,
            MenuItemUpdateDTO menuItemUpdate
        )
        {
            try
            {
                var itemUpdate = await _context.MenuItems.FirstOrDefaultAsync(mi => mi.item_id == item_id);
                if (itemUpdate == null)
                {
                    return NotFound("Menu Item was Not Found ");
                }
                if (menuItemUpdate.Name != null)
                {
                    itemUpdate.Name = menuItemUpdate.Name;
                }
                if (menuItemUpdate.Description != null)
                {
                    itemUpdate.Description = menuItemUpdate.Description;
                }
                if (menuItemUpdate.Category_Id != null)
                {
                    itemUpdate.Category_id = (int)menuItemUpdate.Category_Id;
                }
                if (menuItemUpdate.Item_Image_Url != null)
                {
                    itemUpdate.image_url = menuItemUpdate.Item_Image_Url;
                }
                if (menuItemUpdate.Status != null)
                {
                    itemUpdate.Status = (MenuItemStatus)menuItemUpdate.Status;
                }
                if (menuItemUpdate != null && menuItemUpdate.Tag_Ids != null && menuItemUpdate.Tag_Ids.Count > 0)
                {
                    await _context.Entry(itemUpdate).Collection(i => i.MenuItemTags).LoadAsync();
                    foreach (var tagId in menuItemUpdate.Tag_Ids.Distinct())
                    {
                        itemUpdate.MenuItemTags.Add(new MenuItemTag
                        {
                            Menu_item_id = itemUpdate.item_id,
                            Tag_id = tagId
                        });
                    }
                    ;
                }
                ;
                _context.Add(itemUpdate);
                await _context.SaveChangesAsync();
                return Ok(new MenuItemResponseDTO
                {
                    Name = itemUpdate.Name,
                    Description = itemUpdate.Description,
                    Category_Id = itemUpdate.Category_id,
                    Item_Image_Url = itemUpdate.image_url,
                    Status = itemUpdate.Status,
                    Item_Id = itemUpdate.item_id,
                    Tags = itemUpdate.MenuItemTags.Select(t => new FullTagResponseDTO
                    {
                        Tag_Id = t.Tag.tag_id,
                        Name = t.Tag.tag_name,
                        Color_Code = t.Tag.tag_color
                    }).ToList()
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active session");
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost("{item_id:int}/image")]                    // route param matches method param
        [RequestSizeLimit(5 * 1024 * 1024)]                  // 5 MB (1024, not 1025)
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload_Item_Image(
            int item_id,
            IFormFile file,
            CancellationToken ct)
        {
            try
            {
                // 1) Fetch item
                var item = await _context.MenuItems
                    .FirstOrDefaultAsync(mi => mi.item_id == item_id, ct); // ensure property name matches your model
                if (item is null) return NotFound("Menu item not found.");

                // 2) Basic file checks
                if (file is null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                const long MAX_BYTES = 5 * 1024 * 1024;
                if (file.Length > MAX_BYTES)
                    return StatusCode(StatusCodes.Status413PayloadTooLarge, "File too large.");

                var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "image/jpeg", "image/png", "image/webp"
                };
                if (!allowed.Contains(file.ContentType))
                    return BadRequest("Unsupported image type. Use JPEG/PNG/WEBP.");

                // 3) Build file name + extension
                var ext = file.ContentType switch
                {
                    "image/jpeg" => ".jpg",
                    "image/png" => ".png",
                    "image/webp" => ".webp",
                    _ => ".bin"
                };
                var fileName = $"item_{item_id}_{Guid.NewGuid():N}{ext}";

                // 4) Resolve target folder (front-end/public/menu-items)
                var backendRoot = _env.ContentRootPath; // .../src/back-end
                var frontEndFolder = Path.GetFullPath(
                    Path.Combine(backendRoot, "..", "front-end", "public", "menu-items")
                ); // .../src/front-end/public/menu-items

                if (!Directory.Exists(frontEndFolder))
                    Directory.CreateDirectory(frontEndFolder);

                // 5) Save file to disk
                var savePath = Path.Combine(frontEndFolder, fileName);            // filesystem path

                // 6) Persist and return a public URL (your FE will serve from /menu-items/*)
                var publicUrl = $"/menu-items/{fileName}".Replace("\\", "/");
                item.image_url = publicUrl;

                await _context.SaveChangesAsync(ct);

                return Ok(new { item_id, image_url = publicUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Saving the File selected");
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }

        }


        [Authorize(Roles = "Admin,Staff")]
        [HttpDelete("{item_id}")]
        public async Task<IActionResult> Delete_Menu_item(
            int item_id
        )
        {
            try
            {
                var item = await _context.MenuItems.FirstAsync(mi => mi.item_id == item_id);
                if (item is null)
                {
                    return NotFound("Item was Not found");
                }
                if (item.MenuAssignments.Count > 0)
                {
                    return BadRequest("Can not Delete Item if there are Items Ordered Check Orders");
                }
                var backendRoot = _env.ContentRootPath; // .../src/back-end
                var frontEndFolder = Path.GetFullPath(
                    Path.Combine(backendRoot, "..", "front-end", "public", "menu-items")
                );

                string fileName = Path.GetFileName(item.image_url);
                var ImagePath = Path.Combine(frontEndFolder, fileName);
                if (!System.IO.File.Exists(ImagePath))
                {
                    return BadRequest(ImagePath);
                }
                else
                {
                    System.IO.File.Delete(ImagePath);
                }
                _context.Remove(item);
                await _context.SaveChangesAsync();
                return Ok("Image Was Deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Deleting the File selected");
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }

        }
        [Authorize(Roles = "Admin,Staff")]
        [HttpPut("{item_id}")]
        public async Task<IActionResult> Update_Item_Status(
            int item_id,
            MenuItemStatus menu_status
        )
        {
            try
            {
                var item = await _context.MenuItems.FirstOrDefaultAsync(mi => mi.item_id == item_id);
                if (item is null)
                {
                    return NotFound("menu item was not found");
                }
                item.Status = menu_status;
                _context.Update(item);
                await _context.SaveChangesAsync();
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Updating the menu Item Status");
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }



        [Authorize]
        [HttpGet("{item_id}/tags")]
        public async Task<IActionResult> Get_Item_Tags(
            int item_id,
            MenuItemStatus menu_status
        )
        {
            try
            {
                var menu_item = _context.MenuItems.FirstOrDefault(mi => mi.item_id == item_id);
                if (menu_item is null)
                {
                    return NotFound("Menu Item was not found");
                }
                var tags = await _context.Tags.Where(t => t.MenuItemTags.Any(mt => mt.Menu_item_id == item_id)).OrderBy(t => t.tag_name).ToListAsync();
                var response = tags.Select(t => new TagResponseDTO
                {
                    Name = t.tag_name,
                    Tag_Id = t.tag_id
                }).ToList();
                
                return Ok(response);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Getting Menu Item Tags");
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        
        
        [Authorize]
        [HttpGet("{item_id}/tags-with-colors")]
        public async Task<IActionResult> Get_Item_Tags_with_Colors (
            int item_id
        )
        {
            try
            {
                var menu_item = _context.MenuItems.FirstOrDefault(mi => mi.item_id == item_id);
                if (menu_item is null)
                {
                    return NotFound("Menu Item was not found");
                }
                var tags = await _context.Tags.Where(t => t.MenuItemTags.Any(mt => mt.Menu_item_id == item_id)).OrderBy(t => t.tag_name).ToListAsync();
                var response = tags.Select(t => new FullTagResponseDTO
                {
                    Name = t.tag_name,
                    Tag_Id = t.tag_id,
                    Color_Code =t.tag_color
                }).ToList();
                
                return Ok(response);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Getting Menu Item Tags with Colors");
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }           
        
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost("{item_id}/tags/{tag_id}")]
        public async Task<IActionResult> Add_Tag_To_Menu (
            int item_id,
            int tag_id
        )
        {
            try
            {
                var menu_item = await _context.MenuItems.Include(mi => mi.MenuItemTags).FirstOrDefaultAsync(mi => mi.item_id == item_id); ;
                if (menu_item is null)
                {
                    return NotFound("Menu Item was not found");
                }
                var returnedTag = await _context.Tags.FirstOrDefaultAsync(t => t.tag_id == tag_id);
                if (returnedTag is null)
                {
                    return NotFound("Item Tag Was not found");
                }
                if(menu_item.MenuItemTags.Any(mt => mt.Tag_id == returnedTag.tag_id))
                {
                    return Conflict("Can not Assign tag to same Menu Item");
                }
                menu_item.MenuItemTags.Add(new MenuItemTag
                {
                    Menu_item_id = menu_item.item_id,
                    Tag_id = returnedTag.tag_id,
                    Tag = returnedTag
                });
                await _context.SaveChangesAsync();
                return Ok(menu_item);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Getting Menu Item Tags with Colors");
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }          
       
        [Authorize(Roles = "Admin,Staff")]
        [HttpDelete("{item_id}/tags/{tag_id}")]
        public async Task<IActionResult> Remove_Tag_To_Menu (
            int item_id,
            int tag_id
        )
        {
            try
            {
                var menu_item = await _context.MenuItems.Include(mi => mi.MenuItemTags).FirstOrDefaultAsync(mi => mi.item_id == item_id); ;
                if (menu_item is null)
                {
                    return NotFound("Menu Item was not found");
                }
                var returnedTag = await _context.Tags.FirstOrDefaultAsync(t => t.tag_id == tag_id);
                if (returnedTag is null)
                {
                    return NotFound("Item Tag Was not found");
                }
                if(!menu_item.MenuItemTags.Any(mt => mt.Tag_id == returnedTag.tag_id))
                {
                    return Conflict("Item does not exist on menu");
                }
                menu_item.MenuItemTags.Remove(new MenuItemTag
                {
                    Menu_item_id = menu_item.item_id,
                    Tag_id = returnedTag.tag_id,
                    Tag = returnedTag
                });
                await _context.SaveChangesAsync();
                return Ok(menu_item);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Getting Menu Item Tags with Colors");
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }                  

    
    }

}