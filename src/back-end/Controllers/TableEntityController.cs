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
using Swashbuckle.AspNetCore.Annotations;

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

    /// <summary>
    /// Creates a new table.
    /// </summary>
    /// <param name="table_data">The table payload including table number, seat count, active flag, and optional QR code URL.</param>
    /// <remarks>
    /// Returns <c>201 Created</c> with the created table and a <c>Location</c> header pointing to <see cref="GetTable(int)"/>.
    /// Table numbers must be unique.
    /// </remarks>
    /// <response code="201">The table was created successfully.</response>
    /// <response code="400">The request body is invalid or missing required fields.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user is not authorized to create tables.</response>
    /// <response code="409">A table with the specified number already exists.</response>
    /// <response code="500">An unexpected error occurred while creating the table.</response>
    [Authorize(Roles = "Admin,Staff")]
    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    [SwaggerOperation(
        OperationId = "CreateTable",
        Summary = "Create a table",
        Description = "Creates a new table. Requires Admin or Staff role."
    )]
   
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
                _logger.LogError(ex, "Error creating table {@TableData}", table_data);
                return StatusCode(500, new { detail = "Error creating table"});
            }
        }

        /// <summary>
        /// Retrieves all tables from the system.
        /// </summary>
        /// <remarks>
        /// Returns a list of all tables, regardless of whether they are active or in use.  
        /// Requires authentication.
        /// </remarks>
        /// <response code="200">A list of all tables was returned successfully.</response>
        /// <response code="500">An internal server error occurred while retrieving tables.</response>
        [Authorize]
        [HttpGet]
        [Produces("application/json")]
        [SwaggerOperation(
            OperationId = "GetAllTables",
            Summary = "Retrieve all tables",
            Description = "Returns a list of all tables in the database. Includes both active and inactive tables."
        )]
        [ProducesResponseType(typeof(IEnumerable<TableEntity>), StatusCodes.Status200OK)]
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

    /// <summary>
    /// Lists all active tables that are not currently in an ongoing dining session.
    /// </summary>
    /// <remarks>
    /// A table is considered <em>empty</em> if it is marked as active and it is not part of any dining session
    /// where <c>Ended_At == null</c>.
    /// </remarks>
    /// <response code="200">A list of empty (available) tables was returned successfully.</response>
    /// <response code="500">An unexpected error occurred while retrieving the tables.</response>
    [Authorize]
    [HttpGet("empty")]
    [Produces("application/json")]
    [SwaggerOperation(
        OperationId = "ListEmptyTables",
        Summary = "List empty (available) tables",
        Description = "Returns all active tables that are not currently part of an active dining session."
    )]
    [ProducesResponseType(typeof(IEnumerable<TableEntity>), StatusCodes.Status200OK)]

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
                _logger.LogError(ex, "Error retrieving empty tables");
                return StatusCode(500,"Error retrieving empty tables");
            }
        }

        /// <summary>
        /// Retrieves a table by its unique identifier.
        /// </summary>
        /// <param name="table_id">The unique identifier of the table to retrieve.</param>
        /// <remarks>
        /// Returns the table’s complete record, including its number, seat count, active state, and QR code information.
        /// Requires authorization.
        /// </remarks>
        /// <response code="200">The table was found and returned successfully.</response>
        /// <response code="404">No table exists with the specified <paramref name="table_id"/>.</response>
        /// <response code="500">An unexpected error occurred while retrieving the table.</response>
        [Authorize]
        [HttpGet("{table_id:int}")]
        [Produces("application/json")]
        [SwaggerOperation(
            OperationId = "GetTable",
            Summary = "Retrieve a table by ID",
            Description = "Fetches details of a specific table by its unique identifier. Requires authentication."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
                _logger.LogError(ex, "Error retrieving table {TableId}", table_id);
                return StatusCode(500, "Error retrieving table");
            }
        }

        /// <summary>
        /// Updates a table by its identifier.
        /// </summary>
        /// <param name="table_id">The unique identifier of the table to update.</param>
        /// <param name="table_data">Fields to update for the table (partial updates supported).</param>
        /// <remarks>
        /// This endpoint performs a <em>partial</em> update using <c>PUT</c> for convenience:
        /// only the non-null properties in <c>TableEntityUpdateDTO</c> are applied.
        /// <br/><br/>
        /// - If <c>Table_Number</c> is provided and differs from the current value, it must be unique.
        /// - Returns the updated table in the response body.
        /// </remarks>
        /// <response code="200">The table was updated successfully.</response>
        /// <response code="400">Invalid request body or no updatable fields provided.</response>
        /// <response code="404">No table exists with the specified <paramref name="table_id"/>.</response>
        /// <response code="409">A different table already uses the requested table number.</response>
        /// <response code="500">An unexpected error occurred while updating the table.</response>

        [Authorize]
        [HttpPut("{table_id:int}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerOperation(
            OperationId = "UpdateTable",
            Summary = "Update a table",
            Description = "Partially updates a table (number, seat count, active flag, QR code URL)."
        )]
        [ProducesResponseType(typeof(TableEntity), StatusCodes.Status200OK)]

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
                        return BadRequest("No updatable fields were provided");
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
                _logger.LogError(ex, "Error updating table {TableId}", table_id);
                return StatusCode(500, "Error updating table");
            }
        }

    /// <summary>
    /// Deletes a table by its identifier.
    /// </summary>
    /// <param name="table_id">The unique identifier of the table to delete.</param>
    /// <remarks>
    /// Returns <c>204 No Content</c> on successful deletion.  
    /// If the table is associated with an active dining session (<c>Ended_At == null</c>), deletion is blocked and a <c>409 Conflict</c> is returned.
    /// </remarks>
    /// <response code="204">The table was deleted successfully.</response>
    /// <response code="404">No table exists with the specified <paramref name="table_id"/>.</response>
    /// <response code="409">The table cannot be deleted because it is currently in use.</response>
    /// <response code="500">An unexpected error occurred while deleting the table.</response>
    [Authorize]
    [HttpDelete("{table_id:int}")]
    [Produces("application/json")]
    [SwaggerOperation(
        OperationId = "DeleteTable",
        Summary = "Delete a table",
        Description = "Deletes a table by ID. Deletion is blocked if the table is part of an active dining session."
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<IActionResult> DeleteTable(
            int table_id
        )
        {
            try
            {
                var table = await _context.Tables.FirstOrDefaultAsync(t => t.Table_Id == table_id);
                if (table is null)
                {
                    return NotFound("Table not found");
                }
                var activeSession = await _context.DiningSessions.Where(ds => ds.Tables.Any(t => t.Table_Id == table_id) && ds.Ended_At == null).FirstOrDefaultAsync();
                if (activeSession != null)
                {
                    return BadRequest("Cannot delete a table that is currently in use");
                }

                _context.Tables.Remove(table);
                await _context.SaveChangesAsync();
                return Ok("Table was Deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting table {TableId}", table_id);
                return StatusCode(500, "Error deleting table");
            }
        }

    /// <summary>
    /// Toggles a table's active status.
    /// </summary>
    /// <param name="table_id">The unique identifier of the table.</param>
    /// <remarks>
    /// - If the table is <c>inactive</c>, this endpoint **activates** it.  
    /// - If the table is <c>active</c>, this endpoint attempts to **deactivate** it.  
    /// - Deactivation is **blocked** when the table is part of an active dining session (<c>Ended_At == null</c>).  
    /// Returns the new status in the response body.
    /// </remarks>
    /// <response code="200">The table status was toggled; response includes the current status.</response>
    /// <response code="404">No table exists with the specified <paramref name="table_id"/>.</response>
    /// <response code="409">The table cannot be deactivated because it is currently in use.</response>
    /// <response code="500">An unexpected error occurred while toggling the table status.</response>
    [Authorize(Roles = "Admin,Staff")]
    [HttpPost("{table_id:int}/toggle-status")]
    [Produces("application/json")]
    [SwaggerOperation(
        OperationId = "ToggleTableStatus",
        Summary = "Toggle a table's active status",
        Description = "Activates an inactive table, or deactivates an active table if it has no active session."
    )]
    [ProducesResponseType(typeof(TableEntity), StatusCodes.Status200OK)]

        public async Task<IActionResult> ToggleTableStatus(
            int table_id
        )
        {
            try
            {
                var table = await _context.Tables.FirstOrDefaultAsync(t => t.Table_Id == table_id);
                if (table is null)
                {
                    return NotFound("Table not found");
                }
                var activeSession = await _context.DiningSessions.Where(ds => ds.Tables.Any(t => t.Table_Id == table_id) && ds.Ended_At == null).FirstOrDefaultAsync();
                if (activeSession != null)
                {
                    return BadRequest("Cannot deactivate table that is currently in use");
                }
                if (table.is_active)
                {
                    table.is_active = false;
                }
                if (!table.is_active)
                {
                    table.is_active = true;
                }

                _context.Tables.Update(table);
                await _context.SaveChangesAsync();
                return Ok(table);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling table status for table {TableId}", table_id);
                return StatusCode(500, "Internal Server Error");
            }
        }
                  
    /// <summary>
    /// Checks whether a table currently has an active dining session.
    /// </summary>
    /// <param name="table_id">The unique identifier of the table.</param>
    /// <remarks>
    /// Returns <c>success=true</c> if the table has **no** active session (available), otherwise <c>success=false</c>.
    /// An active session is defined as a <c>DiningSession</c> where <c>Ended_At == null</c> and the session includes the given table.
    /// </remarks>
    /// <response code="200">Request succeeded; response indicates availability via the <c>success</c> flag.</response>
    /// <response code="403">The user is not authorized to perform this action.</response>
    /// <response code="500">An unexpected error occurred while checking availability.</response>
    [Authorize]
    [HttpPost("{table_id:int}/active-session")]
    [Produces("application/json")]
    [SwaggerOperation(
        OperationId = "CheckTableActiveSession",
        Summary = "Check if a table has an active session",
        Description = "Returns success=true when the table is available (no active session), success=false otherwise."
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<IActionResult> CheckTableActiveSession(
            int table_id
        )
        {
            try
            {
                var activeSession = await _context.DiningSessions.Where(ds => ds.Tables.Any(t => t.Table_Id == table_id) && ds.Ended_At == null).FirstOrDefaultAsync();
                if (activeSession != null)
                {
                    return BadRequest(new { success = false});
                }
                else
                {
                    return Ok(new { success = true });
                }
            
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking active session for table {TableId}", table_id);
                return StatusCode(500, "Internal Server Error");
            }
        }       
    }
}