// File: sushi-toshi-frontend/components/staff/SessionDashboard/components/SessionCard/BillSection.jsx
import { useState } from 'react';
import { 
  Box,
  Typography,
  Chip,
  Button,
  Alert,
  Collapse
} from '@mui/material';
import { Users } from 'lucide-react';
import { useSession } from '../../context/SessionContext';
import SwipeableBillCard from './SwipeableBillCard';

const BillSection = ({ session }) => {
  const { openDialog, closeBill } = useSession();
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  const handleCloseBill = async (billId) => {
    try {
      setLoading(true);
      const success = await closeBill(session.session_id, billId);
      if (!success) {
        throw new Error('Failed to close bill');
      }
    } catch (err) {
      console.error('Error closing bill:', err);
      setError('Failed to close bill. Please try again');
    } finally {
      setLoading(false);
    }
  };

  const getBillStatusColor = (status) => {
    switch (status) {
      case 'OPEN':
        return 'primary';
      case 'CLOSED':
        return 'default';
      case 'CANCELLED':
        return 'error';
      default:
        return 'default';
    }
  };

  return (
    <Box sx={{ mt: 2 }}>
      <Collapse in={Boolean(error)}>
        <Alert 
          severity="error" 
          onClose={() => setError(null)}
          sx={{ mb: 2 }}
        >
          {error}
        </Alert>
      </Collapse>

      <Box sx={{ 
        display: 'flex', 
        justifyContent: 'space-between', 
        alignItems: 'center',
        mb: 2 
      }}>
        <Typography 
          variant="subtitle2" 
          sx={{ 
            display: 'flex', 
            alignItems: 'center', 
            gap: 1 
          }}
        >
          <Users className="w-4 h-4" />
          Bills ({session.bills_count})
        </Typography>
        
        <Button
          size="small"
          variant="contained"
          onClick={() => openDialog('newBill', session.session_id)}
          disabled={session.table_numbers.length === 0 || loading}
          sx={{
            minWidth: 100,
            backgroundColor: 'primary.main',
            '&:hover': {
              backgroundColor: 'primary.dark',
            }
          }}
        >
          New Bill
        </Button>
      </Box>

      <Box sx={{ 
        display: 'flex', 
        flexDirection: 'column',
        gap: 1
      }}>
        {session.bills.map((bill) => (
          <SwipeableBillCard
            key={bill.bill_id}
            bill={bill}
            onClose={handleCloseBill}
            disabled={loading}
          />
        ))}
      </Box>
    </Box>
  );
};

export default BillSection;
