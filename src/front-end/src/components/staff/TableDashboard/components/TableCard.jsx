import { useState } from "react";
import {
  Card,
  CardContent,
  Box,
  Typography,
  IconButton,
  Chip,
  Collapse,
  Button,
} from "@mui/material";
import { Edit, Trash2, ChevronDown, ChevronUp, Users } from "lucide-react";
import { styled } from "@mui/material/styles";
import { useTable } from "../context/TableContext";
import api from "@/config/api";

const StyledCard = styled(Card, {
  shouldForwardProp: (prop) => prop !== "inUse",
})(({ theme, inUse }) => ({
  marginBottom: theme.spacing(2),
  backgroundColor: theme.palette.background.paper,
  transition: "all 0.3s ease",
  "&:hover": {
    boxShadow: theme.shadows[4],
  },
}));

const TableCard = ({ table }) => {
  const { openDialog, refreshData, setActionError } = useTable();
  const [expanded, setExpanded] = useState(false);
  const [isCheckingSession, setIsCheckingSession] = useState(false);

  const checkIfInUse = async () => {
    setIsCheckingSession(true);
    try {
      const response = await api.post(
        `/TableEntity/${table.table_Id}/active-session`
      );
      return !response.data.success;
    } catch (error) {
      console.error("Error checking table status:", error);
      return false;
    } finally {
      setIsCheckingSession(false);
    }
  };

  const handleToggleStatus = async () => {
    try {
      const inUse = await checkIfInUse();
      if (inUse) {
        setActionError(
          `Cannot ${
            table.is_active ? "deactivate" : "activate"
          } table that is currently in use`
        );
        return;
      }

      await api.post(`/TableEntity/${table.table_Id}/toggle-status`);
      await refreshData();
    } catch (error) {
      setActionError("Failed to toggle table status");
    }
  };

  const handleDelete = async () => {
    if (window.confirm("Are you sure you want to delete this table?")) {
      try {
        await api.delete(`/TableEntity/${table.table_Id}`);
        await refreshData();
      } catch (error) {
        setActionError(
          error.response?.data?.message || "Failed to delete table"
        );
      }
    }
  };

  const handleRemoveFromGroup = async () => {
    if (
      window.confirm(
        "Are you sure you want to remove this table from its group?"
      )
    ) {
      try {
        await api.delete(`/TableEntity/${table.table_Id}/remove-from-group`);
        await refreshData();
      } catch (error) {
        setActionError(
          error.response?.data?.message || "Failed to remove table from group"
        );
      }
    }
  };

  return (
    <StyledCard inUse={!table.is_active || table.tableGroup_Id}>
      <CardContent>
        <Box
          display="flex"
          justifyContent="space-between"
          alignItems="flex-start"
        >
          <Box flex={1}>
            <Box display="flex" alignItems="center" gap={1} mb={1}>
              <Typography variant="h6">Table {table.table_number}</Typography>
              <Chip
                label={table.is_active ? "Active" : "Inactive"}
                color={table.is_active ? "success" : "default"}
                size="small"
              />
              {table.tableGroup_Id && (
                <Chip label="In Group" color="info" size="small" />
              )}
            </Box>

            <Box display="flex" alignItems="center" gap={1}>
              <Users size={16} />
              <Typography variant="body2" color="text.secondary">
                {table.seat_count} seats
              </Typography>
            </Box>
          </Box>

          <Box display="flex" gap={1}>
            <IconButton
              size="small"
              onClick={() => openDialog("editTable", table)}
            >
              <Edit size={18} />
            </IconButton>
            <IconButton size="small" onClick={handleDelete} color="error">
              <Trash2 size={18} />
            </IconButton>
            <IconButton size="small" onClick={() => setExpanded(!expanded)}>
              {expanded ? <ChevronUp size={18} /> : <ChevronDown size={18} />}
            </IconButton>
          </Box>
        </Box>

        <Collapse in={expanded}>
          <Box mt={2} display="flex" flexDirection="column" gap={1}>
            {table.qr_Code && (
              <Typography variant="body2">QR Code: {table.qr_Code}</Typography>
            )}

            {table.tableGroup && (
              <Box>
                <Typography variant="body2" color="text.secondary">
                  Group: {table.tableGroup.group_Name}
                </Typography>
                <Button
                  size="small"
                  variant="outlined"
                  color="warning"
                  onClick={handleRemoveFromGroup}
                  sx={{ mt: 1 }}
                >
                  Remove from Group
                </Button>
              </Box>
            )}

            <Button
              variant="outlined"
              size="small"
              onClick={handleToggleStatus}
              disabled={isCheckingSession}
              sx={{ mt: 1 }}
            >
              {table.is_active ? "Deactivate" : "Activate"}
            </Button>
          </Box>
        </Collapse>
      </CardContent>
    </StyledCard>
  );
};

export default TableCard;
