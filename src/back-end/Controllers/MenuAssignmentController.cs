using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.domain.Entities;
using back_end.DTO.MenuItemAssignmentDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualBasic;

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
                    Tot_Limit = assignmentData.Tot_Limit ?? 0,
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
        [Authorize(Roles = "Admin,Staff")]
        [HttpPut("/{menu_id}/item_id")]
        public async Task<IActionResult> Update_Menu_Item_Assignment(
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
                var finalTotLimit = updateData.Tot_Limit ?? assignment.Tot_Limit;
                var finalTotalLimit = updateData.Total_Units_Ordered ?? assignment.Total_Units_Ordered;
                if ((finalChildLimit > assignment.Adult_Limit) || finalChildLimit > assignment.Tot_Limit)
                {
                    return BadRequest("Child Limit Can not be Greater then a Adult or Total Limit");
                }
                if (finalTotLimit > assignment.Adult_Limit)
                {
                    return BadRequest("Toddler limit must be less then a adult  ");
                }
                if (finalSeniorLimit > assignment.Adult_Limit)
                {
                    return BadRequest("Senior Limit Can not be Greater then a Adult or Total limit");
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
                    assignment.Tot_Limit = (int)updateData.Tot_Limit;
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
                    assignment.LastViewedAt = DateTime.UtcNow;
                }
                assignment.Price = updateData.Price;
                assignment.Is_Add_On = updateData.Is_Add_on;
                assignment.Status = updateData.Status;
                _context.Update(assignment);
                await _context.SaveChangesAsync();
                return Ok(assignment);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active session");
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpDelete("/{menu_id}/{Item_id}")]
        public async Task<IActionResult> Delete_Menu_Item_Assignment(
            int menu_id,
            int Item_id
        )
        {
            try
            {
                var assignment = await _context.MenuItemAssignments.Where(mia => mia.Menu_Id == menu_id && mia.Item_Id == Item_id).FirstAsync();
                if (assignment == null)
                {
                    return NotFound("Menu Assignment for deletion was not found");
                }
                _context.Remove(assignment);
                await _context.SaveChangesAsync();
                return Ok("Menu Assignment was Deleted Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active session");
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPost("/copy_men/{source_menu_id}")]
        public async Task<IActionResult> Copy_Menu_Assignments(
            int source_menu_id,
            string new_menu_name
        )
        {
            try
            {
                var existingMenu = await _context.Menus.Where(m => m.Name == new_menu_name).FirstOrDefaultAsync();
                if (existingMenu != null)
                {
                    return Conflict("Menu Name Already Exist");
                }
                var new_menu = new Menu
                {
                    Name = new_menu_name,
                };
                _context.Add(new_menu);
                await _context.SaveChangesAsync();
                var sourceAssignments = await _context.MenuItemAssignments.Where(mia => mia.Menu_Id == source_menu_id).ToListAsync();

                var newAssignment = new List<MenuItemAssignment>();
                foreach (var element in sourceAssignments)
                {
                    newAssignment.Add(new MenuItemAssignment
                    {
                        Menu_Id = new_menu.Menu_id,
                        Item_Id = element.Item_Id,
                        Price = element.Price,
                        Adult_Limit = element.Adult_Limit,
                        Child_limit = element.Child_limit,
                        Senior_limit = element.Senior_limit,
                        Tot_Limit = element.Tot_Limit,
                        Total_Units_Ordered = element.Total_Units_Ordered,
                        Total_Views = element.Total_Views,
                        Total_View_Seconds = element.Total_View_Seconds

                    });
                }
                _context.AddRange(newAssignment);
                await _context.SaveChangesAsync();
                
                return Ok(newAssignment);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active session");
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
    }

}