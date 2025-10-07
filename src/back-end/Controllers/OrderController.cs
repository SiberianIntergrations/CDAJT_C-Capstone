using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.domain.Entities;

namespace back_end.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class OrderController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<OrderController> _logger;

        public OrderController(ApplicationContext context, ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //GETL api/order
        //Get all Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SessionOrder>>> GetAllOrder()
        {
            try
            {
                var order = await _context.SessionOrders.ToListAsync();

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Orders");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}