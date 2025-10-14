using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.domain.Entities;

namespace back_end.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceRequestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ServiceRequestController> _logger;

        public ServiceRequestController(ApplicationDbContext context, ILogger<ServiceRequestController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //GET api/servicerequest
        //Get all current service requests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceRequest>>> GetAllServiceRequest()
        {
            try
            {
                var request = await _context.ServiceRequests.ToListAsync();
                return Ok(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Service requests");
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}