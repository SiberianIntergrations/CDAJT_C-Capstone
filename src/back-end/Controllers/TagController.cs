using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using back_end.domain.DbContexts;
using back_end.domain.Entities;
using back_end.domain;
using back_end.DTO.TagDTOs;

namespace back_end.controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [Authorize]
  public class TagController : Controller
  {
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TagController> _logger;

    public TagController(ApplicationDbContext context, ILogger<TagController> logger)
    {
      _context = context;
      _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTag([FromBody] TagBaseDTO tagData)
    {
      try
      {
        // Check if user is Admin
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        if (userRole != UserRoles.Admin.ToString())
        {
          return StatusCode(403, new { detail = "Permission denied" });
        }

        // Check if tag already exists
        var existingTag = await _context.Tags
          .FirstOrDefaultAsync(t => t.tag_name == tagData.Tag_Name);

        if (existingTag != null)
        {
          return BadRequest(new { detail = "Tag already exists" });
        }

        var tag = new Tag
        {
          tag_name = tagData.Tag_Name,
          tag_color = tagData.Tag_Color
        };

        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTagById), new { tag_id = tag.tag_id }, new TagBaseDTO
        {
          Tag_Id = tag.tag_id,
          Tag_Name = tag.tag_name,
          Tag_Color = tag.tag_color
        });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error creating tag");
        return StatusCode(500, new { detail = "Error creating tag" });
      }
    }

    [HttpGet("colors")]
    public async Task<IActionResult> GetTagColors()
    {
      try
      {
        var tags = await _context.Tags.ToListAsync();

        var response = tags.Select(t => new TagBaseDTO
        {
          Tag_Id = t.tag_id,
          Tag_Name = t.tag_name,
          Tag_Color = t.tag_color
        }).ToList();

        return Ok(response);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error retrieving tag colors");
        return StatusCode(500, new { detail = "Error retrieving tag colors" });
      }
    }

    [HttpGet("{tag_id}")]
    public async Task<IActionResult> GetTagById(int tag_id)
    {
      try
      {
        var tag = await _context.Tags
          .FirstOrDefaultAsync(t => t.tag_id == tag_id);

        if (tag == null)
        {
          return NotFound(new { detail = "Tag not found" });
        }

        return Ok(new TagBaseDTO
        {
          Tag_Id = tag.tag_id,
          Tag_Name = tag.tag_name,
          Tag_Color = tag.tag_color
        });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error retrieving tag");
        return StatusCode(500, new { detail = "Error retrieving tag" });
      }
    }

    [HttpPut("{tag_id}")]
    public async Task<IActionResult> UpdateTag(int tag_id, [FromBody] TagUpdateDTO tagData)
    {
      try
      {
        // Check if user is Admin
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        if (userRole != UserRoles.Admin.ToString())
        {
          return StatusCode(403, new { detail = "Permission denied" });
        }

        var tag = await _context.Tags
          .FirstOrDefaultAsync(t => t.tag_id == tag_id);

        if (tag == null)
        {
          return NotFound(new { detail = "Tag not found" });
        }

        // Update only provided fields
        if (tagData.Tag_Name != null)
        {
          tag.tag_name = tagData.Tag_Name;
        }
        if (tagData.Tag_Color != null)
        {
          tag.tag_color = tagData.Tag_Color;
        }

        await _context.SaveChangesAsync();

        return Ok(new TagBaseDTO
        {
          Tag_Id = tag.tag_id,
          Tag_Name = tag.tag_name,
          Tag_Color = tag.tag_color
        });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error updating tag");
        return StatusCode(500, new { detail = "Error updating tag" });
      }
    }

    [HttpGet("{tag_id}/menu_items")]
    public async Task<IActionResult> GetMenuItemsByTag(int tag_id)
    {
      try
      {
        // Check if user is Admin
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        if (userRole != UserRoles.Admin.ToString())
        {
          return StatusCode(403, new { detail = "Permission denied" });
        }

        var tag = await _context.Tags
          .Include(t => t.MenuItemTags)
            .ThenInclude(mit => mit.MenuItem)
          .FirstOrDefaultAsync(t => t.tag_id == tag_id);

        if (tag == null)
        {
          return NotFound(new { detail = "Tag not found" });
        }

        var menuItems = tag.MenuItemTags.Select(mit => mit.MenuItem).ToList();

        return Ok(menuItems);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error retrieving menu items by tag");
        return StatusCode(500, new { detail = "Error retrieving menu items" });
      }
    }

    [HttpDelete("{tag_id}")]
    public async Task<IActionResult> DeleteTag(int tag_id)
    {
      try
      {
        // Check if user is Admin
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        if (userRole != UserRoles.Admin.ToString())
        {
          return StatusCode(403, new { detail = "Permission denied" });
        }

        var tag = await _context.Tags
          .Include(t => t.MenuItemTags)
          .FirstOrDefaultAsync(t => t.tag_id == tag_id);

        if (tag == null)
        {
          return NotFound(new { detail = "Tag not found" });
        }

        // Clear menu item tags association
        tag.MenuItemTags.Clear();
        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();

        return Ok(new { detail = "Tag deleted" });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error deleting tag");
        return StatusCode(500, new { detail = "Error deleting tag" });
      }
    }
  }
}