import {
  Box,
  Card,
  Typography,
  IconButton,
  Collapse,
  Stack,
} from "@mui/material";
import { Plus, ChevronDown, ChevronUp } from "lucide-react";
import { styled } from "@mui/material/styles";
import { useTable } from "../context/TableContext";
import { useState } from "react";

const CompactCard = styled(Card)(({ theme }) => ({
  backgroundColor: theme.palette.common.white,
  minHeight: "80px",
  display: "flex",
  alignItems: "center",
  padding: theme.spacing(1.5),
}));

const AddButton = styled(Box)(({ theme }) => ({
  backgroundColor: theme.palette.common.white,
  border: `2px solid ${theme.palette.primary.main}`,
  borderRadius: theme.spacing(1),
  padding: theme.spacing(2),
  cursor: "pointer",
  transition: "all 0.2s ease-in-out",
  "&:hover": {
    backgroundColor: theme.palette.primary.main,
    color: "white",
    "& .MuiTypography-root": {
      color: "white",
    },
    "& svg": {
      color: "white",
    },
  },
}));

const OptionButton = styled(Box)(({ theme }) => ({
  backgroundColor: theme.palette.common.white,
  border: `2px dashed ${theme.palette.divider}`,
  borderRadius: theme.spacing(1),
  padding: theme.spacing(2),
  cursor: "pointer",
  transition: "all 0.2s ease-in-out",
  "&:hover": {
    borderColor: theme.palette.primary.main,
    backgroundColor: theme.palette.action.hover,
    transform: "translateX(4px)",
  },
}));

const TableSummary = () => {
  const { tableSummary, openDialog } = useTable();
  const [expanded, setExpanded] = useState(false);
  console.log(tableSummary);

  return (
    <Box
      sx={{
        display: "flex",
        flexDirection: "column",
        gap: "1rem",
        width: "100%",
      }}
    >
      <Stack direction="row" justifyContent="left" alignItems="center" mb={2}>
        <Typography pr={3} variant="h4">
          Active Tables and Table Groups:
        </Typography>
      </Stack>

      <Box sx={{ display: "flex", flexDirection: "column", gap: 1.5 }}>
        <AddButton onClick={() => setExpanded(!expanded)}>
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Box sx={{ display: "flex", alignItems: "center", gap: 2 }}>
              <Plus size={24} color="currentColor" />
              <Typography variant="h6" color="text.primary">
                Add New
              </Typography>
            </Box>
            {expanded ? (
              <ChevronUp size={20} color="currentColor" />
            ) : (
              <ChevronDown size={20} color="currentColor" />
            )}
          </Box>
        </AddButton>

        <Collapse in={expanded}>
          <Box
            sx={{ display: "flex", flexDirection: "column", gap: 1.5, pl: 2 }}
          >
            <OptionButton onClick={() => openDialog("newTable")}>
              <Box sx={{ display: "flex", alignItems: "center", gap: 2 }}>
                <Plus size={20} />
                <Box>
                  <Typography variant="subtitle1" color="text.primary">
                    Individual Table
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    Create a single table
                  </Typography>
                </Box>
              </Box>
            </OptionButton>

            <OptionButton onClick={() => openDialog("newTableGroup")}>
              <Box sx={{ display: "flex", alignItems: "center", gap: 2 }}>
                <Plus size={20} />
                <Box>
                  <Typography variant="subtitle1" color="text.primary">
                    Table Group
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    Create a group of tables
                  </Typography>
                </Box>
              </Box>
            </OptionButton>
          </Box>
        </Collapse>
      </Box>
    </Box>
  );
};

export default TableSummary;
