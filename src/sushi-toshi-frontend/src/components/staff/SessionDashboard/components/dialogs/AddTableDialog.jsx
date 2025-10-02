// File: sushi-toshi-frontend/components/staff/SessionDashboard/components/dialogs/AddTableDialog.jsx
import { useState, useEffect } from 'react';
import { axiosInstance, createApiUrl } from '../../../../../config/api';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  IconButton,
  Typography
} from '@mui/material';
import { X } from 'lucide-react';
import { useSession } from '../../context/SessionContext';

const AddTableDialog = ({ open, sessionId, onClose }) => {
  const { addTable } = useSession();
  const [selectedTable, setSelectedTable] = useState('');
  const [availableTables, setAvailableTables] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (open) {
      fetchTables();
    } else {
      setSelectedTable('');
      setError(null);
    }
  }, [open]);

  const fetchTables = async () => {
    try {
      setIsLoading(true);
      setError(null);
      
      // First, get all active tables
      const tablesResponse = await axiosInstance.get(createApiUrl('/table-entities'));
      const allTables = tablesResponse.data;
  
      // Then, get tables in all active sessions
      const sessionsResponse = await axiosInstance.get(createApiUrl('/dashboard/sessions'));
      const activeSessions = sessionsResponse.data;
  
      // Gather all tables in use across all sessions
      const tablesInUse = new Set();
      activeSessions.forEach(session => {
        session.table_numbers.forEach(tableNum => {
          tablesInUse.add(tableNum);
        });
      });
  
      // Filter out tables that are active and not in use in any session
      const availableTables = allTables.filter(table => 
        table.is_active && !tablesInUse.has(table.table_number)
      );
  
      setAvailableTables(availableTables);
    } catch (err) {
      console.error('Error fetching tables:', err);
      setError('Failed to load available tables');
    } finally {
      setIsLoading(false);
    }
  };
  
  const handleSubmit = async () => {
    if (!sessionId || !selectedTable) return;
    
    try {
      const success = await addTable(sessionId, selectedTable);
      if (success) {
        // Remove the selected table from the available tables list
        setAvailableTables(prevTables =>
          prevTables.filter(table => table.table_id !== selectedTable)
        );
        
        // Reset the selected table
        setSelectedTable('');
        onClose();
      }
    } catch (err) {
      setError('Failed to add table');
    }
  };
  
  return (
    <Dialog open={open} onClose={onClose} fullWidth>
      <DialogTitle>
        Add Table to Session
        <IconButton
          onClick={onClose}
          sx={{ position: 'absolute', right: 8, top: 8 }}
        >
          <X />
        </IconButton>
      </DialogTitle>
      <DialogContent>
        {error && (
          <Typography color="error" sx={{ mb: 2 }}>
            {error}
          </Typography>
        )}
        <FormControl fullWidth sx={{ mt: 2 }}>
          <InputLabel>Select Table</InputLabel>
          <Select
            value={selectedTable}
            onChange={(e) => setSelectedTable(e.target.value)}
            label="Select Table"
            disabled={isLoading}
          >
            {availableTables.length === 0 ? (
              <MenuItem disabled value="">
                {isLoading ? 'Loading tables...' : 'No available tables'}
              </MenuItem>
            ) : (
              availableTables.map((table) => (
                <MenuItem key={table.table_id} value={table.table_id}>
                  Table {table.table_number} ({table.seat_count} seats)
                </MenuItem>
              ))
            )}
          </Select>
        </FormControl>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        <Button 
          onClick={handleSubmit}
          variant="contained"
          disabled={!selectedTable || isLoading}
        >
          Add Table
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default AddTableDialog;
