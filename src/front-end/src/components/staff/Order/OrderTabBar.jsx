import React from "react";
import { Box, Button } from "@mui/material";

export const TabBar = ({ currentTab, onChange, counts }) => {
  const tabs = [
    { value: "PENDING", label: "Pending", color: "#FF9800" },
    { value: "PROCESSING", label: "Processing", color: "#2196F3" },
    { value: "DELIVERED", label: "Done", color: "#4CAF50" },
  ];

  return (
    <Box
      sx={{
        display: "flex",
        gap: 1,
        p: 2,
        bgcolor: "background.paper",
        overflowX: "auto",
      }}
    >
      {tabs.map((tab) => (
        <Button
          key={tab.value}
          onClick={() => onChange(tab.value)}
          variant={currentTab === tab.value ? "contained" : "outlined"}
          sx={{
            flex: 1,
            minWidth: "100px",
            bgcolor: currentTab === tab.value ? tab.color : "transparent",
            color: currentTab === tab.value ? "white" : tab.color,
            borderColor: tab.color,
            "&:hover": {
              bgcolor: currentTab === tab.value ? tab.color : `${tab.color}15`,
            },
          }}
        >
          {tab.label} ({counts[tab.value] || 0})
        </Button>
      ))}
    </Box>
  );
};