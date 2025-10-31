using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using back_end.Services;
using Swashbuckle.AspNetCore.Annotations;
using QRCoder;
using SkiaSharp;

namespace back_end.Controllers
{
    /// <summary>
    /// Admin-only QR Code generation endpoints for WiFi and session (menu redirect)
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
                    return BadRequest("locationId and table must be positive integers");

                var existingPath = _qrService.GetExistingQrCodePath(locationId, table, "wifi");
                if (existingPath != null && System.IO.File.Exists(existingPath))
                {
                    _logger.LogInformation("Returning cached WiFi QR for Location {loc}, Table {table}", locationId, table);
                    var cachedBytes = await System.IO.File.ReadAllBytesAsync(existingPath);
                    return File(cachedBytes, "image/png", $"wifi_L{locationId}_T{table}.png");
                }

                var credentials = await _qrService.GetLocationWifiCredentials(locationId);
                if (credentials == null)
                    return NotFound($"Location {locationId} not found or WiFi credentials not configured");

                var (ssid, password) = credentials.Value;

                // Load location name (entity uses Locations class with Name property)
                var location = await _qrService._context.Locations.FindAsync(locationId);
                var locationName = location?.Name ?? $"Location {locationId}";

                // 1) Create QR as PNG bytes (cross-platform, no System.Drawing)
                using var gen = new QRCodeGenerator();
                using var data = gen.CreateQrCode(
                    new PayloadGenerator.WiFi(ssid, password, PayloadGenerator.WiFi.Authentication.WPA, false).ToString(),
                    QRCodeGenerator.ECCLevel.Q);
                var pngQr = new PngByteQRCode(data);
                byte[] qrPngBytes = pngQr.GetGraphic(pixelsPerModule: 20);

                // 2) Add label with SkiaSharp
                var labeledBytes = AddLabelWithSkiaBelow(qrPngBytes, $"{locationName} — Table {table} — WiFi");

                // Save asynchronously for cache
                _ = Task.Run(async () =>
                {
                    try { await _qrService.GenerateAndSaveWifiQr(locationId, table); }
                    catch (Exception ex) { _logger.LogError(ex, "Error saving WiFi QR to storage"); }
                });

                _logger.LogInformation("Generated WiFi QR for Location {loc}, Table {table}", locationId, table);
                return File(labeledBytes, "image/png", $"wifi_L{locationId}_T{table}.png");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating WiFi QR code");
                return StatusCode(500, "Error generating QR code");
            }
        }

        [HttpGet("session")]
        [SwaggerOperation(
            Summary = "Get session QR code",
            Description = "Generate or retrieve cached session QR code for a specific location and table. Starts dining session and redirects to menu. Requires Admin or Staff role."
        )]
        [SwaggerResponse(200, "QR code image (PNG)", typeof(FileResult))]
        [SwaggerResponse(404, "Location not found")]
        [SwaggerResponse(401, "Unauthorized - requires authentication")]
        [SwaggerResponse(403, "Forbidden - requires Admin or Staff role")]
        [Produces("image/png")]
        public async Task<IActionResult> GetSessionQr(
            [FromQuery, SwaggerParameter("Location ID", Required = true)] int locationId,
            [FromQuery, SwaggerParameter("Table number", Required = true)] int table)
        {
            try
            {
                if (locationId <= 0 || table <= 0)
                    return BadRequest("locationId and table must be positive integers");

                var existingPath = _qrService.GetExistingQrCodePath(locationId, table, "session");
                if (existingPath != null && System.IO.File.Exists(existingPath))
                {
                    _logger.LogInformation("Returning cached Session QR for Location {loc}, Table {table}", locationId, table);
                    var cachedBytes = await System.IO.File.ReadAllBytesAsync(existingPath);
                    return File(cachedBytes, "image/png", $"session_L{locationId}_T{table}.png");
                }

                var location = await _qrService._context.Locations.FindAsync(locationId);
                if (location == null)
                    return NotFound($"Location {locationId} not found");

                var locationName = location.Name ?? $"Location {locationId}";
                var sessionUrl = _qrService.GetSessionUrl(locationId, table);

                using var gen = new QRCodeGenerator();
                using var data = gen.CreateQrCode(sessionUrl, QRCodeGenerator.ECCLevel.Q);
                var pngQr = new PngByteQRCode(data);
                byte[] qrPngBytes = pngQr.GetGraphic(pixelsPerModule: 20);

                var labeledBytes = AddLabelWithSkiaBelow(qrPngBytes, $"{locationName} — Table {table} — Menu");

                _ = Task.Run(async () =>
                {
                    try { await _qrService.GenerateAndSaveSessionQr(locationId, table); }
                    catch (Exception ex) { _logger.LogError(ex, "Error saving Session QR to storage"); }
                });

                _logger.LogInformation("Generated Session QR for Location {loc}, Table {table}", locationId, table);
                return File(labeledBytes, "image/png", $"session_L{locationId}_T{table}.png");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating session QR code");
                return StatusCode(500, "Error generating QR code");
            }
        }

        [HttpPost("bulk")]
        [SwaggerOperation(
            Summary = "Bulk generate QR codes",
            Description = "Generate WiFi and Session QR codes for all tables at a location. Saves to storage/qrcodes/. Requires Admin or Staff role."
        )]
        public async Task<IActionResult> BulkGenerateQrCodes(
            [FromQuery, SwaggerParameter("Location ID", Required = true)] int locationId,
            [FromQuery, SwaggerParameter("Number of tables", Required = true)] int tableCount)
        {
            try
            {
                if (locationId <= 0 || tableCount <= 0 || tableCount > 1000)
                    return BadRequest("locationId must be positive and tableCount must be between 1 and 1000");

                var location = await _qrService._context.Locations.FindAsync(locationId);
                if (location == null)
                    return NotFound($"Location {locationId} not found");

                var results = new List<string>();
                for (int t = 1; t <= tableCount; t++)
                {
                    var wifiPath = await _qrService.GenerateAndSaveWifiQr(locationId, t);
                    if (wifiPath != null) results.Add($"wifi_L{locationId}_T{t}.png");
                    var sessionPath = await _qrService.GenerateAndSaveSessionQr(locationId, t);
                    if (sessionPath != null) results.Add($"session_L{locationId}_T{t}.png");
                }

                _logger.LogInformation("Bulk generated {count} QR codes for Location {loc}", results.Count, locationId);
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

        [HttpDelete("clear")]
        [SwaggerOperation(
            Summary = "Clear cached QR codes",
            Description = "Delete cached WiFi and Session QR codes for a specific table. Requires Admin or Staff role."
        )]
        public IActionResult ClearQrCache(
            [FromQuery, SwaggerParameter("Location ID", Required = true)] int locationId,
            [FromQuery, SwaggerParameter("Table number", Required = true)] int table)
        {
            try
            {
                _qrService.DeleteQrCode(locationId, table, "wifi");
                _qrService.DeleteQrCode(locationId, table, "session");
                _logger.LogInformation("Cleared QR cache for Location {loc}, Table {table}", locationId, table);
                return Ok(new { success = true, message = $"Cleared QR codes for Location {locationId}, Table {table}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing QR cache");
                return StatusCode(500, "Error clearing QR cache");
            }
        }

        // ---------- SkiaSharp helper: label BELOW the QR (single line) ----------
        private static byte[] AddLabelWithSkiaBelow(byte[] qrPng, string label)
        {
            using var qrBitmap = SKBitmap.Decode(qrPng);
            int qrW = qrBitmap.Width;
            int qrH = qrBitmap.Height;

            int labelHeight = 60;
            var info = new SKImageInfo(qrW, qrH + labelHeight);
            using var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;

            // white background
            canvas.Clear(SKColors.White);

            // draw QR
            canvas.DrawBitmap(qrBitmap, new SKPoint(0, 0));

            // label
            using var paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                TextSize = 24,
                TextAlign = SKTextAlign.Center,
                Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold) ?? SKTypeface.Default
            };

            float textX = qrW / 2f;
            float textY = qrH + (labelHeight / 2f) + (paint.TextSize / 3f);
            canvas.DrawText(label, textX, textY, paint);

            using var img = surface.Snapshot();
            using var data = img.Encode(SKEncodedImageFormat.Png, 100);
            return data.ToArray();
        }
    }
}