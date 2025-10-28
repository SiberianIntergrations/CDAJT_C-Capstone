using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using back_end.Services.QrCode;
using Swashbuckle.AspNetCore.Annotations;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

namespace back_end.Controllers
{
    /// <summary>
    /// Admin-only QR Code generation endpoints for WiFi and authentication
    /// </summary>
    [ApiController]
    [Route("api/admin/qr")]
    [Authorize(Roles = "Admin,Staff")]
    [SupportedOSPlatform("windows")]
    public class AdminQrCodeController : ControllerBase
    {
        private readonly QrGeneratorService _qrService;
        private readonly ILogger<AdminQrCodeController> _logger;

        public AdminQrCodeController(QrGeneratorService qrService, ILogger<AdminQrCodeController> logger)
        {
            _qrService = qrService;
            _logger = logger;
        }

        /// <summary>
        /// Get WiFi QR code for a specific location and table
        /// </summary>
        /// <param name="locationId">Location ID</param>
        /// <param name="table">Table number</param>
        /// <returns>PNG image of WiFi QR code</returns>
        [HttpGet("wifi")]
        [SwaggerOperation(
            Summary = "Get WiFi QR code",
            Description = "Generate or retrieve cached WiFi QR code for a specific location and table. Requires Admin or Staff role."
        )]
        [SwaggerResponse(200, "QR code image (PNG)", typeof(FileResult))]
        [SwaggerResponse(404, "Location not found or WiFi credentials not configured")]
        [SwaggerResponse(401, "Unauthorized - requires authentication")]
        [SwaggerResponse(403, "Forbidden - requires Admin or Staff role")]
        [Produces("image/png")]
        public async Task<IActionResult> GetWifiQr(
            [FromQuery, SwaggerParameter("Location ID", Required = true)] int locationId,
            [FromQuery, SwaggerParameter("Table number", Required = true)] int table)
        {
            try
            {
                if (locationId <= 0 || table <= 0)
                {
                    return BadRequest("locationId and table must be positive integers");
                }

                // Check if QR code already exists in cache
                var existingPath = _qrService.GetExistingQrCodePath(locationId, table, "wifi");
                if (existingPath != null && System.IO.File.Exists(existingPath))
                {
                    _logger.LogInformation($"Returning cached WiFi QR for Location {locationId}, Table {table}");
                    var cachedBytes = await System.IO.File.ReadAllBytesAsync(existingPath);
                    return File(cachedBytes, "image/png", $"wifi_L{locationId}_T{table}.png");
                }

                // Get WiFi credentials by locationId
                var credentials = await _qrService.GetLocationWifiCredentials(locationId);
                if (credentials == null)
                {
                    return NotFound($"Location {locationId} not found or WiFi credentials not configured");
                }

                var (ssid, password) = credentials.Value;

                // Get location name for label
                var location = await _qrService._context.Locations.FindAsync(locationId);
                var locationName = location?.Name ?? $"Location {locationId}";

                // Generate QR code with label
                using var bmp = _qrService.CreateWifiQr(ssid, password, hidden: false);
                using var labeled = _qrService.RenderLabeledQr(bmp, $"{locationName} — Table {table} — WiFi");
                using var ms = new MemoryStream();

                labeled.Save(ms, ImageFormat.Png);
                ms.Position = 0;

                var bytes = ms.ToArray();

                // Save to storage for caching
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _qrService.GenerateAndSaveWifiQr(locationId, table);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error saving WiFi QR code to storage");
                    }
                });

                _logger.LogInformation($"Generated WiFi QR for Location {locationId}, Table {table}");
                return File(bytes, "image/png", $"wifi_L{locationId}_T{table}.png");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating WiFi QR code");
                return StatusCode(500, "Error generating QR code");
            }
        }

        /// <summary>
        /// Get authentication page QR code for a specific location and table
        /// </summary>
        /// <param name="locationId">Location ID</param>
        /// <param name="table">Table number</param>
        /// <returns>PNG image of authentication QR code</returns>
        [HttpGet("auth")]
        [SwaggerOperation(
            Summary = "Get authentication QR code",
            Description = "Generate or retrieve cached authentication page QR code for a specific location and table. Requires Admin or Staff role."
        )]
        [SwaggerResponse(200, "QR code image (PNG)", typeof(FileResult))]
        [SwaggerResponse(404, "Location not found")]
        [SwaggerResponse(401, "Unauthorized - requires authentication")]
        [SwaggerResponse(403, "Forbidden - requires Admin or Staff role")]
        [Produces("image/png")]
        public async Task<IActionResult> GetAuthQr(
            [FromQuery, SwaggerParameter("Location ID", Required = true)] int locationId,
            [FromQuery, SwaggerParameter("Table number", Required = true)] int table)
        {
            try
            {
                if (locationId <= 0 || table <= 0)
                {
                    return BadRequest("locationId and table must be positive integers");
                }

                // Check if QR code already exists in cache
                var existingPath = _qrService.GetExistingQrCodePath(locationId, table, "auth");
                if (existingPath != null && System.IO.File.Exists(existingPath))
                {
                    _logger.LogInformation($"Returning cached Auth QR for Location {locationId}, Table {table}");
                    var cachedBytes = await System.IO.File.ReadAllBytesAsync(existingPath);
                    return File(cachedBytes, "image/png", $"auth_L{locationId}_T{table}.png");
                }

                // Get location for validation and name
                var location = await _qrService._context.Locations.FindAsync(locationId);
                if (location == null)
                {
                    return NotFound($"Location {locationId} not found");
                }

                var locationName = location.Name ?? $"Location {locationId}";
                var authUrl = _qrService.GetAuthUrl(locationId, table);

                // Generate QR code with label
                using var bmp = _qrService.CreateAuthQr(authUrl);
                using var labeled = _qrService.RenderLabeledQr(bmp, $"{locationName} — Table {table} — Login");
                using var ms = new MemoryStream();

                labeled.Save(ms, ImageFormat.Png);
                ms.Position = 0;

                var bytes = ms.ToArray();

                // Save to storage for caching
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _qrService.GenerateAndSaveAuthQr(locationId, table);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error saving Auth QR code to storage");
                    }
                });

                _logger.LogInformation($"Generated Auth QR for Location {locationId}, Table {table}");
                return File(bytes, "image/png", $"auth_L{locationId}_T{table}.png");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating authentication QR code");
                return StatusCode(500, "Error generating QR code");
            }
        }

        /// <summary>
        /// Generate QR codes for all tables at a location
        /// </summary>
        /// <param name="locationId">Location ID</param>
        /// <param name="tableCount">Number of tables to generate QR codes for</param>
        /// <returns>Status of bulk generation</returns>
        [HttpPost("bulk")]
        [SwaggerOperation(
            Summary = "Bulk generate QR codes",
            Description = "Generate WiFi and Auth QR codes for all tables at a location. Saves to storage/qrcodes/. Requires Admin or Staff role."
        )]
        [SwaggerResponse(200, "QR codes generated successfully")]
        [SwaggerResponse(404, "Location not found")]
        [SwaggerResponse(401, "Unauthorized - requires authentication")]
        [SwaggerResponse(403, "Forbidden - requires Admin or Staff role")]
        public async Task<IActionResult> BulkGenerateQrCodes(
            [FromQuery, SwaggerParameter("Location ID", Required = true)] int locationId,
            [FromQuery, SwaggerParameter("Number of tables", Required = true)] int tableCount)
        {
            try
            {
                if (locationId <= 0 || tableCount <= 0 || tableCount > 1000)
                {
                    return BadRequest("locationId must be positive and tableCount must be between 1 and 1000");
                }

                var location = await _qrService._context.Locations.FindAsync(locationId);
                if (location == null)
                {
                    return NotFound($"Location {locationId} not found");
                }

                var results = new List<string>();

                for (int table = 1; table <= tableCount; table++)
                {
                    // Generate WiFi QR
                    var wifiPath = await _qrService.GenerateAndSaveWifiQr(locationId, table);
                    if (wifiPath != null)
                    {
                        results.Add($"wifi_L{locationId}_T{table}.png");
                    }

                    // Generate Auth QR
                    var authPath = await _qrService.GenerateAndSaveAuthQr(locationId, table);
                    if (authPath != null)
                    {
                        results.Add($"auth_L{locationId}_T{table}.png");
                    }
                }

                _logger.LogInformation($"Bulk generated {results.Count} QR codes for Location {locationId}");

                return Ok(new
                {
                    success = true,
                    message = $"Generated {results.Count} QR codes for {tableCount} tables",
                    locationId,
                    tableCount,
                    filesGenerated = results.Count,
                    storageLocation = "storage/qrcodes/"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk QR code generation");
                return StatusCode(500, "Error generating QR codes");
            }
        }

        /// <summary>
        /// Delete cached QR codes for a table
        /// </summary>
        /// <param name="locationId">Location ID</param>
        /// <param name="table">Table number</param>
        /// <returns>Success status</returns>
        [HttpDelete("clear")]
        [SwaggerOperation(
            Summary = "Clear cached QR codes",
            Description = "Delete cached WiFi and Auth QR codes for a specific table. Requires Admin or Staff role."
        )]
        [SwaggerResponse(200, "QR codes deleted successfully")]
        [SwaggerResponse(401, "Unauthorized - requires authentication")]
        [SwaggerResponse(403, "Forbidden - requires Admin or Staff role")]
        public IActionResult ClearQrCache(
            [FromQuery, SwaggerParameter("Location ID", Required = true)] int locationId,
            [FromQuery, SwaggerParameter("Table number", Required = true)] int table)
        {
            try
            {
                _qrService.DeleteQrCode(locationId, table, "wifi");
                _qrService.DeleteQrCode(locationId, table, "auth");

                _logger.LogInformation($"Cleared QR cache for Location {locationId}, Table {table}");

                return Ok(new
                {
                    success = true,
                    message = $"Cleared QR codes for Location {locationId}, Table {table}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing QR cache");
                return StatusCode(500, "Error clearing QR cache");
            }
        }
    }
}
