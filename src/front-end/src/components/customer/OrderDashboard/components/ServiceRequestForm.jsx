"use client";

import { useState, useEffect } from "react";
import SendIcon from '@mui/icons-material/Send';
import {
  Box,
  Button,
  Typography,
  TextField,
  Grid,
  Alert,
  CircularProgress,
} from "@mui/material";
import api from "@/config/api";

const ServiceRequestForm = () => {
  const [message, setMessage] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState(false);
  const [sessionId, setSessionId] = useState(null);
  const [userId, setUserId] = useState(null);

  const commonRequests = [
    "Need water refill",
    "Need napkins",
    "Need utensils",
    "Need soy sauce",
    "Need wasabi",
    "Need ginger",
    "Ready for bill",
  ];

  //Find the current user's dining session right when the page loads
  useEffect(() => {
      const getActiveSession = async () => {
        try {
          const response = await api.get("/DiningSession/participants/active-session-id");

          if (response?.data?.session_id) {
            setSessionId(response.data.session_id);
          } else {
            setError("No active session found");
          }
        } catch (err) {
          console.error("Error fetching session:", err);
          if (err.response?.data?.message) {
            setError(err.response.data.message);
          } else {
            setError("Unexpected error occurred");
          }
        }
      };

      getActiveSession();
    }, []);

    //Send the service request to the staff
    const handleSubmit = async (e) => {
      e.preventDefault();
      if (!message.trim() || !sessionId) return;

      setIsSubmitting(true);
      setError("");
      setSuccess(false);

      try {
        const response = await api.post(`/ServiceRequest/${sessionId}`, { notes: message });

        if (response.status !== 200 && response.status !== 201) {
          throw new Error(response.data?.message || "Failed to submit request");
        }

        setSuccess(true);
        setMessage("");
        setTimeout(() => setSuccess(false), 3000);

      } catch (err) {
        console.error("Service request error:", err);
        setError(err.response?.data?.message || err.message);
      } finally {
        setIsSubmitting(false);
      }
    };

  return (
    <Box maxWidth="sm" mx="auto" sx={{ pt: 3 }}>
      <Typography variant="h4" align="center" gutterBottom>
        Request Assistance
      </Typography>

      <Grid container spacing={2} justifyContent="center" mb={3}>
        {commonRequests.map((request) => (
          <Grid key={request}>
            <Button variant="outlined" onClick={() => setMessage(request)}>
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
          startIcon={
            isSubmitting ? <CircularProgress size={20} /> : <SendIcon />
          }
        >
          {isSubmitting ? "Submitting..." : "Send Request"}
        </Button>
      </Box>
    </Box>
  );
};

export default ServiceRequestForm;
