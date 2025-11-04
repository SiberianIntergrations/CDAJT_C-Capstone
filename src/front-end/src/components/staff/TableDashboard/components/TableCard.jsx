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
import { Edit, Trash2, ChevronDown, ChevronUp, Users,LocateFixed } from "lucide-react";
import { styled } from "@mui/material/styles";
import { useTable } from "../context/TableContext";
import api from "@/config/api";

const CardWrapper = styled("div")({
  position: "relative",
  marginBottom: 16,
});

const ConfirmOverlay = styled(Box, {
  shouldForwardProp: (prop) => prop !== "show",
})(({ theme, show }) => ({
  position: "absolute",
  top: 0,
  left: 0,
  width: "100%",
  height: "100%",
  display: show ? "flex" : "none",
  alignItems: "center",
  backgroundColor: "#C01E2E",
  color: "#ffffff",
  zIndex: 2,
  borderRadius: theme.shape.borderRadius,
  transition: "all 0.3s ease-out",
}));

const StyledCard = styled(Card, {
  shouldForwardProp: (prop) => prop !== "inUse" && prop !== "dimmed",
})(({ theme, inUse, dimmed }) => ({
  marginBottom: 0,
  backgroundColor: theme.palette.background.paper,
  transition: "all 0.3s ease",
  opacity: dimmed ? 0.5 : 1,
  pointerEvents: dimmed ? "none" : "auto",
  "&:hover": {
    boxShadow: dimmed ? theme.shadows[2] : theme.shadows[4],
  },
}));

const TableCard = ({ table }) => {
  const { openDialog, refreshData, setActionError } = useTable();
  const [expanded, setExpanded] = useState(false);
  const [isCheckingSession, setIsCheckingSession] = useState(false);
  const [confirmAction, setConfirmAction] = useState(null);
  const [isProcessing, setIsProcessing] = useState(false);

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

  const handleDeleteClick = (e) => {
    e.stopPropagation();
    setExpanded(false);
    setConfirmAction("delete");
  };

  const handleRemoveFromGroupClick = (e) => {
    e.stopPropagation();
    setConfirmAction("removeFromGroup");
  };

  const handleCancel = (e) => {
    e.stopPropagation();
    setConfirmAction(null);
    setIsProcessing(false);
  };

  const handleConfirmDelete = async (e) => {
    e.stopPropagation();
    if (isProcessing) return;

    setIsProcessing(true);
    try {
      await api.delete(`/TableEntity/${table.table_Id}`);
      await refreshData();
      setConfirmAction(null);
    } catch (error) {
      setActionError(error.response?.data?.message || "Failed to delete table");
      setConfirmAction(null);
    } finally {
      setIsProcessing(false);
    }
  };

  const handleConfirmRemoveFromGroup = async (e) => {
    e.stopPropagation();
    if (isProcessing) return;

    setIsProcessing(true);
    try {
      await api.delete(`/TableEntity/${table.table_Id}/remove-from-group`);
      await refreshData();
      setConfirmAction(null);
    } catch (error) {
      setActionError(
        error.response?.data?.message || "Failed to remove table from group"
      );
      setConfirmAction(null);
    } finally {
      setIsProcessing(false);
    }
  };

  const getConfirmMessage = () => {
    if (confirmAction === "delete") {
      return `Delete Table ${table.table_number}?`;
    }
    if (confirmAction === "removeFromGroup") {
      return `Remove Table ${table.table_number} from group?`;
    }
    return "";
  };

  const handleConfirm = (e) => {
    if (confirmAction === "delete") {
      handleConfirmDelete(e);
    } else if (confirmAction === "removeFromGroup") {
      handleConfirmRemoveFromGroup(e);
    }
  };

  return (
<<<<<<< HEAD
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
            <Box display = "flex" alignItems="center" gap={1}>
              <LocateFixed size={16} />
              <Typography variant="body2" color="text.secondary">
                {table.location.name}
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

=======
    <CardWrapper>
      {confirmAction && (
        <ConfirmOverlay show={confirmAction}>
          <Box
            sx={{
              px: 3,
              width: "100%",
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
              gap: 2,
            }}
            onClick={(e) => e.stopPropagation()}
          >
>>>>>>> main
            <Button
              variant="outlined"
              onClick={handleCancel}
              disabled={isProcessing}
              sx={{
                borderColor: "white",
                color: "white",
                "&:hover": {
                  borderColor: "white",
                  backgroundColor: "rgba(255, 255, 255, 0.1)",
                },
              }}
            >
              Cancel
            </Button>
            <Typography
              sx={{
                textAlign: "center",
                color: "white",
                flexGrow: 1,
              }}
            >
              {getConfirmMessage()}
            </Typography>
            <Button
              variant="contained"
              onClick={handleConfirm}
              disabled={isProcessing}
              sx={{
                bgcolor: "white",
                color: "#C01E2E",
                "&:hover": {
                  bgcolor: "rgba(255, 255, 255, 0.9)",
                },
              }}
            >
              {isProcessing ? "Processing..." : "Confirm"}
            </Button>
          </Box>
        </ConfirmOverlay>
      )}

      <StyledCard
        inUse={!table.is_active || table.tableGroup_Id}
        dimmed={!!confirmAction}
      >
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
              <IconButton
                size="small"
                onClick={handleDeleteClick}
                color="error"
              >
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
                <Typography variant="body2">
                  QR Code: {table.qr_Code}
                </Typography>
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
                    onClick={handleRemoveFromGroupClick}
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
    </CardWrapper>
  );
};

export default TableCard;
