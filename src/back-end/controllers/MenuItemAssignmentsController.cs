using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.domain.Entities;
using back_end.DTO.MenuItemAssignmentDTO;

namespace back_end.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuAssignmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MenuAssignmentController> _logger;

        public MenuAssignmentController(ApplicationDbContext context, ILogger<MenuAssignmentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("/")]
        public async Task<IActionResult> Create_Menu_Item_Assignment(
            MenuAssignmentCreate assignmentData
        )
        {

            try{
                var assignment = new MenuItemAssignment
                {
                    Menu_Id = assignmentData.Menu_Id,
                    Item_Id = assignmentData.Item_Id,
                    Price = assignmentData.Price,
                    Total_Units_Ordered = assignmentData.Total_Units_Ordered ?? 0,
                    Total_Views = assignmentData.Total_Views ?? 0,
                    Total_View_Seconds = assignmentData.Total_View_Seconds ?? 0,
                    Adult_Limit = assignmentData.Adult_Limit ?? 0,
                    Child_limit = assignmentData.Child_Limit ?? 0,
                    Senior_limit = assignmentData.Senior_Limit ?? 0,
                    Total_Limit = assignmentData.Tot_Limit ?? 0,
                    Status = assignmentData.Status
                };
            
                await _context.MenuItemAssignments.AddAsync(assignment);
                await _context.SaveChangesAsync();
                return Ok(assignment);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }

        [HttpPut("/{menu_id}/item_id")]
        public async Task<IActionResult>Update_Menu_Item_Assignment(
            int menu_id,
            int item_id,
            MenuAssignmentUpdateDTO updateData
        )
        {
            try
            {
                var assignment = await _context.MenuItemAssignments.Where(mia => mia.Menu_Id == menu_id && mia.Item_Id == item_id).FirstOrDefaultAsync();
                if (assignment == null)
                {
                    return NotFound("Menu Assignment was not found.");
                }
                var finalAdultLimit = updateData.Adult_Limit ?? assignment.Adult_Limit;
                var finalSeniorLimit = updateData.Senior_Limit ?? assignment.Senior_limit;
                var finalChildLimit = updateData.Child_Limit ?? assignment.Child_limit;
                var finalTotalLimit = updateData.Tot_Limit ?? assignment.Total_Limit;
                if ((finalChildLimit > assignment.Adult_Limit) || finalChildLimit > assignment.Total_Limit)
                {
                    return BadRequest("Child Limit Can not be Greater then a Adult or Total Limit");
                }
                if ((finalSeniorLimit > assignment.Adult_Limit) || (finalSeniorLimit > assignment.Total_Limit))
                {
                    return BadRequest("Senior Limit Can not be Graeter then a Adult or Total limit");
                }
                if (finalAdultLimit + finalChildLimit + finalSeniorLimit > finalTotalLimit)
                {
                    return BadRequest($"Adult:{finalAdultLimit},Senior:{finalSeniorLimit},Child{finalSeniorLimit} total:{finalAdultLimit + finalSeniorLimit + finalChildLimit} Limits can not be greater then Total Limit{finalTotalLimit}");
                }
                if (updateData.Adult_Limit.HasValue)
                {
                    assignment.Adult_Limit = (int)updateData.Adult_Limit;
                }
                if (updateData.Senior_Limit.HasValue)
                {
                    assignment.Senior_limit = (int)updateData.Senior_Limit;
                }
                if (updateData.Tot_Limit.HasValue)
                {
                    assignment.Total_Limit = (int)updateData.Tot_Limit;
                }
                if (updateData.Total_Units_Ordered.HasValue)
                {
                    assignment.Total_Units_Ordered = (int)updateData.Total_Units_Ordered;
                }
                if (updateData.Total_Views.HasValue)
                {
                    assignment.Total_Views = assignment.Total_Views + (int)updateData.Total_Views;
                }
                if (updateData.Total_View_Seconds.HasValue)
                {
                    assignment.Total_View_Seconds = assignment.Total_View_Seconds + (int)updateData.Total_View_Seconds;
                }
                if (updateData.Last_Ordered_At.HasValue)
                {
                    assignment.LastOrdered = DateTime.UtcNow;
                }
                if (updateData.Last_Viewed_At.HasValue)
                {
                    assignment.LastOrdered = DateTime.UtcNow;
                }
                assignment.Price = updateData.Price;
                assignment.Is_Add_On = updateData.Is_Add_on;
                assignment.Status = updateData.Status;
                _context.Update(assignment);
                await _context.SaveChangesAsync();
                return Ok(assignment);

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active session");
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
    }

}