import React from "react";
import { Box, Typography, IconButton } from "@mui/material";
import { RefreshCw } from "lucide-react";

export const PageHeader = ({ 
  title, 
  onRefresh, 
  isLoading = false,
  actions = null,
  showRefresh = true,
  sx = {}
}) => {
  return (
    <Box
      sx={{
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        mb: 3,
        p: 2,
        borderBottom: "1px solid",
        borderColor: "divider",
        position: "sticky",
        top: 0,
        zIndex: 10,
        ...sx
      }}
    >
      <Typography variant="h4" component="h1">
        {title}
      </Typography>
      <Box sx={{ display: "flex", gap: 1, alignItems: "center" }}>
        {actions}
        {showRefresh && onRefresh && (
          <IconButton
            onClick={onRefresh}
            disabled={isLoading}
            aria-label="refresh"
            sx={{
              animation: isLoading ? "spin 1s linear infinite" : "none",
              "@keyframes spin": {
                "0%": { transform: "rotate(0deg)" },
                "100%": { transform: "rotate(360deg)" }
              }
            }}
            color="primary"
          >
            <RefreshCw size={20} />
          </IconButton>
        )}
      </Box>
    </Box>
  );
};