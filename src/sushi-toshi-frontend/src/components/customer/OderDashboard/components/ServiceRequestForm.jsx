// FIle: sushi-toshi-frontend/components/customer/OderDashboard/components/ServiceRequestForm.jsx
import { useState, useEffect } from 'react';
import SendIcon from '@mui/icons-material/Send';
import {
  Box,
  Button,
  Typography,
  TextField,
  Grid,
  Alert,
  CircularProgress
} from '@mui/material';
import { axiosInstance, createApiUrl } from '../../../../config/api';

const ServiceRequestForm = () => {
  const [message, setMessage] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);
  const [sessionId, setSessionId] = useState(null); 

  const commonRequests = [
    'Need water refill',
    'Need napkins',
    'Need utensils',
    'Need soy sauce',
    'Need wasabi',
    'Need ginger',
    'Ready for bill'
  ];

  
  useEffect(() => {
    const getActiveSession = async () => {
      try {
        const response = await axiosInstance.get(
          createApiUrl('/dining-sessions/participants/active-session-id')
        );
  
        if (response && response.data) {
          setSessionId(response.data.session_id); 
        } else {
          setError('No active session found');
        }
      } catch (err) {
        console.error('Error fetching session:', err);
        if (err.response && err.response.data) {
          setError(err.response.data.detail || 'Error fetching session');
        } else {
          setError('Unexpected error occurred');
        }
      }
    };
  
    getActiveSession();
  }, []);
  

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!message.trim() || !sessionId) return; 
    setIsSubmitting(true);
    setError('');
    setSuccess(false);

    try {
      const response = await axiosInstance.post(
        createApiUrl(`/service-requests/${sessionId}`),
        { notes: message }
      );
      console.log('Response:', response); 

      if (!response.statusText === 'OK') {
        const data = response.data;
        throw new Error(data.detail || 'Failed to submit request');
      }

      setSuccess(true);
      setMessage('');
      setTimeout(() => setSuccess(false), 3000);
    } catch (err) {
      setError(err.message);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Box maxWidth="sm" mx="auto" sx={{pt: 3}}>
      <Typography variant="h4" align="center" gutterBottom>
        Request Assistance
      </Typography>
      
      <Grid container spacing={2} justifyContent="center" mb={3}>
        {commonRequests.map((request) => (
          <Grid item key={request}>
            <Button
              variant="outlined"
              onClick={() => setMessage(request)}
            >
              {request}
            </Button>
          </Grid>
        ))}
      </Grid>

      <Box mb={2}>
        <Typography variant="h6">Custom Message</Typography>
        <TextField
          value={message}
          onChange={(e) => setMessage(e.target.value)}
          placeholder="Type your request here..."
          fullWidth
          multiline
          rows={4}
          variant="outlined"
        />
      </Box>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      {success && (
        <Alert severity="success" sx={{ mb: 2 }}>
          Request submitted successfully!
        </Alert>
      )}

      <Box display="flex" justifyContent="center">
        <Button
          onClick={handleSubmit}
          disabled={isSubmitting || !message.trim() || !sessionId} 
          variant="contained"
          color="primary"
          startIcon={isSubmitting ? <CircularProgress size={20} /> : <SendIcon />}
        >
          {isSubmitting ? 'Submitting...' : 'Send Request'}
        </Button>
      </Box>
    </Box>
  );
};

export default ServiceRequestForm;
