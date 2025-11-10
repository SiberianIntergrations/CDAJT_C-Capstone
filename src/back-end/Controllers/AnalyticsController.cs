using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using Microsoft.AspNetCore.Mvc;
using back_end.DTO.Analytics;
using Microsoft.AspNetCore.Http.Features;
using back_end.Helpers;


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

        /// <summary>
        /// Retrieves performance metrics for all menu items showing total units sold.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing performance data with date range and item statistics.
        /// Returns HTTP 200 (OK) with item performance metrics on success.
        /// Returns HTTP 404 (Not Found) if no performance data is available.
        /// Returns HTTP 500 (Internal Server Error) if an exception occurs during retrieval.
        /// </returns>
        /// <response code="200">Returns item performance metrics with date range and sales data</response>
        /// <response code="404">If no item performance data is found</response>
        /// <response code="500">If an internal error occurs while retrieving metrics</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/analytics/item-performance
        ///
        /// Returns performance data including:
        /// - Date range (earliest and latest order completion dates)
        /// - For each menu item:
        ///   - Item ID and name
        ///   - Total units sold across all orders
        /// 
        /// Items with zero sales are included in the results.
        /// The date range represents the period covered by completed orders.
        /// </remarks>
        [HttpGet("item-performance")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

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

        /// <summary>
        /// Retrieves browsing behavior metrics for menu items showing total views and view duration.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a collection of browsing behavior data objects.
        /// Returns HTTP 200 (OK) with browsing metrics on success.
        /// Returns HTTP 404 (Not Found) if no browsing data is available.
        /// Returns HTTP 500 (Internal Server Error) if an exception occurs during retrieval.
        /// </returns>
        /// <response code="200">Returns browsing behavior metrics for all menu items</response>
        /// <response code="404">If no browsing behavior data is found</response>
        /// <response code="500">If an internal error occurs while retrieving metrics</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/analytics/browsing-behavior
        ///
        /// Returns aggregated metrics across all menu assignments showing:
        /// - Item ID and name
        /// - Total view duration in seconds
        /// - Total number of views across all menus
        /// 
        /// Data is aggregated from all MenuItemAssignments, combining statistics
        /// for the same item across different menus.
        /// </remarks>
        [HttpGet("browsing-behavior")]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                            assignment.Total_Views,
                            assignment.Menu_Id
                        })
                    .GroupBy(x => new { x.Item_Id, x.Name })
                    .Select(g => new
                    {
                        item_id = g.Key.Item_Id,
                        name = g.Key.Name,
                        total_view_seconds = g.Sum(x => x.Total_View_Seconds),
                        total_views = g.Sum(x => x.Total_Views) 
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

        /// <summary>
        /// Retrieves table turnover metrics showing average dining duration grouped by day and month.
        /// </summary>
        /// <param name="partySize">The party size to filter metrics by (default: 2)</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a <see cref="TableTurnOverResponseDTO"/> object with daily and monthly metrics.
        /// Returns HTTP 200 (OK) with turnover metrics on success.
        /// Returns HTTP 404 (Not Found) if no data is available for the specified party size.
        /// Returns HTTP 500 (Internal Server Error) if an exception occurs during retrieval.
        /// </returns>
        /// <response code="200">Returns table turnover metrics grouped by day and month</response>
        /// <response code="404">If no turnover data is found for the specified party size</response>
        /// <response code="500">If an internal error occurs while retrieving metrics</response>
        /// <remarks>
        /// Sample requests:
        ///
        ///     GET /api/analytics/table-turnover
        ///     (Returns metrics for party size of 2)
        ///     
        ///     GET /api/analytics/table-turnover?partySize=4
        ///     (Returns metrics for party size of 4)
        ///
        /// Calculates average dining duration (from bill creation to closure) in minutes.
        /// Party size is calculated as the sum of seniors, adults, and children on each bill.
        /// Only includes closed bills (bills with a Closed_At timestamp).
        /// 
        /// Returns two datasets:
        /// - Daily: Average duration per day (format: yyyy-MM-dd)
        /// - Monthly: Average duration per month (format: yyyy-MM)
        /// </remarks>
        [HttpGet("table-turnover")]
        [ProducesResponseType(typeof(TableTurnOverResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTableTurnoverMetrics([FromQuery] int partySize = 2)
        {
            try
            {
                // Filter out bills with null Closed_At because we need to group by Closed_At date/time
                var filteredBills = _context.Bills
                    .Where(b => b.Closed_At.HasValue);
                    // .Where(b => (b.Senior_Count + b.Adult_Count + b.Child_Count) == partySize);

                // Daily turnover query
                var dailyTurnover = await filteredBills
                    .GroupBy(b => new
                    {
                        Day = b.Closed_At!.Value.Date,
                        PartySize = b.Senior_Count + b.Adult_Count + b.Child_Count
                    })
                    .Select(g => new TurnoverMetricDTO
                    {
                        Period = g.Key.Day.ToString("yyyy-MM-dd"),
                        AverageDuration = (int)Math.Round(
                            g.Average(b => EF.Functions.DateDiffMinute(b.Created_At, b.Closed_At!.Value))),
                        PartySize = g.Key.PartySize
                    })
                    .ToListAsync();

                // Monthly turnover query
                var monthlyTurnover = await filteredBills
                    .GroupBy(b => new
                    {
                        Month = new DateTime(b.Closed_At!.Value.Year, b.Closed_At!.Value.Month, 1),
                        PartySize = b.Senior_Count + b.Adult_Count + b.Child_Count
                    })
                    .Select(g => new TurnoverMetricDTO
                    {
                        Period = g.Key.Month.ToString("yyyy-MM"),
                        AverageDuration = (int)Math.Round(
                            g.Average(b => EF.Functions.DateDiffMinute(b.Created_At, b.Closed_At!.Value))),
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



        [HttpGet("order-timing/{location_id}")]
        [ProducesResponseType(typeof(TableTurnOverResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOrderTimingMetrics(
            string location_id
        )
        {
            try
            {
                // Use ClaimsHelpers for user identification
                string userOid = ClaimsHelpers.GetUserOid(User);

                // Get user's location from claims (if available)
                // string? locationIdStr = User.FindFirst("location_id")?.Value;
                string locationIdStr = location_id;
                int? locationId = null;
                if (int.TryParse(locationIdStr, out var locId))
                    locationId = locId;

                if (locationId == null)
                {
                    return BadRequest("Cannot determine user location from claims.");
                }

                var orderTiming = await _context.DiningSessions
                    .Where(ds => ds.Location_Id == locationId)
                    .Select(ds => new
                    {
                        sessionId = ds.Session_Id,
                        TimeToFirstOrderSeconds = EF.Functions.DateDiffSecond(ds.Started_At, ds.First_Order_At)
                    }).Take(25)
                    .ToListAsync();

                var dailyAverageTiming = await _context.DiningSessions
                    .GroupBy(ds => ds.Started_At.Date)
                    .Select(g => new
                    {
                        Day = g.Key,
                        AverageTimeToFirstOrderSeconds = g.Average(ds => EF.Functions.DateDiffSecond(ds.Started_At, ds.First_Order_At))
                    })
                    .ToListAsync();

                return Ok(new { orderTiming, dailyAverageTiming });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving order timing metrics: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }

        [HttpGet("max-party-size")]
        public async Task<IActionResult> GetMaxPartySize()
        {
            var maxPartySize = _context.Bills.Max(b => b.Child_Count + b.Adult_Count + b.Senior_Count);
            return Ok(maxPartySize);
        }
    }
}
