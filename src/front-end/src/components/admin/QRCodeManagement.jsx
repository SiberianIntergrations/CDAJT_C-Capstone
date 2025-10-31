'use client';

import React, { useState, useEffect } from 'react';
import {
  Box,
  Paper,
  Typography,
  TextField,
  Button,
  Grid,
  Card,
  CardContent,
  CardActions,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Alert,
  Snackbar,
  CircularProgress,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Chip,
  Divider,
} from '@mui/material';
import { Download, QrCode, Wifi, Menu } from 'lucide-react';
import { api } from '@/config/api';

export default function QRCodeManagement() {
  const [locations, setLocations] = useState([]);
  const [selectedLocation, setSelectedLocation] = useState('');
  const [tableNumber, setTableNumber] = useState('');
  const [bulkTableCount, setBulkTableCount] = useState('');
  const [loading, setLoading] = useState(false);
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });
  const [previewDialog, setPreviewDialog] = useState({ open: false, url: '', title: '', type: '' });

  // Fetch locations on mount
  useEffect(() => {
    fetchLocations();
  }, []);

  const fetchLocations = async () => {
    try {
      const response = await api.get('/location');
      setLocations(response.data || []);
      if (response.data && response.data.length > 0) {
        setSelectedLocation(response.data[0].location_Id);
      }
    } catch (error) {
      console.error('Error fetching locations:', error);
      showSnackbar('Failed to fetch locations', 'error');
    }
  };

  const showSnackbar = (message, severity = 'success') => {
    setSnackbar({ open: true, message, severity });
  };

  const handleCloseSnackbar = () => {
    setSnackbar({ ...snackbar, open: false });
  };

  const downloadQRCode = async (type) => {
    if (!selectedLocation || !tableNumber) {
      showSnackbar('Please select a location and enter a table number', 'warning');
      return;
    }

    setLoading(true);
    try {
      const endpoint = type === 'wifi' ? '/admin/qr/wifi' : '/admin/qr/session';
      const response = await api.get(endpoint, {
        params: {
          locationId: selectedLocation,
          table: tableNumber,
        },
        responseType: 'blob',
      });

      // Create blob URL and trigger download
      const blob = new Blob([response.data], { type: 'image/png' });
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = `${type}_L${selectedLocation}_T${tableNumber}.png`;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      window.URL.revokeObjectURL(url);

      showSnackbar(`${type === 'wifi' ? 'WiFi' : 'Session'} QR code downloaded successfully!`);
    } catch (error) {
      console.error('Error downloading QR code:', error);
      showSnackbar(
        error.response?.data?.message || `Failed to download ${type} QR code`,
        'error'
      );
    } finally {
      setLoading(false);
    }
  };

  const previewQRCode = async (type) => {
    if (!selectedLocation || !tableNumber) {
      showSnackbar('Please select a location and enter a table number', 'warning');
      return;
    }

    setLoading(true);
    try {
      const endpoint = type === 'wifi' ? '/admin/qr/wifi' : '/admin/qr/session';
      const response = await api.get(endpoint, {
        params: {
          locationId: selectedLocation,
          table: tableNumber,
        },
        responseType: 'blob',
      });

      const blob = new Blob([response.data], { type: 'image/png' });
      const url = window.URL.createObjectURL(blob);

      setPreviewDialog({
        open: true,
        url,
        title: `${type === 'wifi' ? 'WiFi' : 'Session'} QR Code - Location ${selectedLocation}, Table ${tableNumber}`,
        type,
      });
    } catch (error) {
      console.error('Error previewing QR code:', error);
      showSnackbar(
        error.response?.data?.message || `Failed to preview ${type} QR code`,
        'error'
      );
    } finally {
      setLoading(false);
    }
  };

  const handleClosePreview = () => {
    if (previewDialog.url) {
      window.URL.revokeObjectURL(previewDialog.url);
    }
    setPreviewDialog({ open: false, url: '', title: '', type: '' });
  };

  const bulkGenerateQRCodes = async () => {
    if (!selectedLocation || !bulkTableCount) {
      showSnackbar('Please select a location and enter the number of tables', 'warning');
      return;
    }

    const count = parseInt(bulkTableCount);
    if (isNaN(count) || count < 1 || count > 1000) {
      showSnackbar('Please enter a valid number of tables (1-1000)', 'warning');
      return;
    }

    setLoading(true);
    try {
      const response = await api.post('/admin/qr/bulk', null, {
        params: {
          locationId: selectedLocation,
          tableCount: count,
        },
      });

      showSnackbar(
        `Successfully generated ${response.data.filesGenerated} QR codes for ${count} tables!`,
        'success'
      );
    } catch (error) {
      console.error('Error generating bulk QR codes:', error);
      showSnackbar(
        error.response?.data?.message || 'Failed to generate bulk QR codes',
        'error'
      );
    } finally {
      setLoading(false);
    }
  };

  const clearCache = async () => {
    if (!selectedLocation || !tableNumber) {
      showSnackbar('Please select a location and enter a table number', 'warning');
      return;
    }

    setLoading(true);
    try {
      await api.delete('/admin/qr/clear', {
        params: {
          locationId: selectedLocation,
          table: tableNumber,
        },
      });

      showSnackbar('QR code cache cleared successfully!');
    } catch (error) {
      console.error('Error clearing cache:', error);
      showSnackbar('Failed to clear QR code cache', 'error');
    } finally {
      setLoading(false);
    }
  };

  const selectedLocationName = locations.find(
    (loc) => loc.location_Id === selectedLocation
  )?.name || '';

  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h4" gutterBottom sx={{ mb: 3, fontWeight: 600 }}>
        <QrCode style={{ display: 'inline', marginRight: '10px', verticalAlign: 'middle' }} />
        QR Code Management
      </Typography>

      <Alert severity="info" sx={{ mb: 3 }}>
        Generate QR codes for WiFi access and starting customer dining sessions. QR codes are cached on the
        server for faster access.
      </Alert>

      {/* Location and Table Selection */}
      <Paper sx={{ p: 3, mb: 3 }}>
        <Typography variant="h6" gutterBottom>
          Select Location and Table
        </Typography>
        <Grid container spacing={2}>
          <Grid item xs={12} md={6}>
            <FormControl fullWidth>
              <InputLabel>Location</InputLabel>
              <Select
                value={selectedLocation}
                onChange={(e) => setSelectedLocation(e.target.value)}
                label="Location"
              >
                {locations.map((location) => (
                  <MenuItem key={location.location_Id} value={location.location_Id}>
                    {location.name} - {location.city}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={12} md={6}>
            <TextField
              fullWidth
              label="Table Number"
              type="number"
              value={tableNumber}
              onChange={(e) => setTableNumber(e.target.value)}
              inputProps={{ min: 1 }}
            />
          </Grid>
        </Grid>
      </Paper>

      {/* Individual QR Code Generation */}
      <Grid container spacing={3} sx={{ mb: 3 }}>
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                <Wifi size={32} style={{ marginRight: '10px', color: '#1976d2' }} />
                <Typography variant="h6">WiFi QR Code</Typography>
              </Box>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                Generate a QR code that customers can scan to automatically connect to the
                restaurant WiFi network.
              </Typography>
              <Divider sx={{ my: 2 }} />
              <Typography variant="body2" sx={{ mb: 1 }}>
                <strong>Location:</strong> {selectedLocationName || 'Not selected'}
              </Typography>
              <Typography variant="body2">
                <strong>Table:</strong> {tableNumber || 'Not entered'}
              </Typography>
            </CardContent>
            <CardActions sx={{ justifyContent: 'space-between', px: 2, pb: 2 }}>
              <Button
                variant="outlined"
                startIcon={<QrCode size={18} />}
                onClick={() => previewQRCode('wifi')}
                disabled={loading || !selectedLocation || !tableNumber}
              >
                Preview
              </Button>
              <Button
                variant="contained"
                startIcon={<Download size={18} />}
                onClick={() => downloadQRCode('wifi')}
                disabled={loading || !selectedLocation || !tableNumber}
              >
                Download
              </Button>
            </CardActions>
          </Card>
        </Grid>

        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                <Menu size={32} style={{ marginRight: '10px', color: '#2e7d32' }} />
                <Typography variant="h6">Session QR Code</Typography>
              </Box>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                Generate a QR code that starts a dining session and directs customers to the menu page
                for ordering.
              </Typography>
              <Divider sx={{ my: 2 }} />
              <Typography variant="body2" sx={{ mb: 1 }}>
                <strong>Location:</strong> {selectedLocationName || 'Not selected'}
              </Typography>
              <Typography variant="body2">
                <strong>Table:</strong> {tableNumber || 'Not entered'}
              </Typography>
            </CardContent>
            <CardActions sx={{ justifyContent: 'space-between', px: 2, pb: 2 }}>
              <Button
                variant="outlined"
                startIcon={<QrCode size={18} />}
                onClick={() => previewQRCode('session')}
                disabled={loading || !selectedLocation || !tableNumber}
              >
                Preview
              </Button>
              <Button
                variant="contained"
                startIcon={<Download size={18} />}
                onClick={() => downloadQRCode('session')}
                disabled={loading || !selectedLocation || !tableNumber}
              >
                Download
              </Button>
            </CardActions>
          </Card>
        </Grid>
      </Grid>

      {/* Bulk Generation */}
      <Paper sx={{ p: 3, mb: 3 }}>
        <Typography variant="h6" gutterBottom>
          Bulk QR Code Generation
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
          Generate QR codes for multiple tables at once. Files will be saved on the server in the
          storage/qrcodes/ directory.
        </Typography>
        <Grid container spacing={2} alignItems="flex-end">
          <Grid item xs={12} md={6}>
            <TextField
              fullWidth
              label="Number of Tables"
              type="number"
              value={bulkTableCount}
              onChange={(e) => setBulkTableCount(e.target.value)}
              inputProps={{ min: 1, max: 1000 }}
              helperText="Generate WiFi and Session QR codes for tables 1 to N"
            />
          </Grid>
          <Grid item xs={12} md={6}>
            <Button
              fullWidth
              variant="contained"
              color="secondary"
              onClick={bulkGenerateQRCodes}
              disabled={loading || !selectedLocation || !bulkTableCount}
              startIcon={loading ? <CircularProgress size={20} /> : <QrCode size={20} />}
            >
              Generate Bulk QR Codes
            </Button>
          </Grid>
        </Grid>
      </Paper>

      {/* Cache Management */}
      <Paper sx={{ p: 3 }}>
        <Typography variant="h6" gutterBottom>
          Cache Management
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
          Clear cached QR codes for a specific table. Use this after changing WiFi credentials or
          authentication URLs.
        </Typography>
        <Button
          variant="outlined"
          color="error"
          onClick={clearCache}
          disabled={loading || !selectedLocation || !tableNumber}
        >
          Clear Cache for Table {tableNumber}
        </Button>
      </Paper>

      {/* Preview Dialog */}
      <Dialog open={previewDialog.open} onClose={handleClosePreview} maxWidth="sm" fullWidth>
        <DialogTitle>
          {previewDialog.title}
          <Chip
            label={previewDialog.type === 'wifi' ? 'WiFi' : 'Session'}
            size="small"
            color={previewDialog.type === 'wifi' ? 'primary' : 'success'}
            sx={{ ml: 2 }}
          />
        </DialogTitle>
        <DialogContent>
          {previewDialog.url && (
            <Box sx={{ textAlign: 'center', py: 2 }}>
              <img
                src={previewDialog.url}
                alt={previewDialog.title}
                style={{ maxWidth: '100%', height: 'auto' }}
              />
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClosePreview}>Close</Button>
          <Button
            variant="contained"
            startIcon={<Download size={18} />}
            onClick={() => {
              downloadQRCode(previewDialog.type);
              handleClosePreview();
            }}
          >
            Download
          </Button>
        </DialogActions>
      </Dialog>

      {/* Snackbar for notifications */}
      <Snackbar
        open={snackbar.open}
        autoHideDuration={6000}
        onClose={handleCloseSnackbar}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
      >
        <Alert onClose={handleCloseSnackbar} severity={snackbar.severity} sx={{ width: '100%' }}>
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Box>
  );
}
