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

const NewTableGroupDialog = ({ open }) => {
  const { closeDialog, refreshData } = useTable();
  const [formData, setFormData] = useState({
    group_Name: "",
    is_Active: true,
  });
  const [error, setError] = useState(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleClose = () => {
    setFormData({
      group_Name: "",
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
      await api.post("/TableGroup", {
        Group_Name: formData.group_Name,
        Is_Active: formData.is_Active,
      });
      await refreshData();
      handleClose();
    } catch (err) {
      setError(err.response?.data?.message || "Failed to create table group");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <form onSubmit={handleSubmit}>
        <DialogTitle>Create New Table Group</DialogTitle>
        <DialogContent>
          {error && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {error}
            </Alert>
          )}
          <Box display="flex" flexDirection="column" gap={2} mt={1}>
            <TextField
              label="Group Name"
              required
              value={formData.group_Name}
              onChange={(e) =>
                setFormData({ ...formData, group_Name: e.target.value })
              }
              placeholder="e.g., Large Party Area"
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>Cancel</Button>
          <Button type="submit" variant="contained" disabled={isSubmitting}>
            Create Group
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  );
};

export default NewTableGroupDialog;
