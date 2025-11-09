using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.domain.Entities;
using back_end.domain.enums;
using back_end.DTO.OrdersDTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using back_end.Helpers;

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
        public async Task<ActionResult<IEnumerable<SessionOrder>>> CreateOrder(OrderCreateDTO order_data)
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
                // Use Oauth user info
                var userOid = ClaimsHelpers.GetUserOid(User);
                var userName = ClaimsHelpers.GetUserDisplayName(User);
                if (string.IsNullOrEmpty(userOid))
                {
                    return Unauthorized(new { message = "User Oid not found in claims." });
                }
                var newOrder = new SessionOrder
                {
                    session_id = order_data.Session_Id,
                    Bill_Id = order_data.Bill_Id,
                    User_Oid = userOid,
                    User_Name = userName,
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
                    User_Oid = newOrder.User_Oid,
                    User_Name = newOrder.User_Name,
                    Status = newOrder.Status,
                    Created_At = newOrder.Created_At,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for session {SessionId}", order_data.Session_Id);
                return StatusCode(500, new { message = "An error occurred while creating the order", error = ex.Message });
            }
        }

         /// <summary>
        /// Approves a pending order and transitions to Approved/Processing status.
        /// </summary>
        [Authorize(Policy = "staffOnly")]
        [HttpPatch("{order_id}/approve")]
        [ProducesResponseType(typeof(OrderResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ApproveOrder(int order_id)
        {
            try
            {
                var order = await _context.SessionOrders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.MenuItem)
                    .FirstOrDefaultAsync(o => o.Order_Id == order_id);

                if (order is null)
                {
                    return NotFound("Order not found");
                }
                // Use Oauth user info
                var userOid = ClaimsHelpers.GetUserOid(User);
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
                if (string.IsNullOrEmpty(userOid))
                {
                    return Unauthorized("User Oid not found in claims.");
                }

                if (order.Status != OrderStatus.Pending)
                {
                    return BadRequest("Only pending orders can be approved");
                }

                if (!order.OrderItems.Any())
                {
                    return BadRequest("Cannot approve order with no items");
                }

                order.Status = OrderStatus.Processing;

                foreach (var item in order.OrderItems)
                {
                    item.Order_Item_Status = OrderStatus.Processing;
                }

                _context.SessionOrders.Update(order);
                await _context.SaveChangesAsync();

                return Ok(new OrderResponseDTO
                {
                    Order_Id = order.Order_Id,
                    Session_Id = order.session_id,
                    Bill_Id = order.Bill_Id,
                    User_Oid = order.User_Oid,
                    User_Name = order.User_Name,
                    Status = order.Status,
                    Created_At = order.Created_At,
                    OrderItems = order.OrderItems.Select(oi => new OrderItemResponseDTO
                    {
                        Order_Item_Id = oi.Order_Item_Id,
                        Order_Id = oi.Order_Key,
                        Menu_Id = oi.Menu_Id,
                        Item_Id = oi.Item_Id,
                        Quantity = oi.Quantity,
                        Price_At_Time = oi.Price_At_Time,
                        Status = oi.Order_Item_Status,
                        Name = oi.MenuItem?.Name
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving order");
                return StatusCode(500, "Internal Server Error");
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
        [Authorize(Policy = "staffOnly")]
        [HttpDelete("{order_id}/items/{order_item_id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveOrderItem(
            int order_id,
            int order_item_id
        )
        {
            try
            {
                var order = await _context.SessionOrders.Include(o => o.OrderItems).FirstOrDefaultAsync(so => so.Order_Id == order_id);
                if (order is null)
                {
                    return NotFound("Order Data was not found");
                }
                // Use Oauth user info
                var userOid = ClaimsHelpers.GetUserOid(User);
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
                if (string.IsNullOrEmpty(userOid))
                {
                    return Unauthorized("User Oid not found in claims.");
                }
                if (order.Status != OrderStatus.Pending)
                {
                    // TODO: Please verify new functionality
                    if (!(userRole.Contains("user.Staff") || userRole.Contains("user.Admin")) && userOid != order.User_Oid)
                    {
                        return Conflict("Can not Modify a Approved Order");
                    }
                }
                 var item = order.OrderItems.FirstOrDefault(oi => oi.Order_Item_Id == order_item_id);
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
                _logger.LogError(ex, "Error deleting item {ItemId} from order {OrderId}", order_item_id, order_id);
                return StatusCode(500, new { message = "An error occurred while deleting the order item", error = ex.Message });
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
        [Authorize(Policy = "staffOnly")]
        [HttpPatch("{order_id}/items/{item_id}/complete")]
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
        /// Marks the whole order as delivered/completed.
        /// </summary>
        [Authorize(Policy = "staffOnly")]
        [HttpPatch("{order_id}/complete")]
        [ProducesResponseType(typeof(OrderResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CompleteOrder(int order_id)
        {
            try
            {
                var order = await _context.SessionOrders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.MenuItem)
                    .FirstOrDefaultAsync(o => o.Order_Id == order_id);

                if (order is null)
                {
                    return NotFound("Order not found");
                }

                if (order.Status != OrderStatus.Processing)
                {
                    return BadRequest("Only approved orders can be marked as completed");
                }

                order.Status = OrderStatus.Delivered;
                order.Completed_At = DateTime.UtcNow;

                foreach (var item in order.OrderItems)
                {
                    item.Order_Item_Status = OrderStatus.Delivered;
                    item.Completed_At = DateTime.UtcNow;
                }

                _context.SessionOrders.Update(order);
                await _context.SaveChangesAsync();

                return Ok(new OrderResponseDTO
                {
                    Order_Id = order.Order_Id,
                    Session_Id = order.session_id,
                    Bill_Id = order.Bill_Id,
                    User_Oid = order.User_Oid,
                    Status = order.Status,
                    Created_At = order.Created_At,
                    Completed_At = order.Completed_At,
                    OrderItems = order.OrderItems.Select(oi => new OrderItemResponseDTO
                    {
                        Order_Item_Id = oi.Order_Item_Id,
                        Order_Id = oi.Order_Key,
                        Menu_Id = oi.Menu_Id,
                        Item_Id = oi.Item_Id,
                        Quantity = oi.Quantity,
                        Price_At_Time = oi.Price_At_Time,
                        Status = oi.Order_Item_Status,
                        Completed_At = oi.Completed_At,
                        Name = oi.MenuItem?.Name
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing order");
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
        [HttpPatch("{order_id}/cancel")]
        [ProducesResponseType(typeof(OrderResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CancelOrder(
            int order_id
        )
        {
            try
            {
                var order = await _context.SessionOrders.Include(o => o.OrderItems).FirstOrDefaultAsync(so => so.Order_Id == order_id);
                if (order is null)
                {
                    return NotFound("Order was not found to cancel");
                }
                if (order.Status == OrderStatus.Delivered)
                {
                    return BadRequest("Cannot cancel a delivered order");
                }
                if (order.Status != OrderStatus.Pending)
                {
                    return Conflict("Can only cancel pending orders");
                }
                order.Status = OrderStatus.Cancelled;
                order.Completed_At = DateTime.UtcNow;
                foreach (var item in order.OrderItems)
                {
                    item.Order_Item_Status = OrderStatus.Cancelled;
                    item.Completed_At = DateTime.UtcNow;
                }

                _context.SessionOrders.Update(order);
                await _context.SaveChangesAsync();
                return Ok(new OrderResponseDTO
                {
                    Order_Id = order.Order_Id,
                    Session_Id = order.session_id,
                    Bill_Id = order.Bill_Id,
                    User_Oid = order.User_Oid,
                    User_Name = order.User_Name,
                    Status = order.Status,
                    Created_At = order.Created_At,
                    Completed_At = order.Completed_At,
                    OrderItems = order.OrderItems.Select(oi => new OrderItemResponseDTO
                    {
                        Order_Item_Id = oi.Order_Item_Id,
                        Order_Id = oi.Order_Key,
                        Menu_Id = oi.Menu_Id,
                        Item_Id = oi.Item_Id,
                        Quantity = oi.Quantity,
                        Price_At_Time = oi.Price_At_Time,
                        Status = oi.Order_Item_Status,
                        Completed_At = oi.Completed_At,
                        Name = oi.MenuItem?.Name
                    }).ToList()
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Orders");
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Adds items to the created order from POST: api/Order
        /// POST: api/Order/{order_id}/items
        /// </summary>
        [Authorize]
        [HttpPost("{order_id}/items")]
        [ProducesResponseType(typeof(List<OrderItemResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<OrderItemInsertResponse>>> AddOrderItems(int order_id, [FromBody] List<OrderItemCreateDTO> items)
        {
            try
            {
                //Make sure there are items inside the order
                if (items is null || items.Count == 0)
                {
                    return BadRequest("No items added to order.");
                }

                //Make sure the order exists so items can be added to it
                var order = await _context.SessionOrders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Order_Id == order_id);

                if (order is null)
                {
                    return NotFound($"Order {order_id} was not found");
                }

                if (order.Status != OrderStatus.Pending)
                {
                    return Conflict("Cannot modify a non-pending order.");
                }

                // Use Oauth user info
                var userOid = ClaimsHelpers.GetUserOid(User);
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
                if (string.IsNullOrEmpty(userOid))
                {
                    return Unauthorized("User Oid not found in claims.");
                }
                if (order.User_Oid != userOid)
                {
                    return BadRequest("You can only add items to your own orders");
                }

                var addedItems = new List<OrderItems>();

                foreach (var itemDto in items)
                {
                    var menuItem = await _context.MenuItems.FirstOrDefaultAsync(mi => mi.item_id == itemDto.Item_Id);
                    if (menuItem == null)
                    {
                        return NotFound($"Menu item {itemDto.Item_Id} not found");
                    }

                    if (itemDto.Quantity <= 0)
                    {
                        return BadRequest($"Quantity must be positive for item '{menuItem.Name}' (ID: {itemDto.Item_Id})");
                    }

                    if (menuItem.Status != MenuItemStatus.Available)
                    {
                        return BadRequest($"Item '{menuItem.Name}' is currently unavailable");
                    }

                    var orderItem = new OrderItems
                    {
                        Order_Key = order_id,
                        Menu_Id = itemDto.Menu_Id,
                        Item_Id = itemDto.Item_Id,
                        Quantity = itemDto.Quantity,
                        Price_At_Time = itemDto.Price_At_Time,
                        Order_Item_Status = OrderStatus.Pending,
                        Completed_At = null
                    };

                    _context.OrderItems.Add(orderItem);
                    addedItems.Add(orderItem);
                }

                await _context.SaveChangesAsync();

                //Package up the order in a neat fashion
                var response = addedItems.Select(item => new OrderItemResponseDTO
                {
                    Order_Item_Id = item.Order_Item_Id,
                    Order_Id = item.Order_Key,
                    Menu_Id = item.Menu_Id,
                    Item_Id = item.Item_Id,
                    Quantity = item.Quantity,
                    Price_At_Time = item.Price_At_Time,
                    Status = item.Order_Item_Status,
                    Name = item.MenuItem?.Name
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding items to order {OrderId}", order_id);
                return StatusCode(500, new { message = "An error occurred while adding order items", error = ex.Message });
            }
        }

        /// <summary>
        /// Updates order items before approval (pending orders only).
        /// </summary>
        [Authorize]
        [HttpPatch("{order_id}/items/{item_id}")]
        [ProducesResponseType(typeof(OrderItemResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrderItem(
            int order_id, 
            int item_id,
            [FromBody] OrderItemUpdateDTO updateData
        )
        {
            try
            {
                var order = await _context.SessionOrders
                    .FirstOrDefaultAsync(o => o.Order_Id == order_id);

                if (order is null)
                {
                    return NotFound("Order not found");
                }

                // Use OAuth user info
                var userOid = ClaimsHelpers.GetUserOid(User);
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
                
                if (string.IsNullOrEmpty(userOid))
                {
                    return Unauthorized("User Oid not found in claims.");
                }
                
                // Check if user is the owner of the order
                bool isOwner = order.User_Oid == userOid;
                
                // Check if user is staff/admin
                bool isStaff = userRole.Contains("user.Staff") || userRole.Contains("user.Admin");

                if (!isOwner && !isStaff)
                {
                    return Forbid("You don't have permission to modify this order");
                }

                // Can only modify pending orders
                if (order.Status != OrderStatus.Pending)
                {
                    return BadRequest("Can only modify pending orders");
                }

                var orderItem = await _context.OrderItems
                    .Include(oi => oi.MenuItem)
                    .FirstOrDefaultAsync(oi => oi.Order_Item_Id == item_id && oi.Order_Key == order_id);

                if (orderItem is null)
                {
                    return NotFound("Order item not found");
                }

                // Update quantity if provided
                if (updateData.Quantity.HasValue)
                {
                    if (updateData.Quantity.Value <= 0)
                    {
                        return BadRequest("Quantity must be positive");
                    }
                    if (updateData.Quantity.Value > 99)
                    {
                        return BadRequest("Quantity cannot exceed 99");
                    }
                    orderItem.Quantity = updateData.Quantity.Value;
                }

                // Update price if provided (staff only)
                if (updateData.Price_At_Time.HasValue)
                {
                    if (!isStaff)
                    {
                        return Forbid("Only staff can modify prices");
                    }
                    if (updateData.Price_At_Time.Value < 0)
                    {
                        return BadRequest("Price cannot be negative");
                    }
                    orderItem.Price_At_Time = updateData.Price_At_Time.Value;
                }

                await _context.SaveChangesAsync();

                return Ok(new OrderItemResponseDTO
                {
                    Order_Item_Id = orderItem.Order_Item_Id,
                    Order_Id = orderItem.Order_Key,
                    Menu_Id = orderItem.Menu_Id,
                    Item_Id = orderItem.Item_Id,
                    Quantity = orderItem.Quantity,
                    Price_At_Time = orderItem.Price_At_Time,
                    Status = orderItem.Order_Item_Status,
                    Completed_At = orderItem.Completed_At,
                    Name = orderItem.MenuItem?.Name
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order item");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // TODO: Add filters by session or date range instead of returning all orders from the database.
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
        [Authorize(Policy = "staffOnly")]
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
                            // .Include(o => o.User) // Removed for Oauth
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
                        userOid = order.User_Oid,
                        userName = order.User_Name
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
                                        .Include(o => o.Bill)
                                        // .Include(o => o.User) // Removed for Oauth
                                        .Include(o => o.OrderItems)
                                            .ThenInclude(or => or.MenuItem)
                                        .OrderBy(o => o.Created_At)
                                        .ToListAsync();

                var orderAll = sessionOrder.Select(o => new
                {
                    orderId = o.Order_Id,
                    billId = o.Bill_Id,
                    billStatus = o.Bill?.Status.ToString(),
                    sessionId = o.session_id,
                    userOid = o.User_Oid,
                    userName = o.User_Name,
                    status = o.Status,
                    itemTotal = o.OrderItems.Count,
                    orderTotal = o.OrderItems.Sum(or => or.Quantity * or.Price_At_Time),
                    createdAt = o.Created_At,
                    completedAt = o.Completed_At,
                    // List of items in the order
                    items = o.OrderItems.Select(oi => new
                    {
                        orderItemId = oi.Order_Item_Id,
                        itemId = oi.Item_Id,
                        name = oi.MenuItem.Name,
                        quantity = oi.Quantity,
                        priceAtTime = oi.Price_At_Time,
                        status = oi.Order_Item_Status
                    }).ToList()
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
        /// <param name="userOid">The unique identifier of the user (OID)</param>
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
        ///     GET /api/order/user/{userOid}
        ///
        /// Returns order history including:
        /// - Order details (ID, session, menu name, status)
        /// - Item count and total price
        /// - Timestamps (created and completed)
        /// - List of items with names and quantities
        /// 
        /// Orders are sorted by creation date in descending order (newest first).
        /// </remarks>
        //GET: api/order/user/{userOid}
        //Get all orders placed by a specific customer
        [HttpGet("user/{userOid}")]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<object>>> GetOrderByUser(string userOid)
        {
            try
            {
                //Check if userOid is valid
                if (string.IsNullOrWhiteSpace(userOid))
                {
                    return NotFound(new { message = $"Can't find user with Oid: {userOid}" });
                }

                //Get all orders from this user and sort by the newest
                //Include session menu and item details
                var order = await _context.SessionOrders
                            .Where(o => o.User_Oid == userOid)
                            .Include(o => o.DiningSession)
                                .ThenInclude(or => or.Menu)
                            .Include(o => o.OrderItems)
                                .ThenInclude(od => od.MenuItem)
                            .OrderByDescending(o => o.Created_At)
                            .ToListAsync();

                //Include Order totals and items for each OrderID
                var orderHistory = order.Select(o => new
                {
                    orderId = o.Order_Id,
                    sessionId = o.session_id,
                    billId = o.Bill_Id,
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
                _logger.LogError(ex, $"Can't find Order for User Oid: {userOid}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        //GET: api/Order/active-session/orders
        //Get all of the orders for the user's ACTIVE session
        //Add an optional Bill ID parameter in-case of bill splitting
        [Authorize]
        [HttpGet("active-session/orders")]
        public async Task<ActionResult<IEnumerable<object>>> GetOrdersForActiveSession([FromQuery] int? bill_id = null)
        {

            try
            {
                //Get the currently logged in user's Oid
                var userOid = ClaimsHelpers.GetUserOid(User);
                if (string.IsNullOrWhiteSpace(userOid))
                {
                    return Unauthorized(new { message = $"Can't find user Oid in claims." });
                }

                //Get the user's session that's currently active
                var activeSessionId = await (
                    from p in _context.SessionParticipants
                    join ds in _context.DiningSessions on p.Session_Id equals ds.Session_Id
                    where p.User_Oid == userOid
                    && p.Left_At == null
                    && ds.Ended_At == null
                    orderby ds.Started_At descending
                    select ds.Session_Id
                ).FirstOrDefaultAsync();

                if (activeSessionId == 0)
                {
                    return NotFound("No active session for this user");
                }

                var orders = await _context.SessionOrders
                                .Where(o => o.User_Oid == userOid && o.session_id == activeSessionId && (bill_id == null || o.Bill_Id == bill_id))
                                .Include(o => o.DiningSession)
                                    .ThenInclude(or => or.Menu)
                                .Include(o => o.Bill)
                                .Include(o => o.OrderItems)
                                    .ThenInclude(od => od.MenuItem)
                                .OrderByDescending(o => o.Created_At)
                                .ToListAsync();

                //Package up the user's Order and add in all the menu item's details
                var userOrder = orders.Select(o => new
                {
                    orderId = o.Order_Id,
                    SessionId = o.session_id,
                    bill_id = o.Bill_Id,
                    billStatus = o.Bill?.Status.ToString(),
                    status = o.Status.ToString(),
                    createdAt = o.Created_At,
                    completedAt = o.Completed_At,
                    items = o.OrderItems.Select(i => new
                    {
                        orderItemId = i.Order_Item_Id,
                        itemId = i.Item_Id,
                        name = i.MenuItem.Name,
                        quantity = i.Quantity,
                        price = i.Price_At_Time,
                        status = i.Order_Item_Status.ToString()
                    }).ToList()
                });

                return Ok(userOrder);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Can't get user's active orders");
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}