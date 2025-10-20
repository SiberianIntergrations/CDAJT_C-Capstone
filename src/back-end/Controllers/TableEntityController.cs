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
using System.Security.Claims;
using back_end.DTO.SessionParticipantDTOs;
using back_end.DTO.TableEntityDTOs;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace back_end.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class TableEntityController : ControllerBase
  {
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TableEntityController> _logger;

    public TableEntityController(ApplicationDbContext context, ILogger<TableEntityController> logger)
    {
      _context = context;
      _logger = logger;
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpPost()]
    public async Task<IActionResult> CreateTable(TableEntityCreateDTO table_data)
    {
      try
      {
        var existingTable = await _context.Tables
            .FirstOrDefaultAsync(t => t.table_number == table_data.Table_Number);

        if (existingTable != null)
        {
          return BadRequest("Table Number already exists");
        }

        var newTable = new TableEntity
        {
          table_number = table_data.Table_Number,
          QR_Code = table_data.Qr_Code_Url,
          seat_count = table_data.Seat_Count,
          is_active = table_data.Is_Active,
        };

        _context.Tables.Add(newTable);
        await _context.SaveChangesAsync();
        return Ok(newTable);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error creating table");
        return StatusCode(500, new { detail = "Error creating table" });
      }
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TableEntity>>> GetAllTables()
    {
      try
      {
        var tables = await _context.Tables
            .Include(t => t.TableGroup)
            .ToListAsync();
        return Ok(tables);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error getting tables");
        return StatusCode(500, "Internal Server Error");
      }
    }

    [Authorize]
    [HttpGet("empty")]
    public async Task<ActionResult<IEnumerable<TableEntity>>> ListEmptyTables()
    {
      try
      {
        // Find tables that are:
        // 1. Active
        // 2. Not assigned to any active dining session (either directly or through a table group)
        var emptyTables = await _context.Tables
            .Include(t => t.TableGroup)
            .Where(t => t.is_active == true &&
                        !t.DiningSessions.Any(ds => ds.Ended_At == null) &&
                        (t.TableGroup == null || !t.TableGroup.DiningSessions.Any(ds => ds.Ended_At == null)))
            .ToListAsync();

        return Ok(emptyTables);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error getting empty tables");
        return StatusCode(500, "Internal Server Error");
      }
    }

    [Authorize]
    [HttpGet("{table_id}")]
    public async Task<IActionResult> GetTable(int table_id)
    {
      try
      {
        var table = await _context.Tables
            .Include(t => t.TableGroup)
            .FirstOrDefaultAsync(t => t.Table_Id == table_id);

        if (table is null)
        {
          return NotFound("Table not found");
        }

        return Ok(table);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error getting table");
        return StatusCode(500, "Internal Server Error");
      }
    }

    [Authorize]
    [HttpPut("{table_id}")]
    public async Task<IActionResult> UpdateTable(int table_id, TableEntityUpdateDTO table_data)
    {
      try
      {
        var table = await _context.Tables
            .FirstOrDefaultAsync(t => t.Table_Id == table_id);

        if (table is null)
        {
          return NotFound("Table not found");
        }

        // Check if table number is being changed
        if (table_data.Table_Number.HasValue && table.table_number != table_data.Table_Number)
        {
          var existingTable = await _context.Tables
              .FirstOrDefaultAsync(t => t.table_number == table_data.Table_Number && t.Table_Id != table_id);

          if (existingTable != null)
          {
            return BadRequest("Table number already exists");
          }
          table.table_number = table_data.Table_Number.Value;
        }

        if (table_data.Seat_Count.HasValue)
        {
          table.seat_count = table_data.Seat_Count.Value;
        }

        if (table_data.Is_Active.HasValue)
        {
          table.is_active = table_data.Is_Active.Value;
        }

        if (table_data.Qr_Code_Url != null)
        {
          table.QR_Code = table_data.Qr_Code_Url;
        }

        _context.Tables.Update(table);
        await _context.SaveChangesAsync();
        return Ok(table);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error updating table");
        return StatusCode(500, "Internal Server Error");
      }
    }

    [Authorize]
    [HttpDelete("{table_id}")]
    public async Task<IActionResult> DeleteTable(int table_id)
    {
      try
      {
        var table = await _context.Tables
            .Include(t => t.TableGroup)
            .FirstOrDefaultAsync(t => t.Table_Id == table_id);

        if (table is null)
        {
          return NotFound("Could not find a table to delete");
        }

        // Check if table is in an active session (directly assigned)
        var activeSessionDirect = await _context.DiningSessions
            .AnyAsync(ds => ds.Table_Id == table_id && ds.Ended_At == null);

        if (activeSessionDirect)
        {
          return BadRequest("Cannot delete a table that is currently in use in an active session");
        }

        // Check if table is part of a table group that's in an active session
        if (table.TableGroup_Id.HasValue)
        {
          var activeSessionViaGroup = await _context.DiningSessions
              .AnyAsync(ds => ds.TableGroup_Id == table.TableGroup_Id && ds.Ended_At == null);

          if (activeSessionViaGroup)
          {
            return BadRequest("Cannot delete a table that is part of a table group in an active session");
          }
        }

        _context.Tables.Remove(table);
        await _context.SaveChangesAsync();
        return Ok("Table was deleted");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error deleting table");
        return StatusCode(500, "Internal Server Error");
      }
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpPost("{table_id}/toggle-status")]
    public async Task<IActionResult> ToggleTableStatus(int table_id)
    {
      try
      {
        var table = await _context.Tables
            .Include(t => t.TableGroup)
            .FirstOrDefaultAsync(t => t.Table_Id == table_id);

        if (table is null)
        {
          return NotFound("Could not find table");
        }

        // Check if table is in an active session (directly assigned)
        var activeSessionDirect = await _context.DiningSessions
            .AnyAsync(ds => ds.Table_Id == table_id && ds.Ended_At == null);

        if (activeSessionDirect)
        {
          return BadRequest("Cannot deactivate table that is currently in use in an active session");
        }

        // Check if table is part of a table group that's in an active session
        if (table.TableGroup_Id.HasValue)
        {
          var activeSessionViaGroup = await _context.DiningSessions
              .AnyAsync(ds => ds.TableGroup_Id == table.TableGroup_Id && ds.Ended_At == null);

          if (activeSessionViaGroup)
          {
            return BadRequest("Cannot deactivate table that is part of a table group in an active session");
          }
        }

        // Toggle the status
        table.is_active = !table.is_active;

        _context.Tables.Update(table);
        await _context.SaveChangesAsync();
        return Ok(table);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error toggling table status");
        return StatusCode(500, "Internal Server Error");
      }
    }

    [Authorize]
    [HttpPost("{table_id}/active-session")]
    public async Task<IActionResult> CheckTableActiveSession(int table_id)
    {
      try
      {
        var table = await _context.Tables
            .Include(t => t.TableGroup)
            .FirstOrDefaultAsync(t => t.Table_Id == table_id);

        if (table == null)
        {
          return NotFound("Table not found");
        }

        // Check if table is directly assigned to an active session
        var activeSessionDirect = await _context.DiningSessions
            .AnyAsync(ds => ds.Table_Id == table_id && ds.Ended_At == null);

        if (activeSessionDirect)
        {
          return Ok(new { success = false, message = "Table is in an active session" });
        }

        // Check if table is part of a table group in an active session
        if (table.TableGroup_Id.HasValue)
        {
          var activeSessionViaGroup = await _context.DiningSessions
              .AnyAsync(ds => ds.TableGroup_Id == table.TableGroup_Id && ds.Ended_At == null);

          if (activeSessionViaGroup)
          {
            return Ok(new { success = false, message = "Table is part of a table group in an active session" });
          }
        }

        return Ok(new { success = true, message = "Table is available" });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error checking table active session");
        return StatusCode(500, "Internal Server Error");
      }
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpPost("{table_id}/assign-to-group/{table_group_id}")]
    public async Task<IActionResult> AssignTableToGroup(int table_id, int table_group_id)
    {
      try
      {
        var table = await _context.Tables
            .FirstOrDefaultAsync(t => t.Table_Id == table_id);

        if (table == null)
        {
          return NotFound("Table not found");
        }

        var tableGroup = await _context.TableGroups
            .FirstOrDefaultAsync(tg => tg.TableGroup_Id == table_group_id);

        if (tableGroup == null)
        {
          return NotFound("Table group not found");
        }

        // Check if table is in an active session
        var activeSession = await _context.DiningSessions
            .AnyAsync(ds => ds.Table_Id == table_id && ds.Ended_At == null);

        if (activeSession)
        {
          return BadRequest("Cannot assign table to group while it's in an active session");
        }

        table.TableGroup_Id = table_group_id;
        _context.Tables.Update(table);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Table assigned to group successfully", table });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error assigning table to group");
        return StatusCode(500, "Internal Server Error");
      }
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpDelete("{table_id}/remove-from-group")]
    public async Task<IActionResult> RemoveTableFromGroup(int table_id)
    {
      try
      {
        var table = await _context.Tables
            .Include(t => t.TableGroup)
            .FirstOrDefaultAsync(t => t.Table_Id == table_id);

        if (table == null)
        {
          return NotFound("Table not found");
        }

        if (!table.TableGroup_Id.HasValue)
        {
          return BadRequest("Table is not assigned to any group");
        }

        // Check if the table group is in an active session
        var activeSession = await _context.DiningSessions
            .AnyAsync(ds => ds.TableGroup_Id == table.TableGroup_Id && ds.Ended_At == null);

        if (activeSession)
        {
          return BadRequest("Cannot remove table from group while the group is in an active session");
        }

        table.TableGroup_Id = null;
        _context.Tables.Update(table);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Table removed from group successfully", table });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error removing table from group");
        return StatusCode(500, "Internal Server Error");
      }
    }
  }
}