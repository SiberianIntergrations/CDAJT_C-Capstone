using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using back_end.domain.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Versioning;

namespace back_end.Services.QrCode
{
    /// <summary>
    /// Service for generating QR codes for WiFi and authentication with location-based configuration
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class QrGeneratorService
    {
        // Made internal for controller access - not ideal but simplifies implementation
        internal readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _storagePath;
        private readonly ILogger<QrGeneratorService> _logger;

        public QrGeneratorService(
            ApplicationDbContext context,
            IConfiguration configuration,
            ILogger<QrGeneratorService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;

            // Storage path for generated QR codes
            _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "storage", "qrcodes");

            // Create storage directory if it doesn't exist
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        /// <summary>
        /// Create a WiFi QR code bitmap
        /// </summary>
        /// <param name="ssid">WiFi network name</param>
        /// <param name="password">WiFi password</param>
        /// <param name="hidden">Whether the network is hidden</param>
        /// <returns>Bitmap containing the QR code</returns>
        public Bitmap CreateWifiQr(string ssid, string password, bool hidden = false)
        {
            // WiFi QR format: WIFI:T:WPA;S:ssid;P:password;H:true/false;;
            string encryptionType = "WPA"; // WPA/WPA2
            string hiddenFlag = hidden ? "true" : "false";
            string qrData = $"WIFI:T:{encryptionType};S:{ssid};P:{password};H:{hiddenFlag};;";

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    // Return a new bitmap that won't be disposed
                    return new Bitmap(qrCode.GetGraphic(20));
                }
            }
        }

        /// <summary>
        /// Create an authentication URL QR code bitmap
        /// </summary>
        /// <param name="authUrl">The authentication page URL</param>
        /// <returns>Bitmap containing the QR code</returns>
        public Bitmap CreateAuthQr(string authUrl)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(authUrl, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    // Return a new bitmap that won't be disposed
                    return new Bitmap(qrCode.GetGraphic(20));
                }
            }
        }

        /// <summary>
        /// Render a QR code with a text label
        /// </summary>
        /// <param name="qrBitmap">The QR code bitmap</param>
        /// <param name="labelText">Text to display below the QR code</param>
        /// <returns>New bitmap with QR code and label</returns>
        public Bitmap RenderLabeledQr(Bitmap qrBitmap, string labelText)
        {
            const int labelHeight = 60;
            const int padding = 10;

            // Create a new bitmap with extra height for the label
            Bitmap labeledImage = new Bitmap(
                qrBitmap.Width + (padding * 2),
                qrBitmap.Height + labelHeight + (padding * 2)
            );

            using (Graphics g = Graphics.FromImage(labeledImage))
            {
                // Fill background with white
                g.Clear(Color.White);

                // Draw the QR code
                g.DrawImage(qrBitmap, padding, padding);

                // Draw the label
                using (Font font = new Font("Arial", 12, FontStyle.Bold))
                using (SolidBrush brush = new SolidBrush(Color.Black))
                {
                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };

                    Rectangle labelRect = new Rectangle(
                        0,
                        qrBitmap.Height + padding,
                        labeledImage.Width,
                        labelHeight
                    );

                    g.DrawString(labelText, font, brush, labelRect, sf);
                }
            }

            return labeledImage;
        }

        /// <summary>
        /// Get WiFi credentials for a specific location
        /// </summary>
        /// <param name="locationId">Location ID</param>
        /// <returns>Tuple containing SSID and password, or null if location not found</returns>
        public async Task<(string ssid, string password)?> GetLocationWifiCredentials(int locationId)
        {
            var location = await _context.Locations.FindAsync(locationId);

            if (location == null)
            {
                _logger.LogWarning($"Location {locationId} not found");
                return null;
            }

            // Try to get location-specific WiFi settings from configuration
            // Format: QRCodeSettings:Locations:{locationId}:WiFi:SSID
            var locationSsid = _configuration[$"QRCodeSettings:Locations:{locationId}:WiFi:SSID"];
            var locationPassword = _configuration[$"QRCodeSettings:Locations:{locationId}:WiFi:Password"];

            if (!string.IsNullOrEmpty(locationSsid) && !string.IsNullOrEmpty(locationPassword))
            {
                return (locationSsid, locationPassword);
            }

            // Fallback to default WiFi credentials
            var defaultSsid = _configuration["QRCodeSettings:WiFi:SSID"];
            var defaultPassword = _configuration["QRCodeSettings:WiFi:Password"];

            if (string.IsNullOrEmpty(defaultSsid) || string.IsNullOrEmpty(defaultPassword))
            {
                _logger.LogError("No WiFi credentials configured");
                return null;
            }

            return (defaultSsid, defaultPassword);
        }

        /// <summary>
        /// Get the authentication URL for a specific location and table
        /// </summary>
        /// <param name="locationId">Location ID</param>
        /// <param name="tableNumber">Table number</param>
        /// <returns>Authentication URL</returns>
        public string GetAuthUrl(int locationId, int tableNumber)
        {
            var baseUrl = _configuration["QRCodeSettings:AuthPageUrl"]
                ?? _configuration["Restaurant:BaseUrl"]
                ?? "http://localhost:3000";

            // You can customize the URL format as needed
            return $"{baseUrl}/login?location={locationId}&table={tableNumber}";
        }

        /// <summary>
        /// Generate and save WiFi QR code to storage
        /// </summary>
        /// <param name="locationId">Location ID</param>
        /// <param name="tableNumber">Table number</param>
        /// <returns>File path of saved QR code, or null if failed</returns>
        public async Task<string?> GenerateAndSaveWifiQr(int locationId, int tableNumber)
        {
            var credentials = await GetLocationWifiCredentials(locationId);
            if (credentials == null)
            {
                return null;
            }

            var (ssid, password) = credentials.Value;
            var location = await _context.Locations.FindAsync(locationId);
            var locationName = location?.Name ?? $"Location{locationId}";

            using var qrBitmap = CreateWifiQr(ssid, password, hidden: false);
            using var labeledBitmap = RenderLabeledQr(
                qrBitmap,
                $"{locationName} — Table {tableNumber} — WiFi"
            );

            var fileName = $"wifi_L{locationId}_T{tableNumber}.png";
            var filePath = Path.Combine(_storagePath, fileName);

            labeledBitmap.Save(filePath, ImageFormat.Png);
            _logger.LogInformation($"Generated WiFi QR code: {fileName}");

            return filePath;
        }

        /// <summary>
        /// Generate and save authentication QR code to storage
        /// </summary>
        /// <param name="locationId">Location ID</param>
        /// <param name="tableNumber">Table number</param>
        /// <returns>File path of saved QR code, or null if failed</returns>
        public async Task<string?> GenerateAndSaveAuthQr(int locationId, int tableNumber)
        {
            var location = await _context.Locations.FindAsync(locationId);
            if (location == null)
            {
                _logger.LogWarning($"Location {locationId} not found");
                return null;
            }

            var locationName = location.Name ?? $"Location{locationId}";
            var authUrl = GetAuthUrl(locationId, tableNumber);

            using var qrBitmap = CreateAuthQr(authUrl);
            using var labeledBitmap = RenderLabeledQr(
                qrBitmap,
                $"{locationName} — Table {tableNumber} — Login"
            );

            var fileName = $"auth_L{locationId}_T{tableNumber}.png";
            var filePath = Path.Combine(_storagePath, fileName);

            labeledBitmap.Save(filePath, ImageFormat.Png);
            _logger.LogInformation($"Generated Auth QR code: {fileName}");

            return filePath;
        }

        /// <summary>
        /// Check if QR code already exists in storage
        /// </summary>
        public bool QrCodeExists(int locationId, int tableNumber, string type)
        {
            var fileName = $"{type.ToLower()}_L{locationId}_T{tableNumber}.png";
            var filePath = Path.Combine(_storagePath, fileName);
            return File.Exists(filePath);
        }

        /// <summary>
        /// Get existing QR code file path
        /// </summary>
        public string? GetExistingQrCodePath(int locationId, int tableNumber, string type)
        {
            var fileName = $"{type.ToLower()}_L{locationId}_T{tableNumber}.png";
            var filePath = Path.Combine(_storagePath, fileName);
            return File.Exists(filePath) ? filePath : null;
        }

        /// <summary>
        /// Delete QR code from storage
        /// </summary>
        public void DeleteQrCode(int locationId, int tableNumber, string type)
        {
            var fileName = $"{type.ToLower()}_L{locationId}_T{tableNumber}.png";
            var filePath = Path.Combine(_storagePath, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation($"Deleted QR code: {fileName}");
            }
        }
    }
}
