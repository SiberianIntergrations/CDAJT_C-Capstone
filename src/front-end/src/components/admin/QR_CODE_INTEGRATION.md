# QR Code Management - Front-End Integration Guide

## Overview

The QR Code Management feature has been successfully integrated into the admin/staff menu. Both admin and staff users can now generate and download QR codes for WiFi and authentication directly from the web interface.

## File Structure

```
src/front-end/src/
├── app/admin/qr-codes/
│   └── page.jsx                          # QR Code page route
├── components/admin/
│   ├── QRCodeManagement.jsx              # Main QR code component
│   └── QR_CODE_INTEGRATION.md            # This file
└── components/
    └── AppBarWithTitle.jsx                # Updated navigation menu
```

## Features

### 1. **Individual QR Code Generation**
- Generate WiFi QR codes for table-specific network access
- Generate Authentication QR codes for customer login
- Preview QR codes before downloading
- Instant download as PNG files

### 2. **Bulk Generation**
- Generate QR codes for multiple tables at once
- Files saved on server in `storage/qrcodes/` directory
- Useful for printing table tents or signage

### 3. **Cache Management**
- Clear cached QR codes for specific tables
- Regenerate QR codes after WiFi credential changes

## Navigation

The QR Code Management page is accessible from the main navigation menu:

### For Admin Users:
```
Menu → QR Codes
```

### For Staff Users:
```
Menu → QR Codes
```

**Route:** `/admin/qr-codes`

## Component Usage

### QRCodeManagement Component

The main component handles all QR code operations:

```jsx
import QRCodeManagement from '@/components/admin/QRCodeManagement';

// Used in /admin/qr-codes/page.jsx
<QRCodeManagement />
```

### Key Functions:

1. **Download WiFi QR Code**
   - User selects location and table number
   - Clicks "Download" on WiFi card
   - QR code downloads as `wifi_L{locationId}_T{table}.png`

2. **Download Auth QR Code**
   - User selects location and table number
   - Clicks "Download" on Auth card
   - QR code downloads as `auth_L{locationId}_T{table}.png`

3. **Preview QR Code**
   - Click "Preview" to see QR code before downloading
   - Modal dialog shows the QR code image

4. **Bulk Generate**
   - Enter number of tables (1-1000)
   - Click "Generate Bulk QR Codes"
   - Files saved on server (not downloaded)

5. **Clear Cache**
   - Select location and table
   - Click "Clear Cache for Table X"
   - Forces regeneration on next request

## API Endpoints Used

The component communicates with these backend endpoints:

```javascript
// Get WiFi QR code
GET /api/admin/qr/wifi?locationId={id}&table={number}

// Get Auth QR code
GET /api/admin/qr/auth?locationId={id}&table={number}

// Bulk generate QR codes
POST /api/admin/qr/bulk?locationId={id}&tableCount={count}

// Clear cache
DELETE /api/admin/qr/clear?locationId={id}&table={number}
```

All endpoints require authentication with Admin or Staff role.

## User Interface

### Layout Sections:

1. **Location and Table Selection**
   - Dropdown to select restaurant location
   - Input field for table number

2. **WiFi QR Code Card**
   - Icon and description
   - Preview and Download buttons
   - Shows selected location and table

3. **Authentication QR Code Card**
   - Icon and description
   - Preview and Download buttons
   - Shows selected location and table

4. **Bulk Generation Section**
   - Input for number of tables
   - Generate button with loading state

5. **Cache Management Section**
   - Clear cache button for selected table

### UI Components Used:

- Material-UI Paper, Card, Grid
- Material-UI TextField, Select, Button
- Material-UI Dialog for previews
- Material-UI Snackbar for notifications
- Lucide React icons (QrCode, Wifi, Lock, Download)

## Error Handling

The component includes comprehensive error handling:

- **Missing location/table**: Warning message via Snackbar
- **API errors**: Error message with details
- **Invalid input**: Validation on number of tables (1-1000)
- **Network issues**: Catch and display error messages

## Notifications

Users receive feedback through Snackbar notifications:

- ✅ Success: "WiFi QR code downloaded successfully!"
- ⚠️ Warning: "Please select a location and enter a table number"
- ❌ Error: "Failed to download QR code"

## Responsive Design

The component is fully responsive:

- **Desktop**: Two-column layout for WiFi/Auth cards
- **Tablet**: Stacked cards with full width
- **Mobile**: Single column with optimized spacing

## Configuration

### Backend Configuration (appsettings.json):

```json
{
  "QRCodeSettings": {
    "WiFi": {
      "SSID": "Redsox-5G",
      "Password": "bpjh38vw83"
    },
    "AuthPageUrl": "http://192.168.1.78:3000/login",
    "Locations": {
      "1": {
        "WiFi": {
          "SSID": "Location1-WiFi",
          "Password": "custom_password"
        }
      }
    }
  }
}
```

### Frontend Configuration (api.js):

The API base URL is configured in `/config/api.js`:

```javascript
export const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5264";
```

## Testing

### To test the integration:

1. **Login as Admin or Staff**
   ```
   Navigate to /login
   Enter admin/staff credentials
   ```

2. **Access QR Codes Page**
   ```
   Click menu icon → Select "QR Codes"
   ```

3. **Generate QR Code**
   ```
   Select a location
   Enter table number (e.g., 5)
   Click Preview or Download
   ```

4. **Verify Download**
   ```
   Check Downloads folder for:
   - wifi_L1_T5.png
   - auth_L1_T5.png
   ```

5. **Test Bulk Generation**
   ```
   Select location
   Enter "10" for number of tables
   Click "Generate Bulk QR Codes"
   Check server storage/qrcodes/ directory
   ```

## Common Issues & Solutions

### Issue: "Failed to fetch locations"
**Solution:** Ensure backend is running and `/api/location` endpoint is accessible

### Issue: "401 Unauthorized"
**Solution:** Check that user is logged in and token is valid

### Issue: "QR code not downloading"
**Solution:** Check browser's download permissions and popup blockers

### Issue: "Invalid table number"
**Solution:** Enter a positive integer (1 or greater)

### Issue: "Bulk generation fails"
**Solution:** Ensure table count is between 1-1000 and location is selected

## Future Enhancements

Potential improvements to consider:

- [ ] Download multiple QR codes as ZIP file
- [ ] Custom QR code colors/branding
- [ ] QR code templates for different table sizes
- [ ] Print preview with multiple QR codes per page
- [ ] QR code analytics (scan tracking)
- [ ] Schedule automatic regeneration
- [ ] Email QR codes to staff
- [ ] Integration with table management system

## Support

For issues or questions:
- Backend documentation: `src/back-end/Services/QrCode/README.md`
- API documentation: Swagger UI at `/swagger`
- Component source: `src/front-end/src/components/admin/QRCodeManagement.jsx`
