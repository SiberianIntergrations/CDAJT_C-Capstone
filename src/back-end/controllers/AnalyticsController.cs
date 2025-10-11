
using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.DTO.Auth;
using Microsoft.AspNetCore.Mvc;
using back_end.DTO.Analytics;
using Microsoft.AspNetCore.Http.Features;


namespace back_end.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(ApplicationDbContext context, IConfiguration config, ILogger<AnalyticsController> logger)
        {
            _context = context;
            _config = config;
            _logger = logger;
        }

        [HttpGet("item-performance")]
        public async Task<IActionResult> GetItemPerformance()
        {
            try
            {
                var item_performance = await _context.MenuItems.GroupJoin(
                    _context.OrderItems,
                    menu => menu.item_id,
                    order => order.Item_Id,
                    (menu, orders) => new
                    {
                        menu.item_id,
                        menu.Name,
                        TotalUnitsSold = orders.Sum(o => (int?)o.Quantity) ?? 0
                    })
                .ToListAsync();

                if (item_performance == null || item_performance.Count == 0)
                {
                    return NotFound(new { message = "No item performance data found." });
                }

                var date_period_max = await _context.OrderItems.MaxAsync(o => o.Completed_At);
                var date_period_min = await _context.OrderItems.MinAsync(o => o.Completed_At);


                return Ok(new { date_period_min, date_period_max, item_performance });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }

        [HttpGet("browsing-behavior")]
        public async Task<IActionResult> GetBrowsingBehavior()
        {
            try
            {

                var browsingData = await _context.MenuItemAssignments
                .Join(
                    _context.MenuItems,
                    assignment => assignment.Item_Id,
                    menuItem => menuItem.item_id,
                    (assignment, menuItem) => new
                    {
                        assignment.Item_Id,
                        menuItem.Name,
                        assignment.Total_View_Seconds,
                        assignment.Menu_Id
                    })
                .GroupBy(x => new { x.Item_Id, x.Name })
                .Select(g => new
                {
                    item_id = g.Key.Item_Id,
                    name = g.Key.Name,
                    total_view_seconds = g.Sum(x => x.Total_View_Seconds),
                    total_views = g.Sum(x => x.Menu_Id)
                })
                .ToListAsync();

                if (browsingData == null || browsingData.Count == 0)
                {
                    return NotFound(new { message = "No browsing behavior data found." });
                }

                return Ok(browsingData);


            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }


        public async Task<IActionResult> GetTableTurnoverMetrics(
            [FromQuery] int partySize = 2)
        {
            try
            {
                // Daily turnover query
                var dailyTurnover = await _context.Bills
                    .Where(b => b.Closed_At != null &&
                            (b.Senior_Count + b.Adult_Count + b.Child_Count) == partySize)
                    .GroupBy(b => new
                    {
                        Day = b.Closed_At.Date,
                        PartySize = b.Senior_Count + b.Adult_Count + b.Child_Count
                    })
                    .Select(g => new TurnoverMetricDTO
                    {
                        Period = g.Key.Day.ToString("yyyy-MM-dd"),
                        AverageDuration = (int)Math.Round(
                            g.Average(b => EF.Functions.DateDiffMinute(b.Created_At, b.Closed_At))),
                        PartySize = g.Key.PartySize
                    })
                    .ToListAsync();

                // Monthly turnover query
                var monthlyTurnover = await _context.Bills
                    .Where(b => b.Closed_At != null &&
                            (b.Senior_Count + b.Adult_Count + b.Child_Count) == partySize)
                    .GroupBy(b => new
                    {
                        Month = new DateTime(b.Closed_At.Year, b.Closed_At.Month, 1), // Added the day parameter
                        PartySize = b.Senior_Count + b.Adult_Count + b.Child_Count
                    })
                    .Select(g => new TurnoverMetricDTO
                    {
                        Period = g.Key.Month.ToString("yyyy-MM"),
                        AverageDuration = (int)Math.Round(
                            g.Average(b => EF.Functions.DateDiffMinute(b.Created_At, b.Closed_At))),
                        PartySize = g.Key.PartySize
                    })
                    .ToListAsync();

                if (!dailyTurnover.Any() && !monthlyTurnover.Any())
                {
                    _logger.LogWarning("No table turnover data found");
                    return NotFound(new { message = "No table turnover data found" });
                }

                _logger.LogInformation($"Retrieved turnover data: {dailyTurnover.Count} daily records, {monthlyTurnover.Count} monthly records");

                var response = new TableTurnOverResponseDTO
                {
                    Daily = dailyTurnover.Select(d => new TableTurnoverDailyDTO
                    {
                        Day = d.Period ?? string.Empty,
                        AverageDuration = d.AverageDuration,
                        Party_size = d.PartySize
                    }).ToList(),
                    Monthly = monthlyTurnover.Select(m => new TableTurnOverMonthlyDTO
                    {
                        Month = m.Period ?? string.Empty,
                        AverageDuration = m.AverageDuration,
                        Party_size = m.PartySize
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving table turnover metrics: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
    }

}