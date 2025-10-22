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
} from "lucide-react";
import { styled } from "@mui/material/styles";
import { useTable } from "../context/TableContext";
import api from "@/config/api";

const StyledCard = styled(Card)(({ theme }) => ({
  marginBottom: theme.spacing(2),
  backgroundColor: theme.palette.background.paper,
  transition: "all 0.3s ease",
  "&:hover": {
    boxShadow: theme.shadows[4],
  },
}));

const TableGroupCard = ({ group }) => {
  const { openDialog, refreshData, setActionError } = useTable();
  const [expanded, setExpanded] = useState(false);

  const totalSeats =
    group.tables?.reduce((sum, t) => sum + t.seat_count, 0) || 0;

  const handleToggleStatus = async () => {
    try {
      await api.post(`/TableGroup/${group.tableGroup_Id}/toggle-status`);
      await refreshData();
    } catch (error) {
      setActionError(
        error.response?.data?.message || "Failed to toggle group status"
      );
    }
  };

  const handleDelete = async () => {
    if (
      window.confirm(
        "Are you sure you want to delete this table group? All tables will be unassigned."
      )
    ) {
      try {
        await api.delete(`/TableGroup/${group.tableGroup_Id}`);
        await refreshData();
      } catch (error) {
        setActionError(
          error.response?.data?.message || "Failed to delete table group"
        );
      }
    }
  };

  const handleRemoveTable = async (tableId) => {
    if (
      window.confirm(
        "Are you sure you want to remove this table from the group?"
      )
    ) {
      try {
        await api.delete(
          `/TableGroup/${group.tableGroup_Id}/tables/${tableId}`
        );
        await refreshData();
      } catch (error) {
        setActionError(
          error.response?.data?.message || "Failed to remove table from group"
        );
      }
    }
  };

  return (
    <StyledCard>
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
            <IconButton size="small" onClick={handleDelete} color="error">
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
                        onClick={() => handleRemoveTable(table.table_Id)}
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
            >
              {group.is_Active ? "Deactivate" : "Activate"}
            </Button>
          </Box>
        </Collapse>
      </CardContent>
    </StyledCard>
  );
};

export default TableGroupCard;
