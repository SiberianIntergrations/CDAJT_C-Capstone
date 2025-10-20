import { useState } from "react";
import {
  Box,
  Typography,
  Button,
  Chip,
  Collapse,
  Alert,
  Dialog,
  Stack,
  CircularProgress,
} from "@mui/material";
import { TableIcon, AlertTriangle } from "lucide-react";
import { useSession } from "../../context/SessionContext";
import api from "@/config/api";

const TableSection = ({ session }) => {
  const { openDialog, fetchSessions } = useSession();
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);
  const [removedTableIds, setRemovedTableIds] = useState(new Set());
  const [confirmDialogOpen, setConfirmDialogOpen] = useState(false);
  const [tableToRemove, setTableToRemove] = useState(null);
  const [confirmLoading, setConfirmLoading] = useState(false);

  const initiateTableRemoval = (sessionId, tableNum) => {
    setTableToRemove({ sessionId, tableNum });
    setConfirmDialogOpen(true);
  };

  const handleRemoveTable = async () => {
    if (!tableToRemove) return;

    const { sessionId, tableNum } = tableToRemove;
    setConfirmLoading(true);
    setError(null);

    try {
      const tablesResponse = await api.get("/Table");

      if (tablesResponse.status !== 200) {
        throw new Error("Failed to fetch tables");
      }

      const tables = tablesResponse.data;
      const table = tables.find((t) => t.table_number === tableNum);

      if (!table) {
        throw new Error("Table not found");
      }

      // TODO: Update endpoint (api from old project: /dining-sessions/{sessionId}/tables/{tableId})
      const response = await api.delete(
        `/dining-sessions/${sessionId}/tables/${table.table_id}`
      );

      if (response.status === 204) {
        setRemovedTableIds((prev) => new Set([...prev, tableNum]));
        await fetchSessions();
        setConfirmDialogOpen(false);
        setTableToRemove(null);
      } else {
        throw new Error(response.data?.detail || "Failed to remove table");
      }
    } catch (err) {
      console.error("Error removing table:", err);
      setError(err.message || "Failed to remove table. Please try again.");
    } finally {
      setConfirmLoading(false);
    }
  };

  const displayedTables = session.table_numbers.filter(
    (tableNum) => !removedTableIds.has(tableNum)
  );

  return (
    <Box sx={{ mt: 4 }}>
      <Collapse in={Boolean(error)}>
        <Alert severity="error" onClose={() => setError(null)} sx={{ mb: 2 }}>
          {error}
        </Alert>
      </Collapse>

      <Box
        sx={{
          display: "flex",
          justifyContent: "space-between",
          mb: 2,
          alignItems: "center",
        }}
      >
        <Typography
          variant="subtitle2"
          sx={{
            display: "flex",
            alignItems: "center",
            gap: 1,
          }}
        >
          <TableIcon className="w-4 h-4" />
          Tables ({displayedTables.length})
        </Typography>

        <Button
          size="small"
          variant="contained"
          onClick={() => openDialog("addTable", session.session_id)}
          disabled={session.bills.length > 0 || loading}
          sx={{
            minWidth: 100,
            backgroundColor: "primary.main",
            "&:hover": {
              backgroundColor: "primary.dark",
            },
          }}
        >
          Add Table
        </Button>
      </Box>

      <Box
        sx={{
          display: "flex",
          gap: 1,
          flexWrap: "wrap",
        }}
      >
        {displayedTables.map((tableNum) => (
          <Chip
            key={tableNum}
            label={`Table ${tableNum}`}
            onDelete={
              session.bills.length === 0
                ? () => initiateTableRemoval(session.session_id, tableNum)
                : undefined
            }
            size="small"
            disabled={loading}
            sx={{
              backgroundColor: "grey.100",
              "&:hover": {
                backgroundColor: "grey.200",
              },
            }}
          />
        ))}
      </Box>

      <Dialog
        open={confirmDialogOpen}
        onClose={() => !confirmLoading && setConfirmDialogOpen(false)}
      >
        <Box sx={{ p: 3 }}>
          <Stack direction="row" spacing={1} alignItems="center" mb={2}>
            <AlertTriangle color="error" />
            <Typography variant="h6">Remove Table</Typography>
          </Stack>

          <Typography mb={3}>
            Are you sure you want to remove Table {tableToRemove?.tableNum}?
            This action cannot be undone.
          </Typography>

          <Stack direction="row" spacing={2} justifyContent="flex-end">
            <Button
              onClick={() => setConfirmDialogOpen(false)}
              disabled={confirmLoading}
            >
              Cancel
            </Button>
            <Button
              variant="contained"
              color="error"
              onClick={handleRemoveTable}
              disabled={confirmLoading}
              startIcon={confirmLoading && <CircularProgress size={20} />}
            >
              {confirmLoading ? "Removing..." : "Remove Table"}
            </Button>
          </Stack>

          {error && (
            <Alert severity="error" sx={{ mt: 2 }}>
              {error}
            </Alert>
          )}
        </Box>
      </Dialog>
    </Box>
  );
};

export default TableSection;
