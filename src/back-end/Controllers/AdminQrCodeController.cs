using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using back_end.Services;

namespace back_end.Controllers
{
    /// <summary>
    /// Admin/Staff QR Code endpoints for WiFi and session (menu redirect).
    /// </summary>
    [ApiController]
    [Route("api/admin/qr")]
    [Authorize(Roles = "Admin,Staff")]
    public class AdminQrCodeController : ControllerBase
    {
        private readonly QrGeneratorService _qrService;
        private readonly ILogger<AdminQrCodeController> _logger;

        public AdminQrCodeController(QrGeneratorService qrService, ILogger<AdminQrCodeController> logger)
        {
            _qrService = qrService;
            _logger = logger;
        }

        [HttpGet("wifi")]
        [SwaggerOperation(
            Summary = "Get WiFi QR code",
            Description = "Generate or retrieve cached WiFi QR code for a specific location and table."
        )]
        [Produces("image/png")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWifiQr(
            [FromQuery, SwaggerParameter("Location ID", Required = true)] int locationId,
            [FromQuery, SwaggerParameter("Table number", Required = true)] int table,
            CancellationToken ct)
        {
            if (locationId <= 0 || table <= 0)
                return BadRequest("locationId and table must be positive integers");

            // Try cache path first
            var existing = _qrService.GetExistingQrCodePath(locationId, table, "wifi");
            if (existing is not null && System.IO.File.Exists(existing))
            {
                _logger.LogInformation("Returning cached WiFi QR for L{loc} T{table}", locationId, table);
                var cached = await System.IO.File.ReadAllBytesAsync(existing, ct);
                Response.Headers.CacheControl = "public, max-age=86400";
                return File(cached, "image/png", $"wifi_L{locationId}_T{table}.png");
            }

            var bytes = await _qrService.GetWifiQrBytesAsync(locationId, table, useCache: false, ct);
            if (bytes is null)
                return NotFound($"Location {locationId} not found or WiFi credentials not configured");

            Response.Headers.CacheControl = "public, max-age=86400";
            _logger.LogInformation("Generated WiFi QR for L{loc} T{table}", locationId, table);
            return File(bytes, "image/png", $"wifi_L{locationId}_T{table}.png");
        }

        [HttpGet("session")]
        [SwaggerOperation(
            Summary = "Get session QR code",
            Description = "Generate or retrieve cached session QR code for a specific location and table. Starts dining session and redirects to menu."
        )]
        [Produces("image/png")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSessionQr(
            [FromQuery, SwaggerParameter("Location ID", Required = true)] int locationId,
            [FromQuery, SwaggerParameter("Table number", Required = true)] int table,
            CancellationToken ct)
        {
            if (locationId <= 0 || table <= 0)
                return BadRequest("locationId and table must be positive integers");

            var existing = _qrService.GetExistingQrCodePath(locationId, table, "session");
            if (existing is not null && System.IO.File.Exists(existing))
            {
                _logger.LogInformation("Returning cached Session QR for L{loc} T{table}", locationId, table);
                var cached = await System.IO.File.ReadAllBytesAsync(existing, ct);
                Response.Headers.CacheControl = "public, max-age=86400";
                return File(cached, "image/png", $"session_L{locationId}_T{table}.png");
            }

            var bytes = await _qrService.GetSessionQrBytesAsync(locationId, table, useCache: false, ct);
            if (bytes is null)
                return NotFound($"Location {locationId} not found");

            Response.Headers.CacheControl = "public, max-age=86400";
            _logger.LogInformation("Generated Session QR for L{loc} T{table}", locationId, table);
            return File(bytes, "image/png", $"session_L{locationId}_T{table}.png");
        }

        [HttpPost("bulk")]
        [SwaggerOperation(
            Summary = "Bulk generate QR codes",
            Description = "Generate WiFi and Session QR codes for all tables at a location. Saves to storage/qrcodes/."
        )]
        public async Task<IActionResult> BulkGenerate(
            [FromQuery, SwaggerParameter("Location ID", Required = true)] int locationId,
            [FromQuery, SwaggerParameter("Number of tables", Required = true)] int tableCount,
            CancellationToken ct)
        {
            if (locationId <= 0 || tableCount <= 0 || tableCount > 1000)
                return BadRequest("locationId must be positive and tableCount must be between 1 and 1000");

            var name = await _qrService.GetLocationNameAsync(locationId, ct);
            if (name is null) return NotFound($"Location {locationId} not found");

            var files = new List<string>(tableCount * 2);
            for (int t = 1; t <= tableCount; t++)
            {
                ct.ThrowIfCancellationRequested();

                var wifi = await _qrService.GenerateAndSaveWifiQr(locationId, t, ct);
                if (wifi is not null) files.Add(Path.GetFileName(wifi));

                var sess = await _qrService.GenerateAndSaveSessionQr(locationId, t, ct);
                if (sess is not null) files.Add(Path.GetFileName(sess));
            }

            _logger.LogInformation("Bulk generated {count} PNGs for L{loc}", files.Count, locationId);
            return Ok(new
            {
                success = true,
                locationId,
                tableCount,
                filesGenerated = files.Count,
                storageLocation = "storage/qrcodes/",
                files
            });
        }

        [HttpDelete("clear")]
        [SwaggerOperation(
            Summary = "Clear cached QR codes",
            Description = "Delete cached WiFi and Session QR codes for a specific table."
        )]
        public IActionResult Clear(
            [FromQuery, SwaggerParameter("Location ID", Required = true)] int locationId,
            [FromQuery, SwaggerParameter("Table number", Required = true)] int table)
        {
            _qrService.DeleteQrCode(locationId, table, "wifi");
            _qrService.DeleteQrCode(locationId, table, "session");
            _logger.LogInformation("Cleared cached QRs for L{loc} T{table}", locationId, table);
            return Ok(new { success = true, message = $"Cleared QR codes for Location {locationId}, Table {table}" });
        }
    }
}