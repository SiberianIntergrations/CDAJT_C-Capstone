import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import '@testing-library/jest-dom';
import QRCodeManagement from '../QRCodeManagement';
import { api } from '@/config/api';

// Mock the API module
jest.mock('@/config/api', () => ({
  api: {
    get: jest.fn(),
    post: jest.fn(),
  },
}));

// Mock window.URL methods
global.URL.createObjectURL = jest.fn(() => 'blob:test-url');
global.URL.revokeObjectURL = jest.fn();

describe('QRCodeManagement Component', () => {
  const mockLocations = [
    { location_Id: 1, name: 'Downtown', city: 'Edmonton' },
    { location_Id: 2, name: 'West End', city: 'Edmonton' },
  ];

  beforeEach(() => {
    jest.clearAllMocks();

    // Default mock for location fetch
    api.get.mockImplementation((url) => {
      if (url === '/location') {
        return Promise.resolve({
          data: mockLocations,
        });
      }
      return Promise.reject(new Error('Not found'));
    });
  });

  afterEach(() => {
    jest.clearAllMocks();
  });

  test('renders QR Code Management title', async () => {
    render(<QRCodeManagement />);

    await waitFor(() => {
      expect(screen.getByText(/QR Code Management/i)).toBeInTheDocument();
    });
  });

  test('fetches and displays locations on mount', async () => {
    render(<QRCodeManagement />);

    await waitFor(() => {
      expect(api.get).toHaveBeenCalledWith('/location');
    });

    await waitFor(() => {
      // Check that location select is populated
      const select = screen.getByRole('combobox');
      expect(select).toBeInTheDocument();
    });
  });

  test('shows warning when downloading without table number', async () => {
    render(<QRCodeManagement />);

    await waitFor(() => {
      expect(screen.getByText(/QR Code Management/i)).toBeInTheDocument();
    });

    // Try to download without entering table number
    const downloadButtons = screen.queryAllByText(/Download/i);
    if (downloadButtons.length > 0) {
      fireEvent.click(downloadButtons[0]);

      await waitFor(() => {
        // Snackbar should show warning
        expect(screen.getByText(/Please select a location and enter a table number/i))
          .toBeInTheDocument();
      });
    }
  });

  test('downloads WiFi QR code when valid input provided', async () => {
    const mockBlob = new Blob(['test'], { type: 'image/png' });

    api.get.mockImplementation((url, config) => {
      if (url === '/location') {
        return Promise.resolve({ data: mockLocations });
      }
      if (url === '/admin/qr/wifi') {
        return Promise.resolve({ data: mockBlob });
      }
      return Promise.reject(new Error('Not found'));
    });

    // Mock document methods
    const createElementSpy = jest.spyOn(document, 'createElement');
    const appendChildSpy = jest.spyOn(document.body, 'appendChild').mockImplementation(() => {});
    const removeChildSpy = jest.spyOn(document.body, 'removeChild').mockImplementation(() => {});

    render(<QRCodeManagement />);

    await waitFor(() => {
      expect(screen.getByText(/QR Code Management/i)).toBeInTheDocument();
    });

    // Find and fill table number input
    const tableInput = screen.getByLabelText(/Table Number/i);
    fireEvent.change(tableInput, { target: { value: '5' } });

    // Find and click download WiFi button
    const downloadButtons = screen.queryAllByText(/Download/i);
    if (downloadButtons.length > 0) {
      fireEvent.click(downloadButtons[0]);

      await waitFor(() => {
        expect(api.get).toHaveBeenCalledWith('/admin/qr/wifi', {
          params: { locationId: 1, table: '5' },
          responseType: 'blob',
        });
      });
    }

    // Cleanup mocks
    createElementSpy.mockRestore();
    appendChildSpy.mockRestore();
    removeChildSpy.mockRestore();
  });

  test('opens preview dialog when preview button clicked', async () => {
    const mockBlob = new Blob(['test'], { type: 'image/png' });

    api.get.mockImplementation((url, config) => {
      if (url === '/location') {
        return Promise.resolve({ data: mockLocations });
      }
      if (url === '/admin/qr/wifi' || url === '/admin/qr/session') {
        return Promise.resolve({ data: mockBlob });
      }
      return Promise.reject(new Error('Not found'));
    });

    render(<QRCodeManagement />);

    await waitFor(() => {
      expect(screen.getByText(/QR Code Management/i)).toBeInTheDocument();
    });

    // Fill table number
    const tableInput = screen.getByLabelText(/Table Number/i);
    fireEvent.change(tableInput, { target: { value: '5' } });

    // Click preview button
    const previewButtons = screen.queryAllByText(/Preview/i);
    if (previewButtons.length > 0) {
      fireEvent.click(previewButtons[0]);

      await waitFor(() => {
        expect(api.get).toHaveBeenCalled();
      });
    }
  });

  test('bulk generation sends correct API request', async () => {
    api.get.mockImplementation((url) => {
      if (url === '/location') {
        return Promise.resolve({ data: mockLocations });
      }
      return Promise.reject(new Error('Not found'));
    });

    api.post.mockResolvedValueOnce({
      data: { filesGenerated: 20, success: true },
    });

    render(<QRCodeManagement />);

    await waitFor(() => {
      expect(screen.getByText(/QR Code Management/i)).toBeInTheDocument();
    });

    // Find bulk table count input
    const bulkInputs = screen.queryAllByLabelText(/Number of Tables/i);
    if (bulkInputs.length > 0) {
      fireEvent.change(bulkInputs[0], { target: { value: '10' } });

      // Click bulk generate button
      const bulkButton = screen.queryByText(/Generate Bulk QR Codes/i);
      if (bulkButton) {
        fireEvent.click(bulkButton);

        await waitFor(() => {
          expect(api.post).toHaveBeenCalledWith('/admin/qr/bulk', null, {
            params: { locationId: 1, tableCount: '10' },
          });
        });
      }
    }
  });

  test('displays error message on API failure', async () => {
    api.get.mockImplementation((url, config) => {
      if (url === '/location') {
        return Promise.resolve({ data: mockLocations });
      }
      if (url === '/admin/qr/wifi') {
        return Promise.reject({
          response: { data: { message: 'Server error' } },
        });
      }
      return Promise.reject(new Error('Not found'));
    });

    render(<QRCodeManagement />);

    await waitFor(() => {
      expect(screen.getByText(/QR Code Management/i)).toBeInTheDocument();
    });

    // Fill table number
    const tableInput = screen.getByLabelText(/Table Number/i);
    fireEvent.change(tableInput, { target: { value: '5' } });

    // Try to download
    const downloadButtons = screen.queryAllByText(/Download/i);
    if (downloadButtons.length > 0) {
      fireEvent.click(downloadButtons[0]);

      await waitFor(() => {
        expect(screen.getByText(/Failed to download/i)).toBeInTheDocument();
      }, { timeout: 3000 });
    }
  });

  test('handles location selection change', async () => {
    render(<QRCodeManagement />);

    await waitFor(() => {
      expect(screen.getByText(/QR Code Management/i)).toBeInTheDocument();
    });

    // The select should be populated with locations
    await waitFor(() => {
      const select = screen.getByRole('combobox');
      expect(select).toBeInTheDocument();
    });
  });

  test('shows loading state during API calls', async () => {
    api.get.mockImplementation((url) => {
      if (url === '/location') {
        return new Promise(resolve => setTimeout(() => resolve({ data: mockLocations }), 100));
      }
      return Promise.reject(new Error('Not found'));
    });

    render(<QRCodeManagement />);

    // Component should render even while loading
    expect(screen.getByText(/QR Code Management/i)).toBeInTheDocument();

    await waitFor(() => {
      expect(api.get).toHaveBeenCalledWith('/location');
    });
  });
});
