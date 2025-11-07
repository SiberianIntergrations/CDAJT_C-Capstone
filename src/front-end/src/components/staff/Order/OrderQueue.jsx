import React from "react";
import { Box, Alert } from "@mui/material";
import { OrderCard } from "./OrderCard";

export const OrderQueue = ({ orders, onOrderClick }) => {
  if (orders.length === 0) {
    return (
      <Box sx={{ p: 2 }}>
        <Alert severity="info">No orders to display for this status</Alert>
      </Box>
    );
  }

  return (
    <Box
      sx={{
        p: 2,
        display: "grid",
        gridTemplateColumns: {
          xs: "1fr",
          sm: "repeat(2, 1fr)",
          md: "repeat(3, 1fr)",
          lg: "repeat(4, 1fr)",
        },
        gap: 2,
      }}
    >
      {orders.map((order) => (
        <OrderCard
          key={order.orderId}
          order={order}
          onViewDetails={() => onOrderClick(order.orderId)}
        />
      ))}
    </Box>
  );
};