using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.domain.Entities;
using back_end.domain;

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

        //GET: api/order/{id}
        //Get an Order with all the items
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetOrderById(int id)
        {
            try
            {
                //Get the order from the inputted Id
                //Get the user, session, bill and ordered items
                var order = await _context.SessionOrders
                            .Where(o => o.Order_Id == id)
                            .Include(o => o.User)
                            .Include(o => o.DiningSession)
                            .Include(o => o.Bill)
                            .Include(o => o.OrderItems)
                                .ThenInclude(od => od.MenuItem)
                            .FirstOrDefaultAsync();

                //Check if the order is found
                if (order == null)
                {
                    return NotFound(new { message = $"Can't find Order with ID: {id}" });
                }

                //Create object to store information about the order
                //Add the customers information
                //List all the items and calculate the cost of each item
                //Calculate the cost of every item together
                var orderInfo = new
                {
                    orderId = order.Order_Id,
                    sessionId = order.session_id,
                    billId = order.Bill_Id,
                    status = order.Status,
                    createdAt = order.Created_At,
                    completedAt = order.Completed_At,

                    customer = new
                    {
                        name = $"{order.User.First_name} {order.User.Last_name}",
                        email = order.User.Email
                    },

                    items = order.OrderItems.Select(o => new
                    {
                        itemId = o.Item_Id,
                        itemName = o.MenuItem.Name,
                        quantity = o.Quantity,
                        priceAtTime = o.Price_At_Time,
                        itemStatus = o.Order_Item_Status,
                        itemTotal = o.Quantity * o.Price_At_Time
                    }).ToList(),

                    orderTotal = order.OrderItems.Select(i => i.Quantity * i.Price_At_Time).Sum()
                };

                return Ok(orderInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Can't find order with ID: {id}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        //GET: api/order/session/{id}
        //Get all orders tied to a Dining Session
        [HttpGet("session/{sessionId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetOrderBySession(int sessionId)
        {
            try
            {
                var session = await _context.DiningSessions.FindAsync(sessionId);

                if (session == null)
                {
                    return NotFound(new { message = $"Can't find session with the ID: {sessionId}" });
                }

                var sessionOrder = await _context.SessionOrders
                                        .Where(o => o.session_id == sessionId)
                                        .Include(o => o.User)
                                        .Include(o => o.OrderItems)
                                            .ThenInclude(or => or.MenuItem)
                                        .OrderBy(o => o.Created_At)
                                        .ToListAsync();

                var orderAll = sessionOrder.Select(o => new
                {
                    orderId = o.Order_Id,
                    customerId = o.User_Id,
                    customerName = $"{o.User.First_name} {o.User.Last_name}",
                    status = o.Status,
                    itemTotal = o.OrderItems.Count,
                    orderTotal = o.OrderItems.Sum(or => or.Quantity * or.Price_At_Time),
                    createdAt = o.Created_At,
                    completedAt = o.Completed_At
                }).ToList();

                return Ok(orderAll);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Can't find Session with ID: {sessionId}.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        //GET: api/order/user{id}
        //Get all orders placed by a specific customer
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetOrderByUser(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);

                //Check if user exists
                if (user == null)
                {
                    return NotFound(new { message = $"Can't find user with ID: {userId}" });
                }

                //Get all orders from this user and sort by the newest
                //Include session menu and item details
                var order = await _context.SessionOrders
                            .Where(o => o.User_Id == userId)
                            .Include(o => o.DiningSession)
                                .ThenInclude(or => or.Menu)
                            .Include(o => o.OrderItems)
                                .ThenInclude(od => od.MenuItem)
                            .OrderByDescending(o => o.Created_At)
                            .ToListAsync();

                //Include Order totals and items for each OrderID
                //Should we include Name here?
                var orderHistory = order.Select(o => new
                {
                    orderId = o.Order_Id,
                    sessionId = o.session_id,
                    menuName = o.DiningSession.Menu.Name,
                    status = o.Status,
                    itemCount = o.OrderItems.Count,
                    orderTotal = o.OrderItems.Sum(or => or.Quantity * or.Price_At_Time),
                    createdAt = o.Created_At,
                    completedAt = o.Completed_At,

                    items = o.OrderItems.Select(or => new
                    {
                        name = or.MenuItem.Name,
                        quantity = or.Quantity
                    }).ToList()
                }).ToList();

                return Ok(orderHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Can't find Order for User ID: {userId}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}