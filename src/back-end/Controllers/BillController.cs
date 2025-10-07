using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.domain.Entities;

namespace back_end.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class BillController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<BillController> _logger;

        public BillController(ApplicationContext context, ILogger<BillController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //GET api/bill
        //Get all bills
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Billing>>> GetAllBills()
        {
            try
            {
                var bill = await _context.Billings.ToListAsync();
                return Ok(bill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Getting Bills");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}