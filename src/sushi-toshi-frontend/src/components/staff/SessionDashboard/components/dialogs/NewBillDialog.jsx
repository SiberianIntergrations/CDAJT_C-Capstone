import { useState } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Box,
  TextField,
  IconButton,
} from "@mui/material";
import { X } from "lucide-react";
import { useSession } from "../../context/SessionContext";

const NewBillDialog = ({ open, sessionId, onClose }) => {
  const { createBill } = useSession();
  const [billData, setBillData] = useState({
    billName: "",
    adultCount: 0,
    childCount: 0,
    seniorCount: 0,
    totCount: 0,
  });
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async () => {
    if (!sessionId || isSubmitting) return;

    try {
      setIsSubmitting(true);
      console.log("Submitting bill data:", billData);
      const success = await createBill(sessionId, billData);
      console.log("Bill creation result:", success);

      if (success) {
        setBillData({
          billName: "",
          adultCount: 0,
          childCount: 0,
          seniorCount: 0,
          totCount: 0,
        });
        console.log("Closing dialog with success");
        onClose(true);
      }
    } catch (error) {
      console.error("Error in handleSubmit:", error);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleCancel = () => {
    setBillData({
      billName: "",
      adultCount: 0,
      childCount: 0,
      seniorCount: 0,
      totCount: 0,
    });
    onClose(false);
  };

  const isValid = () => {
    return Boolean(
      billData.billName &&
        (billData.adultCount > 0 ||
          billData.childCount > 0 ||
          billData.seniorCount > 0 ||
          billData.totCount > 0)
    );
  };

  return (
    <Dialog open={open} onClose={handleCancel} fullWidth>
      <DialogTitle>
        Create New Bill
        <IconButton
          onClick={onClose}
          sx={{ position: "absolute", right: 8, top: 8 }}
        >
          <X />
        </IconButton>
      </DialogTitle>
      <DialogContent>
        <Box sx={{ mt: 2, display: "flex", flexDirection: "column", gap: 2 }}>
          <TextField
            label="Bill Name"
            value={billData.billName}
            onChange={(e) =>
              setBillData((prev) => ({ ...prev, billName: e.target.value }))
            }
            fullWidth
            placeholder="e.g., Table 1 - Party of 4"
          />

          <Box sx={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 2 }}>
            <TextField
              label="Adults"
              type="number"
              value={billData.adultCount}
              onChange={(e) =>
                setBillData((prev) => ({
                  ...prev,
                  adultCount: parseInt(e.target.value) || 0,
                }))
              }
              InputProps={{ inputProps: { min: 0 } }}
            />
            <TextField
              label="Children"
              type="number"
              value={billData.childCount}
              onChange={(e) =>
                setBillData((prev) => ({
                  ...prev,
                  childCount: parseInt(e.target.value) || 0,
                }))
              }
              InputProps={{ inputProps: { min: 0 } }}
            />
            <TextField
              label="Seniors"
              type="number"
              value={billData.seniorCount}
              onChange={(e) =>
                setBillData((prev) => ({
                  ...prev,
                  seniorCount: parseInt(e.target.value) || 0,
                }))
              }
              InputProps={{ inputProps: { min: 0 } }}
            />
            <TextField
              label="Tots"
              type="number"
              value={billData.totCount}
              onChange={(e) =>
                setBillData((prev) => ({
                  ...prev,
                  totCount: parseInt(e.target.value) || 0,
                }))
              }
              InputProps={{ inputProps: { min: 0 } }}
            />
          </Box>
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleCancel}>Cancel</Button>
        <Button
          onClick={handleSubmit}
          variant="contained"
          disabled={!isValid()}
        >
          Create Bill
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default NewBillDialog;
