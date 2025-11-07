import { useState, useCallback } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Box,
  TextField,
  Alert,
  CircularProgress,
  Typography,
  IconButton,
} from "@mui/material";
import { X } from "lucide-react";
import { useSession } from "../../context/SessionContext";

const NewBillDialog = ({ open, sessionId, onClose }) => {
  const { createBill } = useSession();
  const [formData, setFormData] = useState({
    billName: "",
    adultCount: 0,
    childCount: 0,
    seniorCount: 0,
  });
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  const totalGuests = formData.adultCount + formData.childCount + formData.seniorCount;

  const handleFieldChange = (field, value) => {
    setFormData((prev) => ({
      ...prev,
      [field]: value,
    }));
  };

  const handleSubmit = useCallback(async () => {
    if (!formData.billName.trim()) {
      setError("Bill name is required.");
      return;
    }

    if (totalGuests === 0) {
      setError("Please add at least one guest");
      return;
    }
    setIsLoading(true);
    setError(null);

    try {
     const billData = {
        billName: formData.billName.trim(),
        adultCount: formData.adultCount,
        childCount: formData.childCount,
        seniorCount: formData.seniorCount,
      };

      console.log("Submitting bill data:", billData);
      console.log("handleSubmit called with sessionId:", sessionId, "billData:", billData);
      const success = await createBill(sessionId, billData);
      console.log("Bill creation result:", success);

      if (success) {
        handleClose();
      } else {
        setError("Failed to create bill. Please try again.");
      }
    } catch (error) {
      console.error("Error creating bill:", error);
      setError(
        error.response?.data?.message || 
        "An unexpected error occurred while creating your Bill."
      );
    } finally {
      setIsLoading(false);
    }
  }, [formData, createBill, sessionId]);

  const handleClose = useCallback(() => {
    if (isLoading) return;

    // Reset form to initial state
    setFormData({
      billName: "",
      adultCount: 0,
      childCount: 0,
      seniorCount: 0,
    });
    setError(null);
    onClose();
  }, [isLoading, onClose]);

  const isSubmitDisabled = !formData.billName.trim() || totalGuests === 0 || isLoading;

  return (
    <Dialog open={open} onClose={handleClose} fullWidth maxWidth="sm">
      <DialogTitle>
        Create New Bill
        <IconButton
          onClick={handleClose}
          sx={{ position: "absolute", right: 8, top: 8 }}
        >
          <X />
        </IconButton>
      </DialogTitle>
      <DialogContent>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 2}}>
          Create a bill for your table.
        </Typography>
        {error && (
          <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>
            {error}
          </Alert>
        )}

        <Box component="form" sx={{ mt: 2, display: "flex", flexDirection: "column", gap: 2 }}>
          <TextField
            autoFocus
            label="Bill Name"
            value={formData.billName}
            onChange={(e) => handleFieldChange("billName", e.target.value)}
            fullWidth
            placeholder="e.g., Table 1 - Party of 4"
            error={!formData.billName.trim() && formData.billName !== ""}
            disabled={isLoading}
          />

          <Box sx={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 2 }}>
            <TextField
              label="Adults"
              type="number"
              value={formData.adultCount}
              onChange={(e) => handleFieldChange("adultCount", Math.max(0, Math.min(50, parseInt(e.target.value) || 0)))}
              slotProps={{
                input: {
                  inputProps: {
                    min: 0,
                    max: 50,
                    inputMode: "numeric", // Numeric keyboard shows on mobile
                    pattern: "[0-9]*"
                  }
                }
              }}
              disabled={isLoading}
            />
            <TextField
              label="Children"
              type="number"
              value={formData.childCount}
              onChange={(e) => handleFieldChange("childCount", Math.max(0, Math.min(50, parseInt(e.target.value) || 0)))}
              slotProps={{
                input: {
                  inputProps: {
                    min: 0,
                    max: 50,
                    inputMode: "numeric", // Numeric keyboard shows on mobile
                    pattern: "[0-9]*"
                  }
                }
              }}
              disabled={isLoading}
            />
            <TextField
              label="Seniors"
              type="number"
              value={formData.seniorCount}
              onChange={(e) => handleFieldChange("seniorCount", Math.max(0, Math.min(50, parseInt(e.target.value) || 0)))}
              slotProps={{
                input: {
                  inputProps: {
                    min: 0,
                    max: 50,
                    inputMode: "numeric", // Numeric keyboard shows on mobile
                    pattern: "[0-9]*"
                  }
                }
              }}
              disabled={isLoading}
            />
          </Box>

          {/* Total Guests Summary */}
          <Box sx={{ mt: 2, p:2, bgcolor: "background.paper", borderRadius: 1, border: 1, borderColor: "divider", textAlign: "center" }}
          >
            <Typography variant="subtitle1" sx={{ fontWeight: "600" }}>Total Guests: {totalGuests}</Typography>
          </Box>
        </Box>
      </DialogContent>
      
      <DialogActions sx={{ px: 3, py: 2 }}>
        <Button onClick={handleClose} disabled={isLoading}>Cancel</Button>
        <Button
        onClick={() => {
            console.log("Create Bill button clicked");
            handleSubmit();
          }}
          variant="contained"
          disabled={isSubmitDisabled}
          startIcon={isLoading && <CircularProgress size={16}/>}
        >
          {isLoading ? "Creating..." : "Create Bill"}
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default NewBillDialog;
