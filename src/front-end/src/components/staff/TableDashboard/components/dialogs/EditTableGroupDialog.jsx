import { useState, useEffect } from "react";
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

const EditTableGroupDialog = ({ open, group }) => {
  const { closeDialog, refreshData } = useTable();
  const [formData, setFormData] = useState({
    group_Name: "",
  });
  const [error, setError] = useState(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    if (group) {
      setFormData({
        group_Name: group.group_Name || "",
      });
    }
  }, [group]);

  const handleClose = () => {
    setError(null);
    closeDialog();
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsSubmitting(true);
    setError(null);

    try {
      await api.put(`/TableGroup/${group.tableGroup_Id}`, {
        Group_Name: formData.group_Name,
      });
      await refreshData();
      handleClose();
    } catch (err) {
      setError(err.response?.data?.message || "Failed to update table group");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <form onSubmit={handleSubmit}>
        <DialogTitle>Edit Table Group</DialogTitle>
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
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>Cancel</Button>
          <Button type="submit" variant="contained" disabled={isSubmitting}>
            Save Changes
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  );
};

export default EditTableGroupDialog;
