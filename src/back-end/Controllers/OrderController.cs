using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.domain.Entities;
using back_end.domain.enums;
using back_end.DTO.OrdersDTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace back_end.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderController> _logger;

        public OrderController(ApplicationDbContext context, ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<SessionOrder>>> CreateOrder(
            OrderCreateDTO order_data
        )
        {
            try
            {
                var session = await _context.DiningSessions.FirstOrDefaultAsync(ds => ds.Session_Id == order_data.Session_Id);
                if (session is null)
                {
                    return NotFound("Dinning Session was not found in the current context");
                }
                var bill = await _context.Bills.FirstOrDefaultAsync(b => b.Bill_Id == order_data.Bill_Id && b.Status == BillStatus.Open);
                if (bill is null)
                {
                    return NotFound("Bill Was not found for the current session order Create Bill");
                }
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var newOrder = new SessionOrder
                {
                    session_id = order_data.Session_Id,
                    Bill_Id = order_data.Bill_Id,
                    User_Id = userId,
                    Status = OrderStatus.Pending,
                    Created_At = DateTime.UtcNow
                };
                _context.SessionOrders.Add(newOrder);
                await _context.SaveChangesAsync();

                return Ok(new OrderResponseDTO
                {
                    Order_Id = newOrder.Order_Id,
                    Session_Id = newOrder.session_id,
                    Bill_Id = newOrder.Bill_Id,
                    User_Id = newOrder.User_Id,
                    Status = newOrder.Status,
                    Created_At = newOrder.Created_At
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Orders");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [Authorize]
        [HttpPut("{order_id}")]
        public async Task<IActionResult> UpdateOrder(
            int order_id,
            OrderUpdateDTO order_data
        )
        {
            try
            {
                var order = await _context.SessionOrders.FirstOrDefaultAsync(so => so.Order_Id == order_id);
                if (order is null)
                {
                    return NotFound("Order Data was not found");
                }
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var foundUser = await _context.Users.FirstOrDefaultAsync(u => u.User_id == userId);
                if (order.Status != OrderStatus.Pending)
                {
                    if ((foundUser.Role != UserRoles.Staff || foundUser.Role != UserRoles.Admin) || userId != order.User_Id)
                    {
                        return Conflict("Can not Modify a Approved Order");
                    }
                }
                var response = new OrderResponseDTO
                {
                    Order_Id = order.Order_Id,
                    Session_Id = order.session_id,
                    Bill_Id = order.Bill_Id,
                    User_Id = order.User_Id,
                    Status = order_data.Status,
                    Created_At = order.Created_At,
                };
                return Ok(response);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Orders");
                return StatusCode(500, "Internal Server Error");
            }
        }
        
        [Authorize]
        [HttpDelete("{order_id}/items/{item_id}")]
        public async Task<IActionResult> DeleteOrder (
            int order_id,   
            int item_id
        )
        {
            try
            {
                var order = await _context.SessionOrders.FirstOrDefaultAsync(so => so.Order_Id == order_id);
                if (order is null)
                {
                    return NotFound("Order Data was not found");
                }
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var foundUser = await _context.Users.FirstOrDefaultAsync(u => u.User_id == userId);
                if (order.Status != OrderStatus.Pending)
                {
                    if ((foundUser.Role != UserRoles.Staff || foundUser.Role != UserRoles.Admin) || userId != order.User_Id)
                    {
                        return Conflict("Can not Modify a Approved Order");
                    }
                }
                var item = await _context.OrderItems.FirstOrDefaultAsync(oi => oi.Order_Item_Id == order_id && oi.Item_Id == item_id);
                if (item is null)
                {
                    return BadRequest("Order Item was not found");
                }
                _context.OrderItems.Remove(item);
                await _context.SaveChangesAsync();
                return Ok("Item was Deleted");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Orders");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost("{order_id}/status")]
        public async Task<IActionResult> UpdateOrderStatus (
            int order_id,   
            OrderStatus new_Status
        )
        {
            try
            {
                var order = await _context.SessionOrders.FirstOrDefaultAsync(so => so.Order_Id == order_id);
                if (order is null)
                {
                    return NotFound("Order Data was not found");
                }
                order.Status = new_Status;
                if (order.Status == OrderStatus.Delivered)
                {
                    order.Completed_At = DateTime.UtcNow;
                } 
                _context.SessionOrders.Update(order);
                await _context.SaveChangesAsync();
                return Ok("Item was Updated");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Orders");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPost("items/{item_id}/complete")]
        public async Task<IActionResult> CompleteOrderItem (
            int item_id  
        )
        {
            try
            {
                var item = await _context.OrderItems.FirstOrDefaultAsync(or => or.Order_Item_Id == item_id);
                if (item is null)
                {
                    return NotFound("Item in a Order was not found");
                }
                item.Order_Item_Status = OrderStatus.Delivered;
                item.Completed_At = DateTime.UtcNow;
                _context.OrderItems.Update(item);
                await _context.SaveChangesAsync();
                var response = new OrderItemResponseDTO
                {
                    Order_Item_Id = item.Order_Item_Id,
                    Order_Id = item.Order_Key,
                    Menu_Id = item.Menu_Id,
                    Item_Id = item.Item_Id,
                    Quantity = item.Quantity,
                    Price_At_Time = item.Price_At_Time,
                    Status = item.Order_Item_Status,
                    Completed_At = item.Completed_At,
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Orders");
                return StatusCode(500, "Internal Server Error");
            }
        }
        //GET api/order
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

                [Authorize]
        [HttpPost("{order_id}/cancel")]
        public async Task<IActionResult> CancelOrder (
            int order_id
        )
        {
            try
            {
                var order = await _context.SessionOrders.FirstOrDefaultAsync(so => so.Order_Id == order_id);
                if (order is null)
                {
                    return NotFound("Order was not found to cancel");
                }
                if (order.Status != OrderStatus.Pending)
                {
                    return Conflict("Issue Canceling Order that is Approved");
                }
                if (order.Status == OrderStatus.Delivered)
                {
                    return BadRequest("Can not cancel a completed / delivered Order");
                }
                order.Status = OrderStatus.Cancelled;
                _context.SessionOrders.Update(order);
                await _context.SaveChangesAsync();
                var response = new OrderResponseDTO
                {
                    Order_Id = order.Order_Id,
                    Session_Id = order.session_id,
                    Bill_Id = order.Bill_Id,
                    User_Id = order.User_Id,
                    Status = order.Status,
                    Created_At = order.Created_At,
                };
                return Ok(response);

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