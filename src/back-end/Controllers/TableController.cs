using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.domain.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace back_end.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class TableController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TableController> _logger;

        public TableController(ApplicationDbContext context, ILogger<TableController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all tables from the system.
        /// </summary>
        /// <remarks>
        /// Returns a list of all tables, including active and inactive ones.  
        /// This endpoint does not require authentication (can be secured if needed).
        /// </remarks>
        /// <response code="200">A list of all tables was returned successfully.</response>
        /// <response code="500">An unexpected error occurred while retrieving tables.</response>
        [HttpGet]
        [Produces("application/json")]
        [SwaggerOperation(
            OperationId = "GetAllTables",
            Summary = "Retrieve all tables",
            Description = "Fetches all tables from the database, including both active and inactive ones."
        )]
        [ProducesResponseType(typeof(IEnumerable<TableEntity>), StatusCodes.Status200OK)]

        [SwaggerResponse(StatusCodes.Status200OK, "All tables retrieved successfully", typeof(IEnumerable<TableEntity>))]
            public async Task<ActionResult<IEnumerable<TableEntity>>> GetAllTables()
        {
            try
            {
                var table = await _context.Tables.OrderBy(t => t.Location_Id).ToListAsync();
                return Ok(table);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Getting Tables");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}