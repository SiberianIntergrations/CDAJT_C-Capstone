import React from "react";
import { Box, Typography } from "@mui/material";
import { FileText } from "lucide-react";
import { DataCard } from "@/components/common/DataCard";

export const OrderCard = ({ order, onViewDetails }) => {
  if (!order) return null;

  console.log("Order Data:", order);
  const tableText = order.tableNumbers && order.tableNumbers.length > 0
    ? `Tables ${order.tableNumbers.join(", ")}`
    : `Order #${order.orderId}`;

  const itemsText = `${order.items?.length || 0} item${order.items?.length !== 1 ? 's' : ''}`;

  return (
    <DataCard
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
          {tableText}
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
          {itemsText}
        </Typography>
      </Box>
    </DataCard>
  );
};