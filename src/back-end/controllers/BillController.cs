using System.IO.Compression;
using System.Numerics;
using System.Runtime.InteropServices;
using back_end.domain;
using back_end.domain.DbContexts;
using back_end.domain.Entities;
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
        private readonly ApplicationContext _context;
        private readonly IConfiguration _config;

        public BillController(ApplicationContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("create_Bill/{_session_id}")]

        public async Task<IActionResult> Create_Bill(int _session_id, CreateBill bill_data)
        {

            try
            {
                var session = await _context.DiningSessions.FirstOrDefaultAsync(ds => ds.Session_Id == _session_id);
                if (session == null)
                {
                    return NotFound(new { message = "Bill or session was not found" });
                }
                if (session.Ended_At == null)
                {
                    return BadRequest(new { message = $"Can not create bill for session that ended {session.Ended_At}" });
                }
                if (session.Tables == null)
                {
                    return BadRequest(new { message = $"can not create a bill that has no table" });
                }
                if (bill_data.Adult_Count <= 0 && bill_data.Senior_Count <= 0 && bill_data.Child_Count <= 0)
                {
                    return BadRequest(new { message = $"Bill Must have At least one Guest In the Session" });
                }
                var NewBill = new Billing
                {
                    Session_Id = _session_id,
                    Bill_Name = bill_data.Bill_Name,
                    Senior_Count = bill_data.Senior_Count,
                    Adult_Count = bill_data.Adult_Count,
                    Child_Count = bill_data.Child_Count,
                    Total_Count = bill_data.Tot_Count,
                    Status = BillStatus.Open
                };
                _context.Billings.Add(NewBill);
                await _context.SaveChangesAsync();
                await _context.Entry(NewBill).ReloadAsync();
                return Ok(new BillResponse
                {
                    Bill_Id = NewBill.Bill_Id,
                    Session_Id = NewBill.Session_Id,
                    Bill_Name = NewBill.Bill_Name ?? string.Empty,
                    Senior_Count = NewBill.Senior_Count,
                    Adult_Count = NewBill.Adult_Count,
                    Child_Count = NewBill.Child_Count,
                    Total_Count = NewBill.Total_Count,
                    Status = NewBill.Status.ToString(),
                    Created_At = NewBill.Created_At,
                    Closed_At = NewBill.Closed_At,
                    Table_Numbers = new List<int>()
                });
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error in Create Bill {ex}");
                return StatusCode(500, "There was a Problem in the Create Bill Method");
            }

        }

        [HttpGet("get_bill/{_session_id}")]
        public async Task<IActionResult> Get_Bill(int _session_id, int? _table_id = null )
        {
            try
            {
                var session = await _context.DiningSessions.FirstOrDefaultAsync(ds => ds.Session_Id == _session_id);
                if (session == null)
                {
                    return NotFound(new { message = "Session was not found" });
                }
                var billQuery = _context.Billings.Where(b => b.Session_Id == _session_id);
                if (_table_id.HasValue)
                {
                    if (!session.Tables.Any(t => t.Table_Id == _table_id.Value))
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
                    Closed_At = bill.Closed_At,
                    Table_Numbers = new List<int>()
                }).ToList();
                return Ok(billResponses);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Get Bill: {ex}");
                return StatusCode(500, "There was a Problem in the Get Bill Method");
            }
        }

    }

}