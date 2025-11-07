import React from "react";
import { Box, Typography, Button } from "@mui/material";
import { RefreshCw } from "lucide-react";

export const Header = ({ onRefresh, isLoading }) => {
  return (
    <Box
      sx={{
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        p: 2,
        borderBottom: "1px solid",
        borderColor: "divider",
        position: "sticky",
        top: 0,
        bgcolor: "background.paper",
        zIndex: 10,
      }}
    >
      <Typography variant="h5">
        Order Management
      </Typography>
      <Button startIcon={<RefreshCw/>}onClick={onRefresh} disabled={isLoading} color="primary">
        Refresh
      </Button>
    </Box>
  );
};