import { useState } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Button,
  Box,
  Alert,
} from "@mui/material";
import { useTable } from "../../context/TableContext";
import api from "@/config/api";

const NewTableDialog = ({ open }) => {
  const { closeDialog, refreshData } = useTable();
  const [formData, setFormData] = useState({
    table_Number: "",
    seat_count: "",
    qr_Code_Url: "",
    is_Active: true,
  });
  const [error, setError] = useState(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleClose = () => {
    setFormData({
      table_Number: "",
      seat_count: "",
      qr_Code_Url: "",
      is_Active: true,
    });
    setError(null);
    closeDialog();
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsSubmitting(true);
    setError(null);

    try {
      await api.post("/TableEntity", {
        Table_Number: parseInt(formData.table_Number),
        Seat_Count: parseInt(formData.seat_count),
        Qr_Code_Url: formData.qr_Code_Url,
        Is_Active: formData.is_Active,
      });
      await refreshData();
      handleClose();
    } catch (err) {
      setError(err.response?.data?.message || "Failed to create table");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <form onSubmit={handleSubmit}>
        <DialogTitle>Create New Table</DialogTitle>
        <DialogContent>
          {error && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {error}
            </Alert>
          )}
          <Box display="flex" flexDirection="column" gap={2} mt={1}>
            <TextField
              label="Table Number"
              type="number"
              required
              value={formData.table_Number}
              onChange={(e) =>
                setFormData({ ...formData, table_Number: e.target.value })
              }
            />
            <TextField
              label="Seat Count"
              type="number"
              required
              value={formData.seat_count}
              onChange={(e) =>
                setFormData({ ...formData, seat_count: e.target.value })
              }
            />
            <TextField
              label="QR Code URL (optional)"
              value={formData.qr_Code_Url}
              onChange={(e) =>
                setFormData({ ...formData, qr_Code_Url: e.target.value })
              }
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>Cancel</Button>
          <Button type="submit" variant="contained" disabled={isSubmitting}>
            Create Table
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  );
};

export default NewTableDialog;
