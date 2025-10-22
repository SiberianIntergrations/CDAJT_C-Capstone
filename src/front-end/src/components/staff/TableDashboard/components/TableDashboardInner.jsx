import { Box, Alert, CircularProgress } from "@mui/material";
import { useTable } from "../context/TableContext";
import TableSummary from "./TableSummary";
import TableList from "./TableList";
import DialogContainer from "./dialogs/DialogContainer";

const TableDashboardInner = () => {
  const { isLoading, error, actionError } = useTable();

  if (isLoading) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight="50vh"
      >
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      {(error || actionError) && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error || actionError}
        </Alert>
      )}
      <TableSummary />
      <TableList />
      <DialogContainer />
    </Box>
  );
};

export default TableDashboardInner;
