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
  Button,
} from "@mui/material";
import { Plus, Receipt } from "lucide-react";
import { styled } from "@mui/material/styles";
import api from "@/config/api";
import publicApi from "@/config/publicApi";
import NewBillDialog from "@/components/staff/SessionDashboard/components/dialogs/NewBillDialog";
import BillSummaryDialog from "@/components/staff/SessionDashboard/components/dialogs/BillSummaryDialog";
import { SessionProvider } from "@/components/staff/SessionDashboard/context/SessionContext";
import { getStatusColorValue, formatDateTime, formatBillStatus, getTableDescription, sortBillsByStatus } from "@/components/staff/SessionDashboard/utils/sessionHelpers";
import dynamic from "next/dynamic";
import { useRouter, useSearchParams } from "next/navigation";

const Tour = dynamic(() => import("@/components/Tour"), { ssr: false });

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

const steps = [
  {
    target: '[data-tour="bills-title"]',
    content: "Start by creating a bill for your table. Specify who you're paying for.",
    placement: "bottom",
    disableBeacon: true,
  },
  {
    target: '[data-tour="add-bill-btn"]',
    content: "Click here to add a new bill.",
    placement: "left",
    disableBeacon: true,
  },
  {
    target: '[data-tour="bill-dialog-name"]',
    content: "Enter a name for your bill (e.g., 'Family Dinner').",
    placement: "bottom",
    disableBeacon: true,
  },
  {
    target: '[data-tour="bill-dialog-guests"]',
    content: "Select the guests covered by this bill.",
    placement: "bottom",
    disableBeacon: true,
  },
  {
    target: '[data-tour="bill-dialog-submit"]',
    content: "Submit your bill when ready.",
    placement: "bottom",
    disableBeacon: true,
  },
  {
    target: '[data-tour="bills-list"]',
    content: "Here's your new bill! You can view details or edit it.",
    placement: "bottom",
    disableBeacon: true,
  },
  {
    target: '[data-tour="bill-select"]',
    content: "Next we'll move to the menu to order food",
    placement: "bottom",
    route: "/menu/full-menu",
  },
];

const BillsDashboard = () => {
  const [bills, setBills] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [summaryDialogOpen, setSummaryDialogOpen] = useState(false);
  const [selectedBillId, setSelectedBillId] = useState(null)
  const [sessionId, setSessionId] = useState(null);
  const [showTour, setShowTour] = useState(false);
  const [tourStepIndex, setTourStepIndex] = useState(0);
  const [newBillId, setNewBillId] = useState(null);

  const router = useRouter();
  const searchParams = useSearchParams();

  const isGuest = () => localStorage.getItem("guest") === "true";

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
        const response = await apiClient.get("/DiningSession/participants/active-session-id/latest");


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

    const apiClient = isGuest() ? publicApi : api;

    try {
      setLoading(true);
      setError(null);

      const res = await apiClient.get(`/Bill/get_bills/${sid}`);

      if (res.status !== 200) {
        throw new Error("Failed to fetch bills");
      }

      const billsData = Array.isArray(res.data) ? res.data : [];

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

  const handleBillCreated = async (createdBillId) => {
    if (sessionId) {
      await fetchBills(sessionId);
      setNewBillId(createdBillId); // Track the new bill for highlighting
    }
    setDialogOpen(false);
    // If tour is running, advance to highlight step
    if (showTour && tourStepIndex === 4) {
      setTourStepIndex(5);
    }
  };

  const handleViewSummary = (billId) => {
    setSelectedBillId(billId);
    setSummaryDialogOpen(true);
  };

  useEffect(() => {
    // Start tour if query param is present (works on navigation too)
    const tourParam = searchParams.get("tour");
    if (tourParam === "1") {
      setShowTour(true);
      setTourStepIndex(0);
    }
  }, [searchParams]);

  const handleTourCallback = (data) => {
    const { status, index, action, type } = data;

    if (
      ["finished", "skipped"].includes(status?.toLowerCase()) ||
      (type === "step:after" && index === steps.length - 1)
    ) {
      setShowTour(false);
      setTourStepIndex(0);
      return;
    }

    if (steps[index]?.route) {
      router.push(`${steps[index].route}?tour=1`);
      setShowTour(false);
      setTourStepIndex(0);
      return;
    }

    if (type === "step:before" && index === 1) {
      setDialogOpen(true);
    }

    // Wait for bill submission before moving from step 4 to 5
    if (type === "step:after" && index === 4 && action === "next") {
      if (newBillId) {
        setTourStepIndex(5);
      } else {
        setTourStepIndex(4);
      }
      return;
    }

    if (type === "step:after" && action === "next") {
      setTourStepIndex((prev) => Math.min(prev + 1, steps.length - 1));
    } else if (type === "step:after" && action === "prev") {
      setTourStepIndex((prev) => Math.max(prev - 1, 0));
    }
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="100vh">
        <CircularProgress />
      </Box>
    );
  }

  const sortedBills = sortBillsByStatus(bills || []);

  return (
    <>
      <Tour
        run={showTour}
        setRun={setShowTour}
        stepIndex={tourStepIndex}
        setStepIndex={setTourStepIndex}
        callback={handleTourCallback}
        steps={steps}
      />
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
              <Typography variant="h4" data-tour="bills-title">Your Table's Bills</Typography>
                {sessionId && (
                  <AddButton onClick={() => setDialogOpen(true)} data-tour="add-bill-btn">
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
                data-tour="bills-list"
                sx={{
                  display: "grid",
                  gridTemplateColumns: "repeat(auto-fill, minmax(280px, 1fr))",
                  gap: 2,
                  width: "100%",
                }}
              >
                {sortedBills.map((bill) => {
                    if (!bill?.bill_id) return null;
                    const statusColor = getStatusColorValue(bill.status);
                    // Highlight the new bill if tour is at step 5
                    const highlight = showTour && tourStepIndex === 5 && bill.bill_id === newBillId;
                    return (
                      <BillCard
                        key={bill.bill_id}
                        $statusColor={statusColor}
                        data-tour={highlight ? "bill-card-highlight" : "bill-card"}
                        sx={highlight ? { boxShadow: 8, border: "2px solid #1976d2" } : {}}
                      >
                        <Typography variant="h6" component="div" noWrap>
                          {bill.bill_name || `Bill #${bill.bill_id}`}
                        </Typography>

                        <StatusChip $statusColor={statusColor}>
                          {formatBillStatus(bill.status)}
                        </StatusChip>

                        <Typography variant="body2" sx={{ color: "text.secondary", fontWeight: "medium" }}>
                          Total: {bill.total_count || 0} guest{bill.total_count !== 1 ? 's' : ''}
                        </Typography>

                        {bill.created_at && (
                          <Typography variant="body2" sx={{ color: "text.secondary" }}>
                            Created: {formatDateTime(bill.created_at)}
                          </Typography>
                        )}

                        {bill.table_numbers?.length > 0 && (
                          <Typography variant="body2" sx={{ color: "text.secondary" }}>
                            {getTableDescription(bill.table_numbers)}
                          </Typography>
                        )}

                        <Button
                          variant="outlined"
                          size="small"
                          startIcon={<Receipt size={16} />}
                          onClick={() => handleViewSummary(bill.bill_id)}
                          sx={{ mt: 1 }}
                        >
                          View Summary
                        </Button>
                      </BillCard>
                  );
                })}
              </Box>
            )}

            {/* New Bill Dialog */}
              {sessionId && (
                <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="sm" fullWidth>
                  <NewBillDialog 
                    open={dialogOpen} 
                    sessionId={sessionId} 
                    onClose={(createdBillId) => handleBillCreated(createdBillId)} 
                    tourStepIndex={tourStepIndex}
                  />
                </Dialog>
              )}

              {/* Bill Summary Dialog */}
              <BillSummaryDialog
                open={summaryDialogOpen}
                sessionId={sessionId}
                billId={selectedBillId}
                onClose={() => {
                  setSummaryDialogOpen(false);
                  setSelectedBillId(null);
                }}
              />
            </Box>
        </Container>
      </SessionProvider>
    </>
  );
};

export default BillsDashboard;
