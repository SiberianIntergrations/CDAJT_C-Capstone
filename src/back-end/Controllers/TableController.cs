using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.domain.Entities;

namespace back_end.Controllers
{
    [ApiController]
    [Route("api/controller")]

    public class TableController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TableController> _logger;

        public TableController(ApplicationDbContext context, ILogger<TableController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //GET api/table
        //Get all tables
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
    }
}