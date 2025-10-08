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
import { axiosInstance, createApiUrl } from "@/config/api";
import NewBillDialog from "@/components/staff/SessionDashboard/components/dialogs/NewBillDialog";
import { SessionProvider } from "@/components/staff/SessionDashboard/context/SessionContext";

// TODO: Tots should not be fillable when creating a bill. This is the total count of guests. Adult + Child + Senior should equal Totals.

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

const StatusChip = styled(Typography)(({ theme, $statusColor }) => ({
  color: "white",
  backgroundColor: $statusColor,
  padding: `${theme.spacing(0.5)} ${theme.spacing(1)}`,
  borderRadius: theme.shape.borderRadius,
  alignSelf: "flex-start",
  fontWeight: "medium",
  fontSize: "0.875rem",
}));

const BillCard = styled(Card)(({ theme, $statusColor }) => ({
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

const SessionContextWrapper = ({ children }) => {
  const createBill = async (sessionId, billData) => {
    try {
      const response = await axiosInstance.post(
        createApiUrl(`/bills/${sessionId}`),
        {
          bill_name: billData.billName,
          adult_count: parseInt(billData.adultCount),
          child_count: parseInt(billData.childCount),
          senior_count: parseInt(billData.seniorCount),
          tot_count: parseInt(billData.totCount),
        }
      );
      if (!response.statusText === "OK")
        throw new Error("Failed to create bill");
      return true;
    } catch (error) {
      console.error("Error creating bill:", error);
      return false;
    }
  };

  return (
    <SessionProvider
      value={{
        createBill,
        dialogState: {
          newBill: false,
          addTable: false,
          currentSessionId: null,
        },
        openDialog: () => {},
        closeDialog: () => {},
        addTable: async () => {},
        createSession: async () => {},
        closeBill: async () => {},
        endSession: async () => {},
        refreshData: async () => {},
        actionError: null,
        clearActionError: () => {},
      }}
    >
      {children}
    </SessionProvider>
  );
};

const BillsDashboard = () => {
  const [bills, setBills] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [sessionId, setSessionId] = useState(null);

  const fetchActiveSession = async () => {
    try {
      const response = await axiosInstance.get(
        createApiUrl("/dining-sessions/participants/active-session-id")
      );
      if (!response.statusText === "OK")
        throw new Error("Failed to fetch active session");

      const data = response.data;
      if (data?.session_id) {
        setSessionId(data.session_id);
      }
    } catch (err) {
      console.error("Error fetching session:", err);
      setError("Unable to fetch active session");
    }
  };

  const fetchBills = async () => {
    try {
      setLoading(true);
      setError(null);

      const response = await axiosInstance.get(
        createApiUrl("/bills/active/bills")
      );
      if (!response.statusText === "OK")
        throw new Error("Failed to fetch bills");

      setBills(Array.isArray(response.data) ? response.data : []);
    } catch (err) {
      console.error("Error fetching bills:", err);
      if (err.response?.status === 404) {
        setBills([]);
      } else {
        setError(err.response?.data?.detail || "Error loading bills");
      }
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    Promise.all([fetchActiveSession(), fetchBills()]);
  }, []);

  const handleBillCreated = async () => {
    await fetchBills();
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
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight="100vh"
      >
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Container maxWidth="lg">
      <Box sx={{ py: 3 }}>
        {/* Header */}
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            mb: 4,
          }}
        >
          <Typography variant="h4">Your Table's Bills</Typography>
          <AddButton onClick={() => setDialogOpen(true)}>
            <Plus />
          </AddButton>
        </Box>

        {/* Error Alert */}
        {error && (
          <Alert severity="error" sx={{ mb: 3 }} onClose={() => setError(null)}>
            {typeof error === "string" ? error : "Error loading bills"}
          </Alert>
        )}

        {/* Bills Grid */}
        {!bills || bills.length === 0 ? (
          <Alert severity="info">
            No bills yet. Create your first bill to get started.
          </Alert>
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

                  <Typography
                    variant="body2"
                    sx={{ color: "text.secondary", mt: 1 }}
                  >
                    {formatGuestCount(bill)}
                  </Typography>

                  {bill.created_at && (
                    <Typography
                      variant="body2"
                      sx={{ color: "text.secondary" }}
                    >
                      Created: {new Date(bill.created_at).toLocaleTimeString()}
                    </Typography>
                  )}

                  {bill.table_numbers?.length > 0 && (
                    <Typography
                      variant="body2"
                      sx={{ color: "text.secondary" }}
                    >
                      Tables: {bill.table_numbers.join(", ")}
                    </Typography>
                  )}
                </BillCard>
              );
            })}
          </Box>
        )}

        {/* Create Bill Dialog */}
        <SessionContextWrapper>
          <Dialog
            open={dialogOpen}
            onClose={() => setDialogOpen(false)}
            maxWidth="sm"
            fullWidth
          >
            <NewBillDialog
              open={dialogOpen}
              sessionId={sessionId}
              onClose={handleBillCreated}
            />
          </Dialog>
        </SessionContextWrapper>
      </Box>
    </Container>
  );
};

export default BillsDashboard;
