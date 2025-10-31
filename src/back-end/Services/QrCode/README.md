# QR Code Generation Service

## Overview

This service provides secure, location-based QR code generation for WiFi credentials and dining session initiation (menu redirect). All endpoints are protected with Admin/Staff authorization.

## Folder Structure

```
src/back-end/
├── Services/QrCode/
│   ├── QrGeneratorService.cs         # Core QR generation logic
│   └── README.md                      # This file
├── Controllers/
│   └── AdminQrCodeController.cs      # Admin-only endpoints
└── storage/
    ├── .gitignore                     # Ignores generated files
    └── qrcodes/
        └── .gitkeep                   # Keeps folder in git
```

## Admin-Protected API Endpoints

All endpoints require authentication with `Admin` or `Staff` role.

### 1. Get WiFi QR Code

```http
GET /api/admin/qr/wifi?locationId={locationId}&table={tableNumber}
```

**Parameters:**
- `locationId` (int, required): Location ID
- `table` (int, required): Table number

**Response:**
- Returns WiFi QR code as PNG image
- Filename: `wifi_L{locationId}_T{table}.png`

**Example:**
```bash
curl -H "Authorization: Bearer {token}" \
  "http://localhost:5264/api/admin/qr/wifi?locationId=1&table=5" \
  --output wifi_L1_T5.png
```

### 2. Get Session QR Code

```http
GET /api/admin/qr/session?locationId={locationId}&table={tableNumber}
```

**Parameters:**
- `locationId` (int, required): Location ID
- `table` (int, required): Table number

**Response:**
- Returns session start QR code as PNG image (redirects to menu)
- Filename: `session_L{locationId}_T{table}.png`

**Example:**
```bash
curl -H "Authorization: Bearer {token}" \
  "http://localhost:5264/api/admin/qr/session?locationId=1&table=5" \
  --output session_L1_T5.png
```

### 3. Bulk Generate QR Codes

```http
POST /api/admin/qr/bulk?locationId={locationId}&tableCount={tableCount}
```

**Parameters:**
- `locationId` (int, required): Location ID
- `tableCount` (int, required): Number of tables (1-1000)

**Response:**
```json
{
  "success": true,
  "message": "Generated 40 QR codes for 20 tables",
  "locationId": 1,
  "tableCount": 20,
  "filesGenerated": 40,
  "storageLocation": "storage/qrcodes/"
}
```

**Example:**
```bash
curl -X POST -H "Authorization: Bearer {token}" \
  "http://localhost:5264/api/admin/qr/bulk?locationId=1&tableCount=20"
```

### 4. Clear QR Code Cache

```http
DELETE /api/admin/qr/clear?locationId={locationId}&table={tableNumber}
```

**Parameters:**
- `locationId` (int, required): Location ID
- `table` (int, required): Table number

**Response:**
```json
{
  "success": true,
  "message": "Cleared QR codes for Location 1, Table 5"
}
```

## Key Features

### Location-Based WiFi Credentials

Each location can have unique WiFi credentials. The system uses a fallback mechanism:

1. **First**: Checks for location-specific WiFi in `appsettings.json` under `QRCodeSettings:Locations:{locationId}:WiFi`
2. **Fallback**: Uses default WiFi credentials if no location-specific config exists

**Configuration Example:**
```json
"QRCodeSettings": {
  "WiFi": {
    "SSID": "DefaultWiFi",         // Fallback for all locations
    "Password": "defaultpass"
  },
  "Locations": {
    "1": {
      "WiFi": {
        "SSID": "Location1-WiFi",  // Location 1 specific
        "Password": "location1pass"
      }
    }
  }
}
```

> **📖 For detailed WiFi configuration instructions, see:** [`WIFI_CONFIGURATION_GUIDE.md`](../../../../WIFI_CONFIGURATION_GUIDE.md)

### File Caching

Generated QR codes are automatically cached to improve performance:

- **Storage Location:** `storage/qrcodes/`
- **WiFi Filename Format:** `wifi_L{locationId}_T{table}.png`
- **Session Filename Format:** `session_L{locationId}_T{table}.png`
- **Cache Behavior:**
  - Checks cache before regenerating
  - Returns cached file if exists
  - Regenerates if cache is cleared

**Note:** The `storage/qrcodes/` folder is ignored by git (see `storage/.gitignore`). Generated files are not committed to version control.

### Labeled QR Codes

Each QR code includes a professional label with:
- Location name (from database)
- Table number
- QR code type (WiFi or Menu)

**Label Format:**
```
Location Name — Table 5 — WiFi
Location Name — Table 5 — Menu
```

### Security Features

1. **Admin/Staff Only:** All endpoints protected with `[Authorize(Roles="Admin,Staff")]`
2. **WiFi Credentials Never Exposed:** WiFi passwords remain server-side only
3. **No Client-Side WiFi Generation:** WiFi QR codes only generated via authenticated backend endpoints
4. **Session URLs Are Public:** Session QR codes can be scanned by anyone (they start a dining session and link to menu page)

## Configuration

### appsettings.json

```json
{
  "QRCodeSettings": {
    "WiFi": {
      "SSID": "Redsox-5G",
      "Password": "bpjh38vw83"
    },
    "SessionPageUrl": "http://192.168.1.78:3000/menu",
    "Locations": {
      "1": {
        "WiFi": {
          "SSID": "Redsox-5G",
          "Password": "bpjh38vw83"
        }
      }
    }
  }
}
```

### Service Registration (Program.cs)

```csharp
builder.Services.AddScoped<back_end.Services.QrCode.QrGeneratorService>();
```

## Usage Examples

### C# Client Example

```csharp
// Using HttpClient to download WiFi QR code
using var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", jwtToken);

var response = await httpClient.GetAsync(
    "http://localhost:5264/api/admin/qr/wifi?locationId=1&table=5");

if (response.IsSuccessStatusCode)
{
    var bytes = await response.Content.ReadAsByteArrayAsync();
    await File.WriteAllBytesAsync("wifi_qr.png", bytes);
}
```

### JavaScript/Fetch Example

```javascript
// Download Session QR code
async function downloadSessionQR(locationId, table) {
  const response = await fetch(
    `/api/admin/qr/session?locationId=${locationId}&table=${table}`,
    {
      headers: {
        'Authorization': `Bearer ${token}`
      }
    }
  );

  if (response.ok) {
    const blob = await response.blob();
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `session_L${locationId}_T${table}.png`;
    a.click();
  }
}
```

### Bulk Generation Example

```javascript
// Generate QR codes for all 20 tables at location 1
async function generateAllQRCodes(locationId, tableCount) {
  const response = await fetch(
    `/api/admin/qr/bulk?locationId=${locationId}&tableCount=${tableCount}`,
    {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`
      }
    }
  );

  const result = await response.json();
  console.log(`Generated ${result.filesGenerated} QR codes`);
}

// Usage
await generateAllQRCodes(1, 20);
```

## WiFi QR Code Format

Generated WiFi QR codes use the standard WiFi QR format:

```
WIFI:T:WPA;S:{ssid};P:{password};H:false;;
```

**Parameters:**
- `T`: Encryption type (WPA/WPA2)
- `S`: SSID (network name)
- `P`: Password
- `H`: Hidden network (true/false)

When scanned with a smartphone camera, this automatically prompts to join the WiFi network.

## Session URL Format

Session QR codes link to:

```
{baseUrl}/start-session?locationId={locationId}&tableNumber={tableNumber}
```

**Example:**
```
http://192.168.1.78:3000/start-session?locationId=1&tableNumber=5
```

The frontend should handle this URL by:
1. Creating a dining session via `POST /api/diningsession/Create_Dinning_Session`
2. Including the location ID and table number in the session creation
3. Redirecting to the menu page after successful session creation

**Frontend Implementation:**

✅ **The `/start-session` page has been implemented** at `src/front-end/src/app/start-session/page.jsx`

The page automatically:
1. Extracts `locationId` and `tableNumber` from query parameters
2. Looks up the `table_id` by querying all tables and finding the match
3. Gets the default menu for the location
4. Creates a dining session via `POST /api/diningsession/Create_Dinning_Session`
5. Redirects to `/menu/full-menu?sessionId={sessionId}` after success

**Flow:**
```
1. Customer scans QR code
   ↓
2. Opens: /start-session?locationId=1&tableNumber=5
   ↓
3. Page shows loading spinner
   ↓
4. Creates dining session in background
   ↓
5. Shows success message
   ↓
6. Redirects to menu (after 1.5s)
```

## Troubleshooting

### QR Code Not Generating

1. **Check database:** Ensure location exists in the database
2. **Check configuration:** Verify WiFi credentials in `appsettings.json`
3. **Check permissions:** Ensure user has Admin or Staff role
4. **Check logs:** Look for errors in application logs

### Cache Issues

To clear cache for a specific table:

```bash
curl -X DELETE -H "Authorization: Bearer {token}" \
  "http://localhost:5264/api/admin/qr/clear?locationId=1&table=5"
```

To clear all cached QR codes:

```bash
# Windows
del /Q "storage\qrcodes\*.png"

# Linux/Mac
rm -f storage/qrcodes/*.png
```

### Platform Warnings

Build warnings about `System.Drawing.Common` being Windows-only are expected. The service is designed to run on Windows servers. For cross-platform deployment, consider using:
- SkiaSharp (alternative graphics library)
- QRCoder with different rendering backend
- Docker container with Windows base image

## Best Practices

1. **Regenerate on WiFi Change:** Clear cache when WiFi credentials change
2. **Print Table Tents:** Use bulk generation to create table tent QR codes
3. **Separate Networks:** Consider different WiFi networks per location for better tracking
4. **Secure Storage:** Ensure `storage/qrcodes/` has appropriate file permissions
5. **Don't Commit QR Codes:** The `.gitignore` already handles this, but verify generated files aren't committed

## Future Enhancements

Potential improvements:

- [ ] QR code analytics (scan tracking)
- [ ] Expiring QR codes for temporary tables
- [ ] Multi-language labels
- [ ] QR code size customization
- [ ] Batch download as ZIP file
