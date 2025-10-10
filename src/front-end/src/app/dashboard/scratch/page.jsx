"use client";
import React, { useState, useEffect } from "react";
import {
  Box,
  Typography,
  Button,
  Paper,
  CircularProgress,
  Alert,
  Divider,
  Stack,
} from "@mui/material";
import BillSelect from "@/components/customer/OrderDashboard/components/BillSelect";
import { axiosInstance, createApiUrl } from "@/config/api";

const CreateOrderPage = () => {
  const [sessionId, setSessionId] = useState(null);
  const [selectedBillId, setSelectedBillId] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    const getActiveSession = async () => {
      try {
        const response = await axiosInstance.get(
          createApiUrl("/dining-sessions/participants/active-session-id")
        );
        if (!response.statusText === "OK") {
          throw new Error("Failed to fetch active session");
        }
        if (!response.ok) {
          throw new Error("Failed to fetch active session");
        }

        const data = await response.json();
        console.log("Response data:", data);

        if (typeof data === "number") {
          setSessionId(data);
        } else if (data && data.session_id) {
          setSessionId(data.session_id);
        } else {
          setError("No active session found");
        }
      } catch (err) {
        setError(err.message);
        console.error("Error fetching session:", err);
      } finally {
        setIsLoading(false);
      }
    };

    setIsLoading(true);
    getActiveSession();
  }, []);

  const handleCreateOrder = async () => {
    if (!sessionId || !selectedBillId) {
      setError("Please select a bill to create an order");
      return;
    }

    try {
      setIsSubmitting(true);
      setError(null);

      const response = await axiosInstance.post(createApiUrl("/orders"), {
        session_id: sessionId,
        bill_id: selectedBillId,
      });

      if (!response.statusText === "OK") {
        const errorData = response.data;
        throw new Error(errorData.detail || "Failed to create order");
      }

      setSelectedBillId("");

      alert("Order created successfully!");
    } catch (err) {
      setError(err.message);
      console.error("Error creating order:", err);
    } finally {
      setIsSubmitting(false);
    }
  };

  if (isLoading) {
    return (
      <Box className="flex items-center justify-center min-h-screen">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box className="p-6 max-w-2xl mx-auto">
      <Paper className="p-6">
        <Typography variant="h4" className="mb-6">
          Create New Order
        </Typography>

        {error && (
          <Alert
            severity="error"
            className="mb-4"
            onClose={() => setError(null)}
          >
            {error}
          </Alert>
        )}

        <Stack spacing={4}>
          <Box>
            <Typography variant="subtitle1" className="mb-2">
              Session #{sessionId}
            </Typography>
            <Divider />
          </Box>

          <Box>
            <Typography variant="subtitle1" className="mb-2">
              Select Bill
            </Typography>
            <BillSelect
              session_id={sessionId}
              value={selectedBillId}
              onChange={(e) => setSelectedBillId(e.target.value)}
              disabled={isSubmitting}
            />
          </Box>

          <Box className="flex justify-end gap-4">
            <Button
              variant="outlined"
              onClick={() => {
                setSelectedBillId("");
              }}
              disabled={isSubmitting}
            >
              Clear
            </Button>
            <Button
              variant="contained"
              onClick={handleCreateOrder}
              disabled={!selectedBillId || isSubmitting}
            >
              {isSubmitting ? (
                <>
                  <CircularProgress size={20} className="mr-2" />
                  Creating...
                </>
              ) : (
                "Create Order"
              )}
            </Button>
          </Box>
        </Stack>
      </Paper>
    </Box>
  );
};

export default CreateOrderPage;
