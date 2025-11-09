import React from "react";
import { Box, Alert } from "@mui/material";

export const DataGrid = ({ 
  items = [],
  renderItem,
  keyExtractor,
  emptyMessage = "No items found",
  emptyVariant = "info",
  gridColumns = {
    xs: "1fr",
    sm: "repeat(2, 1fr)",
    md: "repeat(3, 1fr)",
    lg: "repeat(4, 1fr)",
  },
  gap = 2,
  sx = {}
}) => {
  if (items.length === 0) {
    return (
      <Box sx={{ p: 2 }}>
        <Alert severity={emptyVariant}>{emptyMessage}</Alert>
      </Box>
    );
  }

  return (
    <Box
      sx={{
        p: 2,
        display: "grid",
        gridTemplateColumns: gridColumns,
        gap,
        ...sx
      }}
    >
      {items.map((item, index) => {
        const key = keyExtractor ? keyExtractor(item, index) : `item-${index}`;
        return (
          <React.Fragment key={key}>
            {renderItem(item, index)}
          </React.Fragment>
        );
      })}
    </Box>
  );
};