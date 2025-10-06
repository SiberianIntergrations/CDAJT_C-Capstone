import { Box, Alert, CircularProgress } from "@mui/material";
import { useSession } from "../context/SessionContext";
import DashboardSummary from "./DashboardSummary";
import SessionList from "./SessionList";
import DialogContainer from "./dialogs/DialogContainer";

const DashboardContent = () => {
  const { isLoading, error, sessions, actionError } = useSession();

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
      <DashboardSummary />
      <SessionList />
      <DialogContainer />
    </Box>
  );
};

export default DashboardContent;
