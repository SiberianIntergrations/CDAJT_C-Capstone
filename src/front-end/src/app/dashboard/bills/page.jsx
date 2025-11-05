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

const SessionContextWrapper = ({ children }) => {
  const createBill = async (sessionId, billData) => {
  const res = await api.post(`/Bill/create_Bill/${sessionId}`, {
    bill_name: billData.billName,
    adult_count: Number(billData.adultCount),
    child_count: Number(billData.childCount),
    senior_count: Number(billData.seniorCount),
    tot_count: Number(billData.totCount),
  });
  if (res.status !== 200 && res.status !== 201) throw new Error("Failed");
    return true;
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

useEffect(() => {
    fetchBills();
}, []);

  const fetchBills = async () => {
    try {
      setLoading(true);
      setError(null);

      const res = await api.get(`/Bill/active/bills`);

      setBills(Array.isArray(res.data) ? res.data : []);
    } catch (err) {
      if (err?.response?.status === 404) {
        setBills([]);
      } else {
        setError(err?.response?.data?.detail || "Error loading bills");
      }
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const loadSessionId = async () => {
      try {
        const res = await api.get("/DiningSession/participants/active-session-id");

        setSessionId(res.data?.session_id ?? null);
      } catch (e) {
        setSessionId(null);
        setError("No active session found");
      }
    };
    loadSessionId();
  }, []);

  //Code that was used for sessions
  //May be needed for staff/admin
  /*
  useEffect(() => {
    (async () => {
      await fetchActiveSession();
    })();
  }, []);

  useEffect(() => {
    if (sessionId) fetchBills(sessionId);
  }, [sessionId]);
  */

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
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="100vh">
        <CircularProgress />
      </Box>
    );
  }

  return (
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
          <AddButton onClick={() => setDialogOpen(true)}>
            <Plus />
          </AddButton>
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

        <SessionContextWrapper>
          <NewBillDialog open={dialogOpen} sessionId={sessionId} onClose={handleBillCreated} />
        </SessionContextWrapper>
      </Box>
    </Container>
  );
};

export default BillsDashboard;
