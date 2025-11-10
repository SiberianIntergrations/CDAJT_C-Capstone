import { useState } from "react";
import { Box, Typography, Chip, Button, Alert, Collapse } from "@mui/material";
import { Users } from "lucide-react";
import { useSession } from "../../context/SessionContext";
import SwipeableBillCard from "./SwipeableBillCard";
import BillSummaryDialog from "@/components/staff/SessionDashboard/components/dialogs/BillSummaryDialog";
import { sortBillsByStatus } from "@/components/staff/SessionDashboard/utils/sessionHelpers";

const BillSection = ({ session }) => {
  const { openDialog, closeBill } = useSession();
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);
  const [summaryDialogOpen, setSummaryDialogOpen] = useState(false);
  const [selectedBillId, setSelectedBillId] = useState(null);

  const handleCloseBill = async (billId) => {
    try {
      setLoading(true);
      await closeBill(session.session_Id, billId);
      setError(null); // Clear error on success
    } catch (err) {
      console.error("Error closing bill:", err);
      setError(err.message || "Failed to close bill. Please try again");
    } finally {
      setLoading(false);
    }
  };

  const handleViewSummary = (billId) => {
    setSelectedBillId(billId);
    setSummaryDialogOpen(true);
  };

  const sortedBills = sortBillsByStatus(session.bills || []);

  return (
    <Box sx={{ mt: 2 }}>
      <Collapse in={Boolean(error)}>
        <Alert severity="error" onClose={() => setError(null)} sx={{ mb: 2 }}>
          {error}
        </Alert>
      </Collapse>
      
      <Box
        sx={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          mb: 2,
        }}
      >
        <Typography
          variant="subtitle2"
          sx={{
            display: "flex",
            alignItems: "center",
            gap: 1,
          }}
        >
          <Users className="w-4 h-4" />
          Bills: {session.bill_Count || 0}
        </Typography>
        <Button
          size="small"
          variant="contained"
          onClick={() => openDialog("newBill", session.session_Id)}
          disabled={session.table_Numbers.length === 0 || loading}
          sx={{
            minWidth: 100,
            backgroundColor: "primary.main",
            "&:hover": {
              backgroundColor: "primary.dark",
            },
          }}
        >
          New Bill
        </Button>
      </Box>
      
      <Box
        sx={{
          display: "flex",
          flexDirection: "column",
          gap: 1,
        }}
      >
        {sortedBills.map((bill) => (
          <SwipeableBillCard
            key={bill.bill_Id}
            bill={bill}
            onClose={handleCloseBill}
            onViewSummary={handleViewSummary}
            disabled={loading}
          />
        ))}
      </Box>

      <BillSummaryDialog
        open={summaryDialogOpen}
        sessionId={session.session_Id}
        billId={selectedBillId}
        onClose={() => {
          setSummaryDialogOpen(false);
          setSelectedBillId(null);
        }}
      />
    </Box>
  );
};

export default BillSection;
