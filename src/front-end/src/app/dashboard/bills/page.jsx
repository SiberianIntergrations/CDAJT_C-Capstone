"use client";
import React, { useState, useEffect } from "react";
import {
  Box,
  Card,
  Typography,
  CircularProgress,
  Alert,
  Container,
  IconButton,
  Dialog,
} from "@mui/material";
import { Plus } from "lucide-react";
import { styled } from "@mui/material/styles";
import api from "@/config/api";
import NewBillDialog from "@/components/staff/SessionDashboard/components/dialogs/NewBillDialog";
import { SessionProvider } from "@/components/staff/SessionDashboard/context/SessionContext";

const AddButton = styled(IconButton)(({ theme }) => ({
  backgroundColor: theme.palette.primary.main,
  color: "white",
  padding: theme.spacing(1),
  minWidth: "auto",
  "&:hover": {
    backgroundColor: theme.palette.primary.dark,
  },
  "& svg": {
    width: 20,
    height: 20,
  },
}));

const StatusChip = styled(Typography, {
  shouldForwardProp: (prop) => prop !== '$statusColor',
})(({ theme, $statusColor }) => ({
  color: "white",
  backgroundColor: $statusColor,
  padding: `${theme.spacing(0.5)} ${theme.spacing(1)}`,
  borderRadius: theme.shape.borderRadius,
  alignSelf: "flex-start",
  fontWeight: "medium",
  fontSize: "0.875rem",
  userSelect: "none",
}));

const BillCard = styled(Card, {
  shouldForwardProp: (prop) => prop !== '$statusColor',
})(({ theme, $statusColor }) => ({
  padding: theme.spacing(3),
  display: "flex",
  flexDirection: "column",
  gap: theme.spacing(1),
  backgroundColor: "white",
  borderRadius: theme.shape.borderRadius,
  borderLeft: `6px solid ${$statusColor}`,
  minHeight: "200px",
  position: "relative",
  boxShadow: theme.shadows[2],
  transition: "all 0.3s cubic-bezier(0.4, 0, 0.2, 1)",
  "&:hover": {
    boxShadow: theme.shadows[4],
    transform: "translateY(-4px)",
  },
}));

const BillsDashboard = () => {
  const [bills, setBills] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [sessionId, setSessionId] = useState(null);

//May only need sessions for staff or admin
//Leaving code here now in case
/*const fetchActiveSession = async () => {
  setLoading(true);
  setError(null);
  try {
    const res = await api.get("/Dashboard/sessions");
    if (res.status !== 200) throw new Error();
    const sessions = Array.isArray(res.data) ? res.data : [];
    const s = sessions[0] || null;
    const id = s?.Session_Id ?? null;
    setSessionId(id);
    return id;
  } catch {
    setError("Unable to fetch active session");
    return null;
  } finally {
    setLoading(false);
  }
}; */

// Fetch active session ID first
  useEffect(() => {
    const fetchActiveSession = async () => {
      try {
        setLoading(true);
        setError(null);

        const response = await api.get("/DiningSession/participants/active-session-id");

        if (response.status !== 200) {
          throw new Error("Failed to fetch active session");
        }

        if (response.data && response.data.session_id) {
          setSessionId(response.data.session_id);
        } else {
          setError("No active session found. Please join a table first.");
        }
      } catch (err) {
        console.error("Error fetching active session:", err);
        setError(
          err?.response?.data?.detail || 
          err?.response?.data?.message || 
          "Unable to fetch your active session"
        );
      } finally {
        setLoading(false);
      }
    };

    fetchActiveSession();
  }, []);

// Fetch bills when sessionId is available
  useEffect(() => {
    if (sessionId) {
      fetchBills(sessionId);
    }
  }, [sessionId]);

  const fetchBills = async (sid) => {
    try {
      setLoading(true);
      setError(null);

      console.log("Fetching bills for session:", sid);
      const res = await api.get(`/Bill/get_bills/${sid}`);

      if (res.status !== 200) {
        throw new Error("Failed to fetch bills");
      }

      const billsData = Array.isArray(res.data) ? res.data : [];
      console.log("Bills fetched:", billsData);
      setBills(billsData);
    } catch (err) {
      console.error("Error fetching bills:", err);
      
      // 404 means no bills exist - not an error
      if (err?.response?.status === 404) {
        setBills([]);
        setError(null);
      } else {
        setError(
          err?.response?.data?.detail || 
          err?.response?.data?.message || 
          "Error loading bills"
        );
      }
    } finally {
      setLoading(false);
    }
  };

  const handleBillCreated = async () => {
    if (sessionId) {
      await fetchBills(sessionId);
    }
    setDialogOpen(false);
  };

  const getBillStatusColor = (status) => {
    if (!status) return "#757575";
    switch (status.toString().toUpperCase()) {
      case "OPEN":
        return "#4caf50";
      case "CLOSED":
        return "#757575";
      case "CANCELLED":
        return "#f44336";
      default:
        return "#757575";
    }
  };

  const formatGuestCount = (bill) => {
    if (!bill) return "";
    const counts = [];
    if (bill.adult_count) counts.push(`${bill.adult_count} Adults`);
    if (bill.child_count) counts.push(`${bill.child_count} Children`);
    if (bill.senior_count) counts.push(`${bill.senior_count} Seniors`);
    if (bill.tot_count) counts.push(`${bill.tot_count} Tots`);
    return counts.join(", ") || "No guests";
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="100vh">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <SessionProvider sessionId={sessionId}>
    <Container maxWidth="lg">
      <Box sx={{ py: 3 }}>
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            mb: 4,
          }}
        >
          <Typography variant="h4">Your Table's Bills</Typography>
            {sessionId && (
              <AddButton onClick={() => setDialogOpen(true)}>
                <Plus />
              </AddButton>
            )}
        </Box>

        {error && (
          <Alert severity="error" sx={{ mb: 3 }} onClose={() => setError(null)}>
            {typeof error === "string" ? error : "Error loading bills"}
          </Alert>
        )}

        {!bills || bills.length === 0 ? (
          <Alert severity="info">No bills yet. Create your first bill to get started.</Alert>
        ) : (
          <Box
            sx={{
              display: "grid",
              gridTemplateColumns: "repeat(auto-fill, minmax(280px, 1fr))",
              gap: 2,
              width: "100%",
            }}
          >
            {bills.map((bill) => {
              if (!bill?.bill_id) return null;
              const statusColor = getBillStatusColor(bill.status);

              return (
                <BillCard key={bill.bill_id} $statusColor={statusColor}>
                  <Typography variant="h6" component="div">
                    {bill.bill_name || `Bill #${bill.bill_id}`}
                  </Typography>

                  <StatusChip $statusColor={statusColor}>
                    {bill.status ? String(bill.status) : "Unknown Status"}
                  </StatusChip>

                  <Typography variant="body2" sx={{ color: "text.secondary", mt: 1 }}>
                    {formatGuestCount(bill)}
                  </Typography>

                  {bill.created_at && (
                    <Typography variant="body2" sx={{ color: "text.secondary" }}>
                      Created: {new Date(bill.created_at).toLocaleTimeString()}
                    </Typography>
                  )}

                  {bill.table_numbers?.length > 0 && (
                    <Typography variant="body2" sx={{ color: "text.secondary" }}>
                      Tables: {bill.table_numbers.join(", ")}
                    </Typography>
                  )}
                </BillCard>
              );
            })}
          </Box>
        )}

        {sessionId && (
            <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="sm" fullWidth>
              <NewBillDialog 
                open={dialogOpen} 
                sessionId={sessionId} 
                onClose={handleBillCreated} 
              />
            </Dialog>
          )}
      </Box>
    </Container>
    </SessionProvider>
  );
};

export default BillsDashboard;
