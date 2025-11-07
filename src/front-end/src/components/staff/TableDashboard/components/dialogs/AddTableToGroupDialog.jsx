import { useState, useEffect } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Box,
  Alert,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Typography,
  Chip,
} from "@mui/material";
import { useTable } from "../../context/TableContext";
import api from "@/config/api";

const AddTableToGroupDialog = ({ open, group }) => {
  const { closeDialog, refreshData, tables } = useTable();
  const [selectedTableId, setSelectedTableId] = useState("");
  const [availableTables, setAvailableTables] = useState([]);
  const [error, setError] = useState(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    if (open && group) {
      // Filter tables that are not in any group and are active
      const available = tables.filter((t) => !t.tableGroup_Id && t.is_active);

      // Filter tables in active sessions
      const filterTables = async () => {
        const filtered = await Promise.all(
          available.map(async (table) => {
            try {
              const response = await api.post(
                `/TableEntity/${table.table_Id}/active-session`
              );
              return response.data.success ? table : null;
            } catch (error) {
              console.error(error);
              return null;
            }
          })
        );
        setAvailableTables(filtered.filter((table) => table !== null));
      };

      filterTables();
    }
  }, [open, group, tables]);

  const handleClose = () => {
    setSelectedTableId("");
    setError(null);
    closeDialog();
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsSubmitting(true);
    setError(null);

    try {
      await api.post(
        `/TableGroup/${group.tableGroup_Id}/tables/${selectedTableId}`
      );
      await refreshData();
      handleClose();
    } catch (err) {
      setError(err.response.data || "Failed to add table to group");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <form onSubmit={handleSubmit}>
        <DialogTitle>Add Table to {group?.group_Name}</DialogTitle>
        <DialogContent>
          {error && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {error}
            </Alert>
          )}
          <Box display="flex" flexDirection="column" gap={2} mt={1}>
            {availableTables.length === 0 ? (
              <Alert severity="info">
                No available tables. All tables are either inactive or already
                assigned to a group.
              </Alert>
            ) : (
              <>
                <Typography variant="body2" color="text.secondary">
                  Select a table to add to this group
                </Typography>
                <FormControl fullWidth required>
                  <InputLabel>Table</InputLabel>
                  <Select
                    value={selectedTableId}
                    onChange={(e) => setSelectedTableId(e.target.value)}
                    label="Table"
                  >
                    {availableTables.map((table) => (
                      <MenuItem key={table.table_Id} value={table.table_Id}>
                        <Box
                          sx={{
                            display: "flex",
                            alignItems: "center",
                            gap: 1,
                          }}
                        >
                          Table {table.table_number}
                          <Chip
                            label={`${table.seat_count} seats`}
                            size="small"
                            variant="outlined"
                          />
                          {table.tableGroup_Name && (
                            <Chip
                              label={table.tableGroup_Name}
                              size="small"
                              color="primary"
                              variant="outlined"
                            />
                          )}
                        </Box>
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </>
            )}
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>Cancel</Button>
          <Button
            type="submit"
            variant="contained"
            disabled={
              isSubmitting || availableTables.length === 0 || !selectedTableId
            }
          >
            Add Table
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  );
};

export default AddTableToGroupDialog;
