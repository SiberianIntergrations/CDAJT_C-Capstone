"use client";

import { useState, useEffect } from "react";
import SendIcon from "@mui/icons-material/Send";
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
import publicApi from "@/config/publicApi"

const ServiceRequestForm = () => {
  const [message, setMessage] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState(false);
  const [sessionId, setSessionId] = useState(null);
  const [userId, setUserId] = useState(null);

  const isGuest = () => typeof window !== "undefined" && localStorage.getItem("guest") === "true";

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
        if (isGuest()) {
          const sid = localStorage.getItem("session_id");
          if (sid) {
            setSessionId(parseInt(sid));
            return;
          } else {
            setError("Guest session not found. Please scan your table QR again.");
            return;
          }
        }

        const apiClient = isGuest() ? publicApi : api;
        const response = await apiClient.get("/DiningSession/participants/active-session-id");

        if (response?.data?.session_id) setSessionId(response.data.session_id);
        else setError("No active session found");
      } catch (err) {
        console.error("Error fetching session:", err);
        setError(err.response?.data?.message || "Unexpected error occurred");
      }
    };

      getActiveSession();
    }, []);

    //Send the service request to the staff
    const handleSubmit = async (e) => {

      const apiClient = isGuest() ? publicApi : api;
      e.preventDefault();
      if (!message.trim() || !sessionId) return;

      setIsSubmitting(true);
      setError("");
      setSuccess(false);

      const payload = {
      Session_Id: sessionId,
      Table_Id: localStorage.getItem("table_id"),
      Request_By_Name: localStorage.getItem("guest") ? "Guest" : getUserName(),
      Request_By_Oid: localStorage.getItem("guest_oid") || "",
      Notes: message,
    };

    try {
      const response = await apiClient.post(`/ServiceRequest/${sessionId}`, payload);
 
      if (response.status !== 200) {
        const data = response.data;
        let errorMessage = "Failed to submit request";
        if (data && typeof data === "object" && data.detail) {
          errorMessage = data.detail;
        }
        throw new Error(errorMessage);
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
