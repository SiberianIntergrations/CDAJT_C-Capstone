import { useState, useEffect } from "react";
import {
  Card,
  CardContent,
  Box,
  Typography,
  IconButton,
  Chip,
  Collapse,
  Button,
  List,
  ListItem,
  ListItemText,
} from "@mui/material";
import {
  Edit,
  Trash2,
  ChevronDown,
  ChevronUp,
  Users,
  Plus,
  LocateFixed,
} from "lucide-react";
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
  shouldForwardProp: (prop) => prop !== "dimmed",
})(({ theme, dimmed }) => ({
  marginBottom: 0,
  backgroundColor: theme.palette.background.paper,
  transition: "all 0.3s ease",
  opacity: dimmed ? 0.5 : 1,
  pointerEvents: dimmed ? "none" : "auto",
  "&:hover": {
    boxShadow: dimmed ? theme.shadows[2] : theme.shadows[4],
  },
}));

const TableGroupCard = ({ group }) => {
  const { openDialog, refreshData, setActionError } = useTable();
  const [expanded, setExpanded] = useState(false);
  const [confirmAction, setConfirmAction] = useState(null);
  const [isProcessing, setIsProcessing] = useState(false);
  const [targetTableId, setTargetTableId] = useState(null);
  const [isInUse, setIsInUse] = useState(false);

  useEffect(() => {
    const checkInSession = async () => {
      try {
        const response = await api.get(
          `/TableGroup/${group.tableGroup_Id}/active-session`
        );
        setIsInUse(!response.data.success);
        // await refreshData();
      } catch (error) {
        console.error(error);
        setActionError("Session check failed");
      }
    };
    checkInSession();
  }, []);

  const totalSeats =
    group.tables?.reduce((sum, t) => sum + t.seat_count, 0) || 0;

  const handleToggleStatus = async () => {
    try {
      await api.post(`/TableGroup/${group.tableGroup_Id}/toggle-status`);
      await refreshData();
    } catch (error) {
      setActionError(error.response.data || "Failed to toggle group status");
    }
  };

  const handleDeleteClick = (e) => {
    e.stopPropagation();
    setExpanded(false);
    setConfirmAction("delete");
  };

  const handleRemoveTableClick = (tableId, e) => {
    e.stopPropagation();
    setTargetTableId(tableId);
    setConfirmAction("removeTable");
  };

  const handleCancel = (e) => {
    e.stopPropagation();
    setConfirmAction(null);
    setTargetTableId(null);
    setIsProcessing(false);
  };

  const handleConfirmDelete = async (e) => {
    e.stopPropagation();
    if (isProcessing) return;

    setIsProcessing(true);
    try {
      await api.delete(`/TableGroup/${group.tableGroup_Id}`);
      await refreshData();
      setConfirmAction(null);
    } catch (error) {
      window.scrollTo({
        top: 0,
        behavior: "smooth",
      });
      setActionError(error.response.data || "Failed to delete table group");
      setConfirmAction(null);
    } finally {
      setIsProcessing(false);
    }
  };

  const handleConfirmRemoveTable = async (e) => {
    e.stopPropagation();
    if (isProcessing) return;

    setIsProcessing(true);
    try {
      await api.delete(
        `/TableGroup/${group.tableGroup_Id}/tables/${targetTableId}`
      );
      await refreshData();
      setConfirmAction(null);
      setTargetTableId(null);
    } catch (error) {
      setActionError(
        error.response.data || "Failed to remove table from group"
      );
      setConfirmAction(null);
      setTargetTableId(null);
    } finally {
      setIsProcessing(false);
    }
  };

  const getConfirmMessage = () => {
    if (confirmAction === "delete") {
      return `Delete group "${group.group_Name}"?`;
    }
    if (confirmAction === "removeTable") {
      const table = group.tables?.find((t) => t.table_Id === targetTableId);
      return `Remove Table ${table?.table_number || ""} from group?`;
    }
    return "";
  };

  const handleConfirm = (e) => {
    if (confirmAction === "delete") {
      handleConfirmDelete(e);
    } else if (confirmAction === "removeTable") {
      handleConfirmRemoveTable(e);
    }
  };

  return (
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

      <StyledCard dimmed={!!confirmAction}>
        <CardContent>
          <Box
            display="flex"
            justifyContent="space-between"
            alignItems="flex-start"
          >
            <Box flex={1}>
              <Box display="flex" alignItems="center" gap={1} mb={1}>
                <Typography variant="h6">{group.group_Name}</Typography>
                <Chip
                  label={group.is_Active ? "Active" : "Inactive"}
                  color={group.is_Active ? "success" : "default"}
                  size="small"
                />
                {isInUse && (
                  <Chip label="In Session" color="error" size="small" />
                )}
              </Box>

              <Box display="flex" alignItems="center" gap={2}>
                <Box display="flex" alignItems="center" gap={1}>
                  <Users size={16} />
                  <Typography variant="body2" color="text.secondary">
                    {totalSeats} total seats
                  </Typography>
                </Box>
                <Typography variant="body2" color="text.secondary">
                  {group.table_Count || 0} tables
                </Typography>
              </Box>
            </Box>

            <Box display="flex" gap={1}>
              <IconButton
                size="small"
                onClick={() => openDialog("editTableGroup", group)}
              >
                <Edit size={18} />
              </IconButton>
              <IconButton
                size="small"
                onClick={() => openDialog("addTableToGroup", group)}
              >
                <Plus size={18} />
              </IconButton>
              <IconButton
                size="small"
                onClick={handleDeleteClick}
                color="error"
                disabled={isInUse}
              >
                <Trash2 size={18} />
              </IconButton>
              <IconButton size="small" onClick={() => setExpanded(!expanded)}>
                {expanded ? <ChevronUp size={18} /> : <ChevronDown size={18} />}
              </IconButton>
            </Box>
          </Box>

          <Collapse in={expanded}>
            <Box mt={2}>
              {group.tables && group.tables.length > 0 ? (
                <List dense>
                  {group.tables.map((table) => (
                    <ListItem
                      key={table.table_Id}
                      secondaryAction={
                        <IconButton
                          edge="end"
                          size="small"
                          onClick={(e) =>
                            handleRemoveTableClick(table.table_Id, e)
                          }
                        >
                          <Trash2 size={16} />
                        </IconButton>
                      }
                    >
                      <ListItemText
                        primary={`Table ${table.table_number}`}
                        secondary={`${table.seat_count} seats`}
                      />
                    </ListItem>
                  ))}
                </List>
              ) : (
                <Typography variant="body2" color="text.secondary">
                  No tables in this group
                </Typography>
              )}

              <Button
                variant="outlined"
                size="small"
                onClick={handleToggleStatus}
                sx={{ mt: 1 }}
                fullWidth
                disabled={isInUse}
              >
                {group.is_Active ? "Deactivate" : "Activate"}
              </Button>
            </Box>
          </Collapse>
        </CardContent>
      </StyledCard>
    </CardWrapper>
  );
};

export default TableGroupCard;
