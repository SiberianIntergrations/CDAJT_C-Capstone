using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.domain.Entities;
using back_end.domain;
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
        
        /// <summary>
        /// Creates a new order for a dining session.
        /// </summary>
        /// <param name="order_data">The order creation data including session ID and bill ID</param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing the created <see cref="OrderResponseDTO"/> object.
        /// Returns HTTP 200 (OK) with the created order on success.
        /// Returns HTTP 404 (Not Found) if the session or bill doesn't exist.
        /// Returns HTTP 500 (Internal Server Error) if an exception occurs during creation.
        /// </returns>
        /// <response code="200">Returns the newly created order</response>
        /// <response code="404">If the dining session is not found or the bill is not found/open</response>
        /// <response code="500">If an internal error occurs while creating the order</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/order
        ///     {
        ///         "session_Id": 123,
        ///         "bill_Id": 456
        ///     }
        ///
        /// This endpoint requires authentication.
        /// The order is automatically created with 'Pending' status.
        /// The authenticated user is automatically assigned as the order owner.
        /// The bill must be in 'Open' status to create an order.
        /// </remarks>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(OrderResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                _logger.LogError(ex, "Error creating order for session {SessionId}", order_data.Session_Id);
                return StatusCode(500, new { message = "An error occurred while creating the order", error = ex.Message });
            }
        }

         /// <summary>
        /// Updates an existing order.
        /// </summary>
        /// <param name="order_id">The unique identifier of the order to update</param>
        /// <param name="order_data">The updated order data</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the updated <see cref="OrderResponseDTO"/> object.
        /// Returns HTTP 200 (OK) with the updated order on success.
        /// Returns HTTP 404 (Not Found) if the order doesn't exist.
        /// Returns HTTP 409 (Conflict) if the order is not pending and the user lacks permission.
        /// Returns HTTP 500 (Internal Server Error) if an exception occurs during the operation.
        /// </returns>
        /// <response code="200">Returns the updated order</response>
        /// <response code="404">If the order is not found</response>
        /// <response code="409">If the order is not pending and the user is not authorized to modify it</response>
        /// <response code="500">If an internal error occurs while updating the order</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/order/123
        ///     {
        ///         "status": "Approved"
        ///     }
        ///
        /// This endpoint requires authentication.
        /// Users can only update their own pending orders.
        /// Staff and Admin users can update approved orders.
        /// </remarks>
        [Authorize]
        [HttpPut("{order_id}")]
        [ProducesResponseType(typeof(OrderResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
               _logger.LogError(ex, "Error updating order {OrderId}", order_id);
                return StatusCode(500, new { message = "An error occurred while updating the order", error = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a specific item from an order.
        /// </summary>
        /// <param name="order_id">The unique identifier of the order</param>
        /// <param name="item_id">The unique identifier of the item to delete</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns HTTP 200 (OK) with a success message when the item is deleted.
        /// Returns HTTP 400 (Bad Request) if the order item doesn't exist.
        /// Returns HTTP 404 (Not Found) if the order doesn't exist.
        /// Returns HTTP 409 (Conflict) if the order is not pending and the user lacks permission.
        /// Returns HTTP 500 (Internal Server Error) if an exception occurs during the operation.
        /// </returns>
        /// <response code="200">Returns a success message when the item is deleted</response>
        /// <response code="400">If the order item is not found</response>
        /// <response code="404">If the order is not found</response>
        /// <response code="409">If the order is not pending and the user is not authorized to modify it</response>
        /// <response code="500">If an internal error occurs while deleting the order item</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/order/123/items/456
        ///
        /// This endpoint requires authentication.
        /// Users can only delete items from their own pending orders.
        /// Staff and Admin users can delete items from approved orders.
        /// </remarks>
        [Authorize]
        [HttpDelete("{order_id}/items/{item_id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteOrder(
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
                _logger.LogError(ex, "Error deleting item {ItemId} from order {OrderId}", item_id, order_id);
                return StatusCode(500, new { message = "An error occurred while deleting the order item", error = ex.Message });
            }
        }
        


        /// <summary>
        /// Updates the status of an order.
        /// </summary>
        /// <param name="order_id">The unique identifier of the order to update</param>
        /// <param name="new_Status">The new status to assign to the order</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns HTTP 200 (OK) with a success message when the order status is updated.
        /// Returns HTTP 404 (Not Found) if the order doesn't exist.
        /// Returns HTTP 500 (Internal Server Error) if an exception occurs during the operation.
        /// </returns>
        /// <response code="200">Returns a success message when the order status is updated</response>
        /// <response code="404">If the order is not found</response>
        /// <response code="500">If an internal error occurs while updating the order status</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/order/123/status
        ///     {
        ///         "new_Status": "Delivered"
        ///     }
        ///
        /// This endpoint requires Admin or Staff role authorization.
        /// When the status is set to 'Delivered', the completion timestamp is automatically set to the current UTC time.
        /// 
        /// Valid status values: Pending, Approved, InProgress, Delivered, Cancelled
        /// </remarks>
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost("{order_id}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> UpdateOrderStatus(
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
        

        /// <summary>
        /// Marks an order item as completed/delivered.
        /// </summary>
        /// <param name="item_id">The unique identifier of the order item to complete</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the updated <see cref="OrderItemResponseDTO"/> object.
        /// Returns HTTP 200 (OK) with the completed order item on success.
        /// Returns HTTP 404 (Not Found) if the order item doesn't exist.
        /// Returns HTTP 500 (Internal Server Error) if an exception occurs during the operation.
        /// </returns>
        /// <response code="200">Returns the completed order item</response>
        /// <response code="404">If the order item is not found</response>
        /// <response code="500">If an internal error occurs while completing the order item</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/order/items/123/complete
        ///
        /// This endpoint requires Admin or Staff role authorization.
        /// The order item status will be updated to 'Delivered' and the completion timestamp will be set.
        /// </remarks>
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost("items/{item_id}/complete")]
        [ProducesResponseType(typeof(OrderItemResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CompleteOrderItem(
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
    
    
    
    
        /// <summary>
        /// Retrieves all orders from the database.
        /// </summary>
        /// <returns>
        /// An <see cref="ActionResult"/> containing a collection of <see cref="SessionOrder"/> objects.
        /// Returns HTTP 200 (OK) with the list of all orders on success.
        /// Returns HTTP 500 (Internal Server Error) if an exception occurs during retrieval.
        /// </returns>
        /// <response code="200">Returns the list of all orders</response>
        /// <response code="500">If an internal error occurs while retrieving orders</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/order
        ///
        /// Returns all orders in the system without any filtering.
        /// </remarks>
        //GET api/order
        //Get all Orders
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SessionOrder>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Cancels a pending order.
        /// </summary>
        /// <param name="order_id">The unique identifier of the order to cancel</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the updated <see cref="OrderResponseDTO"/> object.
        /// Returns HTTP 200 (OK) with the cancelled order on success.
        /// Returns HTTP 400 (Bad Request) if the order has already been delivered.
        /// Returns HTTP 404 (Not Found) if the order doesn't exist.
        /// Returns HTTP 409 (Conflict) if the order is not in pending status.
        /// Returns HTTP 500 (Internal Server Error) if an exception occurs during cancellation.
        /// </returns>
        /// <response code="200">Returns the cancelled order</response>
        /// <response code="400">If the order has already been delivered and cannot be cancelled</response>
        /// <response code="404">If the order is not found</response>
        /// <response code="409">If the order is not in pending status</response>
        /// <response code="500">If an internal error occurs while cancelling the order</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/order/123/cancel
        ///
        /// This endpoint requires authentication.
        /// Only orders with 'Pending' status can be cancelled.
        /// Orders that are approved or delivered cannot be cancelled.
        /// </remarks>
        [Authorize]
        [HttpPost("{order_id}/cancel")]
        [ProducesResponseType(typeof(OrderResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Retrieves detailed information about a specific order including all items and customer details.
        /// </summary>
        /// <param name="id">The unique identifier of the order</param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing detailed order information.
        /// Returns HTTP 200 (OK) with the order details on success.
        /// Returns HTTP 404 (Not Found) if the order doesn't exist.
        /// Returns HTTP 500 (Internal Server Error) if an exception occurs during retrieval.
        /// </returns>
        /// <response code="200">Returns the detailed order information</response>
        /// <response code="404">If the order is not found</response>
        /// <response code="500">If an internal error occurs while retrieving the order</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/order/789
        ///
        /// Returns comprehensive order details including:
        /// - Order metadata (ID, session, bill, status, timestamps)
        /// - Customer information (name and email)
        /// - Complete list of ordered items with:
        ///   - Item details (ID, name, quantity, price)
        ///   - Individual item status
        ///   - Item subtotals
        /// - Total order cost
        /// </remarks>
        //GET: api/order/{id}
        //Get an Order with all the items
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                _logger.LogError(ex, "Error retrieving order {OrderId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the order", error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves all orders associated with a specific dining session.
        /// </summary>
        /// <param name="sessionId">The unique identifier of the dining session</param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing a collection of order summary objects.
        /// Returns HTTP 200 (OK) with the list of orders sorted by creation time on success.
        /// Returns HTTP 404 (Not Found) if the session doesn't exist.
        /// Returns HTTP 500 (Internal Server Error) if an exception occurs during retrieval.
        /// </returns>
        /// <response code="200">Returns the list of all orders for the specified session</response>
        /// <response code="404">If the session is not found</response>
        /// <response code="500">If an internal error occurs while retrieving orders</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/order/session/456
        ///
        /// Returns order summaries including:
        /// - Order and customer details (ID, name)
        /// - Order status
        /// - Item count and total price
        /// - Timestamps (created and completed)
        /// 
        /// Orders are sorted by creation date in ascending order (oldest first).
        /// </remarks>
        //GET: api/order/session/{id}
        //Get all orders tied to a Dining Session
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
                _logger.LogError(ex, "Error retrieving orders for session {SessionId}", sessionId);
                return StatusCode(500, new { message = "An error occurred while retrieving session orders", error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves all orders placed by a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user</param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing a collection of order history objects.
        /// Returns HTTP 200 (OK) with the list of orders sorted by most recent on success.
        /// Returns HTTP 404 (Not Found) if the user doesn't exist.
        /// Returns HTTP 500 (Internal Server Error) if an exception occurs during retrieval.
        /// </returns>
        /// <response code="200">Returns the list of all orders for the specified user</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="500">If an internal error occurs while retrieving orders</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/order/user/123
        ///
        /// Returns order history including:
        /// - Order details (ID, session, menu name, status)
        /// - Item count and total price
        /// - Timestamps (created and completed)
        /// - List of items with names and quantities
        /// 
        /// Orders are sorted by creation date in descending order (newest first).
        /// </remarks>
        //GET: api/order/user{id}
        //Get all orders placed by a specific customer
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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