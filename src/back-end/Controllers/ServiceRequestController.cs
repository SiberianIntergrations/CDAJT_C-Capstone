using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.domain.Entities;
using Microsoft.AspNetCore.Authorization;
using back_end.DTO.ServiceRequestDTOs;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using back_end.domain.enums;

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

        [Authorize]
        [HttpPost("{session_id}")]
        public async Task<IActionResult> CreateServiceRequest(
            int session_id,
            ServiceRequestCreateDTO request_data
        )
        {
            try
            {
                var session = await _context.DiningSessions
                    .Include(t => t.Tables)
                    .FirstOrDefaultAsync(ds => ds.Session_Id == session_id && ds.Ended_At == null);

                if (session is null)
                {
                    return NotFound("The Session Id was Not found");
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = await _context.Users.FirstOrDefaultAsync(u => u.User_id == int.Parse(userId));
                if (user is null)
                {
                    return BadRequest("Issue in processing your request");
                }

                var participant = await _context.SessionParticipants
                    .FirstOrDefaultAsync(sp => sp.Session_Id == session_id && sp.User_Id == user.User_id && sp.Left_At == null);

                if (participant is null)
                {
                    return BadRequest("Issue in Processing your request for session");
                }

                //Locate table that's connected to this session
                var tableId = await _context.SessionTables
                    .Where(st => st.Session_Id == session_id)
                    .Select(st => st.Table_Id)
                    .FirstOrDefaultAsync();

                if (tableId == 0)
                {
                    return BadRequest("No table is assigned to this session.");
                }

                var serviceRequest = new ServiceRequest
                {
                    Session_Id = session.Session_Id,
                    Table_Id = tableId,
                    Request_By = user.User_id,
                    Notes = request_data.Notes,
                    Status = ServiceRequestStatus.Pending
                };

                _context.ServiceRequests.Add(serviceRequest);
                await _context.SaveChangesAsync();

                return Ok(new ServiceRequestResponseDTO
                {
                    Request_Id = serviceRequest.request_id,
                    Session_Id = serviceRequest.Session_Id,
                    Table_Id = serviceRequest.Table_Id,
                    Requested_By = serviceRequest.Request_By,
                    Status = serviceRequest.Status,
                    Notes = serviceRequest.Notes,
                    Created_At = serviceRequest.Created_At,
                    Table_Number = serviceRequest.Table_Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Creating Service Request");
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPost("{request_id}/complete")]
        public async Task<IActionResult> CompleteServiceRequest(
            int request_id
        )
        {
            try
            {
                var sessionRequest = await _context.ServiceRequests.FirstOrDefaultAsync(sr => sr.request_id == request_id);
                if (sessionRequest is null)
                {
                    return NotFound("The Service was not found");
                }
                sessionRequest.Status = ServiceRequestStatus.Completed;
                var claimedByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var claimedByUser = await _context.Users.FirstOrDefaultAsync(u => u.User_id == int.Parse(claimedByUserId));
                sessionRequest.ClaimedByUser = claimedByUser;
                sessionRequest.Completed_At = DateTime.UtcNow;
                _context.ServiceRequests.Update(sessionRequest);
                await _context.SaveChangesAsync();
                return Ok(sessionRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Getting Menu Item Tags with Colors");
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }

        }


        [Authorize(Roles = "Admin,Staff")]
        [HttpPost("by-session/{session_id}")]
        public async Task<IActionResult> GetSessionServiceRequest(
            int session_id
        )
        {
            try
            {
                var session = await _context.ServiceRequests.FirstOrDefaultAsync(sr => sr.Session_Id == session_id);
                if (session is null)
                {
                    return NotFound("The Service was not found");
                }
                var sessionRequestList = await _context.ServiceRequests
                    .Where(sr => sr.Session_Id == session_id && sr.Status == ServiceRequestStatus.Pending)
                    .ToListAsync();
                if (sessionRequestList == null || !sessionRequestList.Any())
                {
                    return NotFound("No Pending Requests have been found");
                }
                var response = sessionRequestList.Select(sr => new ServiceRequestResponseDTO
                {
                    Request_Id = sr.request_id,
                    Session_Id = sr.Session_Id,
                    Table_Id = sr.Table_Id,
                    Status = sr.Status,
                    Requested_By = sr.Request_By,
                    Claimed_By = sr.Claimed_By,
                    Notes = sr.Notes,
                    Created_At = sr.Created_At,
                
                }).ToList();

                return Ok(response);               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Getting Menu Item Tags with Colors");
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
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