import { useState, useEffect, useCallback } from "react";
import {
  Box,
  Typography,
  Chip,
  IconButton,
  Alert,
  Collapse,
} from "@mui/material";
import { Bell, Check, AlertCircle } from "lucide-react";
import { axiosInstance, createApiUrl } from "@/config/api";

const ServiceRequests = ({ session, onRequestsUpdate }) => {
  const [requests, setRequests] = useState([]);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  const fetchRequests = useCallback(async () => {
    try {
      const response = await axiosInstance.get(
        createApiUrl(`/service-requests/by-session/${session.session_id}`)
      );
      if (response.statusText !== "OK") {
        throw new Error("Failed to fetch service requests");
      }

      const data = response.data;

      const hasChanges =
        data.length !== requests.length ||
        data.some(
          (newReq) =>
            !requests.find((oldReq) => oldReq.request_id === newReq.request_id)
        );

      if (hasChanges) {
        setRequests(data);
        onRequestsUpdate(data);
      }
    } catch (err) {
      console.error("Error fetching requests:", err);
      setError("Failed to load service requests");
    } finally {
      setLoading(false);
    }
  }, [session.session_id, requests, onRequestsUpdate]);

  const handleComplete = async (requestId) => {
    try {
      setLoading(true);
      const response = await axiosInstance.post(
        createApiUrl(`/service-requests/${requestId}/complete`)
      );
      if (response.statusText !== "OK") {
        throw new Error("Failed to complete request");
      }

      const updatedRequests = requests.filter(
        (req) => req.request_id !== requestId
      );
      setRequests(updatedRequests);
      onRequestsUpdate(updatedRequests);
    } catch (err) {
      console.error("Error completing request:", err);
      setError("Failed to complete request");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchRequests();

    const intervalId = setInterval(fetchRequests, 5000);

    return () => clearInterval(intervalId);
  }, [fetchRequests]);

  useEffect(() => {
    fetchRequests();
  }, [session.session_id, fetchRequests]);

  if (!requests.length && !error) {
    return null;
  }

  return (
    <Box sx={{ mt: 4 }}>
      <Collapse in={Boolean(error)}>
        <Alert severity="error" onClose={() => setError(null)} sx={{ mb: 2 }}>
          {error}
        </Alert>
      </Collapse>

      <Box sx={{ display: "flex", justifyContent: "space-between", mb: 2 }}>
        <Typography
          variant="subtitle2"
          sx={{ display: "flex", alignItems: "center", gap: 1 }}
        >
          <Bell style={{ width: 16, height: 16 }} />
          Service Requests
        </Typography>
        {requests.length > 0 && (
          <Chip
            size="small"
            color="warning"
            label={`${requests.length} Active`}
          />
        )}
      </Box>

      <Box sx={{ display: "flex", flexDirection: "column", gap: 1 }}>
        {requests.map((request) => (
          <Box
            key={request.request_id}
            sx={{
              display: "flex",
              justifyContent: "space-between",
              p: 1,
              backgroundColor: "rgba(245, 245, 245, 0.5)",
              borderRadius: 1,
              border: "1px solid",
              borderColor: "grey.200",
            }}
          >
            <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
              <AlertCircle
                style={{ width: 16, height: 16, color: "#FF9800" }}
              />
              <Box>
                <Typography variant="body2">
                  {request.notes || "No details provided"}
                </Typography>
              </Box>
            </Box>
            <IconButton
              size="small"
              onClick={() => handleComplete(request.request_id)}
              disabled={loading}
              sx={{
                color: "#4caf50",
                "&:hover": { color: "#388e3c" },
              }}
            >
              <Check style={{ width: 16, height: 16 }} />
            </IconButton>
          </Box>
        ))}
      </Box>
    </Box>
  );
};

export default ServiceRequests;
