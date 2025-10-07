using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.domain.Entities;

namespace back_end.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class LocationController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<LocationController> _logger;

        public LocationController(ApplicationContext context, ILogger<LocationController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //GET api/location
        //Get all locations
        [HttpGet]

        public async Task<ActionResult<IEnumerable<Locations>>> GetAllLocations()
        {
            try
            {
                var location = await _context.Locations.ToListAsync();
                return Ok(location);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get all locations");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}