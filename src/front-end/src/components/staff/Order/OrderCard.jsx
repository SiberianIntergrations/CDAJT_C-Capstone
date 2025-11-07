import React from "react";
import { Card, Box, Typography } from "@mui/material";
import { FileText } from "lucide-react";

export const OrderCard = ({ order, onViewDetails }) => {
  return (
    <Card
      onClick={onViewDetails}
      sx={{
        cursor: "pointer",
        transition: "all 0.2s",
        "&:hover": {
          transform: "translateY(-2px)",
          boxShadow: 4,
        },
      }}
    >
      <Box sx={{ p: 2 }}>
        {/* Header */}
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            mb: 2,
          }}
        >
        <Typography variant="h6">Order #{order.orderId}</Typography>
        <Typography variant="body2" color="text.secondary">
          {order.tableNumbers && order.tableNumbers.length > 0
            ? `Tables ${order.tableNumbers.join(", ")}`
            : `Order #${order.orderId}`}
        </Typography>
        </Box>

        {/* Items Count */}
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            gap: 0.5,
            mb: 1,
          }}
        >
          <FileText size={16} color="#666" />
          <Typography variant="body2" color="text.secondary">
            {order.items.length} items
          </Typography>
        </Box>
      </Box>
    </Card>
  );
};
