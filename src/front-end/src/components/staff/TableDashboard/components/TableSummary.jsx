import { Box, Card, Typography, IconButton } from "@mui/material";
import { Plus } from "lucide-react";
import { styled } from "@mui/material/styles";
import { useTable } from "../context/TableContext";

const CompactCard = styled(Card)(({ theme }) => ({
  backgroundColor: theme.palette.common.white,
  minHeight: "80px",
  display: "flex",
  alignItems: "center",
  padding: theme.spacing(1.5),
}));

const AddButton = styled(IconButton)(({ theme }) => ({
  backgroundColor: theme.palette.primary.main,
  color: "white",
  padding: theme.spacing(1),
  minWidth: "auto",
  "&:hover": {
    backgroundColor: theme.palette.primary.dark,
  },
  "& svg": {
    width: 20,
    height: 20,
  },
}));

const TableSummary = () => {
  const { tableSummary, openDialog } = useTable();

  return (
    <Box className="grid grid-cols-1 sm:grid-cols-2 gap-2 mb-3">
      <CompactCard>
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
            width: "100%",
          }}
        >
          <Box sx={{ flex: 1 }}>
            <Box display="flex" alignItems="center" gap={2}>
              <Typography variant="h5" color="primary.dark">
                Tables in Use: {tableSummary.tables_in_use}
              </Typography>
              <AddButton onClick={() => openDialog("newTable")}>
                <Plus />
              </AddButton>
            </Box>
          </Box>
        </Box>
      </CompactCard>

      <CompactCard>
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
            width: "100%",
          }}
        >
          <Box sx={{ flex: 1 }}>
            <Box display="flex" alignItems="center" gap={2}>
              <Typography variant="h5" color="secondary">
                Table Groups
              </Typography>
              <AddButton onClick={() => openDialog("newTableGroup")}>
                <Plus />
              </AddButton>
            </Box>
          </Box>
        </Box>
      </CompactCard>
    </Box>
  );
};

export default TableSummary;
