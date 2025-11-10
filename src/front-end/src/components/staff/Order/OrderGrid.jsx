import React from "react";
import { DataGrid } from "@/components/common/DataGrid";
import { OrderCard } from "./OrderCard";

export const OrderGrid = ({ orders, onOrderClick }) => {
  return (
    <DataGrid
      items={orders}
      renderItem={(order) => (
        <OrderCard
          order={order}
          onViewDetails={() => onOrderClick(order.orderId)}
        />
      )}
      keyExtractor={(order) => `order-${order.orderId}`}
      emptyMessage="There are currently no orders to display."
      emptyVariant="info"
    />
  );
};