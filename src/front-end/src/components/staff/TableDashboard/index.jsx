import { Box } from "@mui/material";
import { styled } from "@mui/material/styles";
import { TableProvider } from "./context/TableContext";
import TableDashboardInner from "./components/TableDashboardInner";

const DashboardContainer = styled(Box)(({ theme }) => ({
  padding: theme.spacing(2),
  maxWidth: "600px",
  margin: "0 auto",
  position: "relative",
  zIndex: 1,
  [theme.breakpoints.up("sm")]: {
    padding: theme.spacing(3),
  },
}));

const TableDashboard = () => (
  <TableProvider>
    <DashboardContainer>
      <TableDashboardInner />
    </DashboardContainer>
  </TableProvider>
);

export default TableDashboard;
