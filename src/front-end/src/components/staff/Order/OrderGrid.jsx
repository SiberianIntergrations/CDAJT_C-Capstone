import React from "react";
import { DataGrid } from "@/components/common/DataGrid";
import { OrderCard } from "./OrderCard";

export const OrderGrid = ({ orders, onOrderClick }) => {
  return (
    <DataGrid
      items={orders}
      renderItem={(order) => (
        <OrderCard
          key={order.orderId}
          order={order}
          onViewDetails={() => onOrderClick(order.orderId)}
        />
      )}
      emptyMessage="No orders to display for this status"
      emptyVariant="info"
    />
  );
};