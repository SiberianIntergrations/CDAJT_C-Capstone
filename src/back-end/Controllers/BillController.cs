using System.IO.Compression;
using System.Numerics;
using back_end.domain;
using back_end.domain.DbContexts;
using back_end.domain.Entities;
using back_end.domain.enums;
using back_end.DTO.bill;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace back_end.controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class BillController : ControllerBase
  {
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;
    private readonly ILogger<BillController> _logger;

    public BillController(ApplicationDbContext context, IConfiguration config, ILogger<BillController> logger)
    {
      _context = context;
      _config = config;
      _logger = logger;
    }

    // Helper method to get table numbers from session
    private List<int> GetTableNumbers(DiningSession session)
    {
      if (session.Table != null)
      {
        return new List<int> { session.Table.table_number };
      }
      else if (session.TableGroup != null && session.TableGroup.Tables != null)
      {
        return session.TableGroup.Tables.Select(t => t.table_number).ToList();
      }
      return new List<int>();
    }

    [HttpPost("create_Bill/{_session_id}")]
    public async Task<IActionResult> Create_Bill(int _session_id, CreateBill bill_data)
    {
      try
      {
        var session = await _context.DiningSessions
            .Include(ds => ds.Table)
            .Include(ds => ds.TableGroup)
                .ThenInclude(tg => tg.Tables)
            .FirstOrDefaultAsync(ds => ds.Session_Id == _session_id);

        if (session == null)
        {
          return NotFound(new { message = "Bill or session was not found" });
        }

        if (session.Ended_At == null)
        {
          return BadRequest(new { message = $"Cannot create bill for session that ended {session.Ended_At}" });
        }

        // Check if session has either a table or table group assigned
        if (!session.Table_Id.HasValue && !session.TableGroup_Id.HasValue)
        {
          return BadRequest(new { message = "Cannot create a bill for a session that has no table or table group assigned" });
        }

        if (bill_data.Adult_Count <= 0 && bill_data.Senior_Count <= 0 && bill_data.Child_Count <= 0)
        {
          return BadRequest(new { message = "Bill must have at least one guest in the session" });
        }

        // Validate Bill_Name is required
        if (string.IsNullOrWhiteSpace(bill_data.Bill_Name))
        {
          return BadRequest(new { message = "Bill_Name is required." });
        }

        var NewBill = new Billing
        {
          Session_Id = _session_id,
          Bill_Name = bill_data.Bill_Name!, // safe after validation
          Senior_Count = bill_data.Senior_Count,
          Adult_Count = bill_data.Adult_Count,
          Child_Count = bill_data.Child_Count,
          Total_Count = bill_data.Tot_Count,
          Status = BillStatus.Open
        };

        _context.Bills.Add(NewBill);
        await _context.SaveChangesAsync();
        await _context.Entry(NewBill).ReloadAsync();

        return Ok(new BillResponse
        {
          Bill_Id = NewBill.Bill_Id,
          Session_Id = NewBill.Session_Id,
          Bill_Name = NewBill.Bill_Name,
          Senior_Count = NewBill.Senior_Count,
          Adult_Count = NewBill.Adult_Count,
          Child_Count = NewBill.Child_Count,
          Total_Count = NewBill.Total_Count,
          Status = NewBill.Status.ToString(),
          Created_At = NewBill.Created_At,
          Closed_At = NewBill.Closed_At.ToString(),
          Table_Numbers = GetTableNumbers(session)
        });
      }
      catch (DbUpdateException ex)
      {
        Console.WriteLine($"Error in Create Bill {ex}");
        return StatusCode(500, "There was a Problem in the Create Bill Method");
      }
    }

    [HttpGet("get_bills/{_session_id}")]
    public async Task<IActionResult> Get_Bills(int _session_id, int? _table_id = null)
    {
      try
      {
        var session = await _context.DiningSessions
            .Include(ds => ds.Table)
            .Include(ds => ds.TableGroup)
                .ThenInclude(tg => tg.Tables)
            .FirstOrDefaultAsync(ds => ds.Session_Id == _session_id);

        if (session == null)
        {
          return NotFound(new { message = "Session was not found" });
        }

        var billQuery = _context.Bills.Where(b => b.Session_Id == _session_id);

        // If table_id is specified, validate it belongs to the session
        if (_table_id.HasValue)
        {
          bool tableInSession = false;

          // Check if it's the directly assigned table
          if (session.Table_Id == _table_id.Value)
          {
            tableInSession = true;
          }
          // Check if it's in the table group
          else if (session.TableGroup_Id.HasValue &&
                   session.TableGroup.Tables.Any(t => t.Table_Id == _table_id.Value))
          {
            tableInSession = true;
          }

          if (!tableInSession)
          {
            return BadRequest(new { message = $"Table with ID {_table_id.Value} is not associated with session {_session_id}" });
          }
        }

        var bills = billQuery.OrderByDescending(b => b.Created_At).ToList();

        if (bills.Count == 0)
        {
          return NotFound(new { message = "No bills found for the specified session" });
        }

        var billResponses = bills.Select(bill => new BillResponse
        {
          Bill_Id = bill.Bill_Id,
          Session_Id = bill.Session_Id,
          Bill_Name = bill.Bill_Name ?? string.Empty,
          Senior_Count = bill.Senior_Count,
          Adult_Count = bill.Adult_Count,
          Child_Count = bill.Child_Count,
          Total_Count = bill.Total_Count,
          Status = bill.Status.ToString(),
          Created_At = bill.Created_At,
          Closed_At = bill.Closed_At.ToString(),
          Table_Numbers = GetTableNumbers(session)
        }).ToList();

        return Ok(billResponses);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error in Get Bill: {ex}");
        return StatusCode(500, "There was a Problem in the Get Bill Method");
      }
    }

    [HttpGet("get_bill/{_bill_id}")]
    public async Task<IActionResult> Get_Bill(int _session_id, int _bill_id)
    {
      try
      {
        var bill = await _context.Bills
            .Include(b => b.DiningSession)
                .ThenInclude(ds => ds.Table)
            .Include(b => b.DiningSession)
                .ThenInclude(ds => ds.TableGroup)
                    .ThenInclude(tg => tg.Tables)
            .FirstOrDefaultAsync(b => b.Bill_Id == _bill_id && b.Session_Id == _session_id);

        if (bill == null)
        {
          return NotFound(new { message = "Bill or session was not found" });
        }

        return Ok(new BillResponse
        {
          Bill_Id = bill.Bill_Id,
          Session_Id = bill.Session_Id,
          Bill_Name = bill.Bill_Name ?? string.Empty,
          Senior_Count = bill.Senior_Count,
          Adult_Count = bill.Adult_Count,
          Child_Count = bill.Child_Count,
          Total_Count = bill.Total_Count,
          Status = bill.Status.ToString(),
          Created_At = bill.Created_At,
          Closed_At = bill.Closed_At.ToString(),
          Table_Numbers = GetTableNumbers(bill.DiningSession)
        });
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error in Get Bill: {ex}");
        return StatusCode(500, "There was a Problem in the Get Bill Method");
      }
    }

    [HttpPut("close_bill/{_bill_id}")]
    public async Task<IActionResult> Close_Bill(int _session_id, int _bill_id)
    {
      try
      {
        var bill = await _context.Bills
            .Include(b => b.DiningSession)
                .ThenInclude(ds => ds.Table)
            .Include(b => b.DiningSession)
                .ThenInclude(ds => ds.TableGroup)
                    .ThenInclude(tg => tg.Tables)
            .FirstOrDefaultAsync(b => b.Bill_Id == _bill_id && b.Session_Id == _session_id);

        if (bill == null)
        {
          return NotFound(new { message = "Bill or session was not found" });
        }

        if (bill.Status == BillStatus.Closed)
        {
          return BadRequest(new { message = "Bill is already closed" });
        }

        var pendingOrders = await _context.SessionOrders
            .Where(o => o.Bill_Id == _bill_id && o.Status == OrderStatus.Pending)
            .ToListAsync();

        if (pendingOrders.Any())
        {
          return BadRequest(new { message = "Cannot close bill with pending orders" });
        }

        bill.Status = BillStatus.Closed;
        bill.Closed_At = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        await _context.Entry(bill).ReloadAsync();

        return Ok(new BillResponse
        {
          Bill_Id = bill.Bill_Id,
          Session_Id = bill.Session_Id,
          Bill_Name = bill.Bill_Name ?? string.Empty,
          Senior_Count = bill.Senior_Count,
          Adult_Count = bill.Adult_Count,
          Child_Count = bill.Child_Count,
          Total_Count = bill.Total_Count,
          Status = bill.Status.ToString(),
          Created_At = bill.Created_At,
          Closed_At = bill.Closed_At.ToString(),
          Table_Numbers = GetTableNumbers(bill.DiningSession)
        });
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error in Close Bill: {ex}");
        return StatusCode(500, "There was a Problem in the Close Bill Method");
      }
    }

    [HttpPost("cancel_bill/{_bill_id}")]
    public async Task<IActionResult> Cancel_Bill(int _session_id, int _bill_id)
    {
      try
      {
        var bill = await _context.Bills
            .Include(b => b.DiningSession)
                .ThenInclude(ds => ds.Table)
            .Include(b => b.DiningSession)
                .ThenInclude(ds => ds.TableGroup)
                    .ThenInclude(tg => tg.Tables)
            .FirstOrDefaultAsync(b => b.Bill_Id == _bill_id && b.Session_Id == _session_id);

        if (bill == null)
        {
          return NotFound(new { message = "Bill or session was not found" });
        }

        if (bill.Status != BillStatus.Open)
        {
          return BadRequest(new { message = $"Cannot cancel bill with status {bill.Status}" });
        }

        var pendingOrders = await _context.SessionOrders
            .Where(o => o.Bill_Id == _bill_id && (o.Status == OrderStatus.Pending))
            .ToListAsync();

        if (pendingOrders.Any())
        {
          for (int i = 0; i < pendingOrders.Count; i++)
          {
            var order = pendingOrders[i];
            order.Status = OrderStatus.Cancelled;
          }
        }

        bill.Status = BillStatus.Cancelled;
        bill.Closed_At = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        await _context.Entry(bill).ReloadAsync();

        return Ok(new BillResponse
        {
          Bill_Id = bill.Bill_Id,
          Session_Id = bill.Session_Id,
          Bill_Name = bill.Bill_Name ?? string.Empty,
          Senior_Count = bill.Senior_Count,
          Adult_Count = bill.Adult_Count,
          Child_Count = bill.Child_Count,
          Total_Count = bill.Total_Count,
          Status = bill.Status.ToString(),
          Created_At = bill.Created_At,
          Closed_At = bill.Closed_At.ToString(),
          Table_Numbers = GetTableNumbers(bill.DiningSession)
        });
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error in Cancel Bill: {ex}");
        return StatusCode(500, "There was a Problem in the Cancel Bill Method");
      }
    }

    [HttpGet("active/bills")]
    public async Task<ActionResult<List<BillResponse>>> GetActiveBills()
    {
      try
      {
        // Retrieve current user ID from claims (assuming JWT authentication)
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == "User_Id");
        var userId = userIdClaim?.Value ?? "Unknown";

        // Get all bills for user's active session using joins
        var bills = await _context.Bills
            .Include(b => b.DiningSession)
                .ThenInclude(ds => ds.Table)
            .Include(b => b.DiningSession)
                .ThenInclude(ds => ds.TableGroup)
                    .ThenInclude(tg => tg.Tables)
            .Join(
                _context.SessionParticipants,
                bill => bill.Session_Id,
                participant => participant.Session_Id,
                (bill, participant) => new { bill, participant }
            )
            .Where(x =>
                x.participant.User_Id.ToString() == userId &&
                x.participant.Left_At == null &&  // User hasn't left
                x.bill.DiningSession.Ended_At == null        // Session is active
            )
            .Select(x => x.bill)
            .ToListAsync();

        if (!bills.Any())
        {
          return NotFound(new { detail = "No bills found for active session" });
        }

        // Convert bills to response model
        _logger.LogInformation($"Found {bills.Count} bills for user {userId}'s active session");

        var billResponses = bills.Select(bill => new BillResponse
        {
          Bill_Id = bill.Bill_Id,
          Session_Id = bill.Session_Id,
          Bill_Name = bill.Bill_Name ?? string.Empty,
          Senior_Count = bill.Senior_Count,
          Adult_Count = bill.Adult_Count,
          Child_Count = bill.Child_Count,
          Total_Count = bill.Total_Count,
          Status = bill.Status.ToString(),
          Created_At = bill.Created_At,
          Closed_At = bill.Closed_At.ToString(),
          Table_Numbers = GetTableNumbers(bill.DiningSession)
        }).ToList();

        return Ok(billResponses);
      }
      catch (Exception ex)
      {
        _logger.LogError($"Database error retrieving bills: {ex.Message}");
        return StatusCode(500, new { detail = "Error retrieving bills" });
      }
    }
  }
}