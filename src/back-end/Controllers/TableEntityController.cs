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
    public async Task<IActionResult> CreateTable(
        TableEntityCreateDTO table_data
    )
    {
      try
      {
        var existingTable = await _context.Tables.FirstOrDefaultAsync(t => t.table_number == table_data.Table_Number);
        if (existingTable != null)
        {
          return BadRequest("Table Number already Exist");
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
        _logger.LogError(ex, "Error retrieving menu items by tag");
        return StatusCode(500, new { detail = "Error retrieving menu items" });
      }
    }

    //GET api/table
    //Get all tables
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TableEntity>>> GetAllTables()
    {
      try
      {
        var table = await _context.Tables.ToListAsync();
        return Ok(table);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error Getting Tables");
        return StatusCode(500, "Internal Server Error");
      }
    }

    //GET api/table
    //Get all tables
    [Authorize]
    [HttpGet("empty")]
    public async Task<ActionResult<IEnumerable<TableEntity>>> ListEmptyTables(

    )
    {
      try
      {
        var emptyTables = await _context.Tables
            .Where(t => t.is_active == true && !t.Sessions.Any(s => s.DiningSession.Ended_At == null)).ToListAsync();
        return Ok(emptyTables);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error Getting Tables");
        return StatusCode(500, "Internal Server Error");
      }
    }

    [Authorize]
    [HttpGet("{table_id}")]
    public async Task<IActionResult> GetTable(
        int table_id
    )
    {
      try
      {
        var table = _context.Tables.FirstOrDefaultAsync(t => t.Table_Id == table_id);
        if (table is null)
        {
          return NotFound("Table not Found");
        }
        return Ok(table);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error Getting Tables");
        return StatusCode(500, "Internal Server Error");
      }
    }

    [Authorize]
    [HttpPut("{table_id}")]
    public async Task<IActionResult> GetTable(
        int table_id,
        TableEntityUpdateDTO table_data
    )
    {
      try
      {
        var table = await _context.Tables.FirstOrDefaultAsync(t => t.Table_Id == table_id);
        if (table is null)
        {
          return NotFound("Table not Found");
        }
        if (table.table_number != table_data.Table_Number)
        {
          var existingTable = await _context.Tables.FirstOrDefaultAsync(t => t.table_number == table_data.Table_Number && t.Table_Id != table_id);
          if (existingTable != null)
          {
            return BadRequest("Table number already Exists");
          }
        }
        if (table_data.Table_Number.HasValue)
        {
          table.table_number = (int)table_data.Table_Number;


        }
        if (table_data.Seat_Count.HasValue)
        {
          table.seat_count = (int)table_data.Seat_Count;
        }
        if (table_data.Is_Active.HasValue)
        {
          table.is_active = (bool)table_data.Is_Active;
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
        _logger.LogError(ex, "Error Getting Tables");
        return StatusCode(500, "Internal Server Error");
      }
    }

    [Authorize]
    [HttpDelete("{table_id}")]
    public async Task<IActionResult> DeleteTable(
        int table_id
    )
    {
      try
      {
        var table = await _context.Tables.FirstOrDefaultAsync(t => t.Table_Id == table_id);
        if (table is null)
        {
          return NotFound("Could not find a table to delete");
        }
        var activeSession = await _context.DiningSessions.Where(ds => ds.Tables.Any(t => t.Table_Id == table_id) && ds.Ended_At == null).FirstOrDefaultAsync();
        if (activeSession != null)
        {
          return BadRequest("Can not delete a table that is currently in use");
        }

        _context.Tables.Remove(table);
        await _context.SaveChangesAsync();
        return Ok("Table was Deleted");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error Getting Tables");
        return StatusCode(500, "Internal Server Error");
      }
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpPost("{table_id}/toggle-status")]
    public async Task<IActionResult> ToggleTableStatus(
        int table_id
    )
    {
      try
      {
        var table = await _context.Tables.FirstOrDefaultAsync(t => t.Table_Id == table_id);
        if (table is null)
        {
          return NotFound("Could not find a table to delete");
        }
        var activeSession = await _context.DiningSessions.Where(ds => ds.Tables.Any(t => t.Table_Id == table_id) && ds.Ended_At == null).FirstOrDefaultAsync();
        if (activeSession != null)
        {
          return BadRequest("Cannot deactivate table that is currently in use");
        }
        table.is_active = false;

        _context.Tables.Update(table);
        await _context.SaveChangesAsync();
        return Ok(table);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error Getting Tables");
        return StatusCode(500, "Internal Server Error");
      }
    }

    [Authorize]
    [HttpPost("{table_id}/active-session")]
    public async Task<IActionResult> CheckTableActiveSession(
        int table_id
    )
    {
      try
      {
        var activeSession = await _context.DiningSessions.Where(ds => ds.Tables.Any(t => t.Table_Id == table_id) && ds.Ended_At == null).FirstOrDefaultAsync();
        if (activeSession != null)
        {
          return BadRequest(new { success = false });
        }
        else
        {
          return Ok(new { success = true });
        }

      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error Getting Tables");
        return StatusCode(500, "Internal Server Error");
      }
    }
  }
}