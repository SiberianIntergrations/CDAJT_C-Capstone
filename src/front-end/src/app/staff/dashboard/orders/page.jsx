"use client";
import React, { useState, useEffect, useCallback } from "react";
import { Box, Alert, CircularProgress } from "@mui/material";
import api from "@/config/api";
import { useOrderActions } from "@/hooks/useOrderActions";
import { Header } from "@/components/staff/Order/OrderHeader";
import { TabBar } from "@/components/staff/Order/OrderTabBar";
import { OrderGrid } from "@/components/staff/Order/OrderGrid";
import { OrderDetailDialog } from "@/components/staff/Order/OrderDetailDialog";

const OrderDashboard = () => {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [selectedOrderId, setSelectedOrderId] = useState(null);
  const [editMode, setEditMode] = useState({});
  const [editedQuantities, setEditedQuantities] = useState({});
  const [currentTab, setCurrentTab] = useState("PENDING");

  const fetchAllOrders = useCallback(async () => {
    try {
      setLoading(true);

      const sessionsResponse = await api.get(
        "/DiningSession/get_list_dining_sessions",
        {
          params: { activeOnly: true },
        }
      );

      if (
        !Array.isArray(sessionsResponse.data) ||
        sessionsResponse.data.length === 0
      ) {
        setOrders([]);
        return;
      }

      const allOrders = [];
      for (const session of sessionsResponse.data) {
        try {
          const sessionId =
            session.session_id || session.Session_Id || session.session_Id;
          if (!sessionId) {
            console.error("Invalid session object:", session);
            continue;
          }

          const ordersResponse = await api.get(`/Order/session/${sessionId}`);
          // Map out the response data of the orders
          if (Array.isArray(ordersResponse.data)) {
            allOrders.push(
              ...ordersResponse.data.map((order) => ({
                orderId: order.orderId || order.order_Id || order.Order_Id,
                sessionId:
                  order.sessionId || order.session_Id || order.Session_Id,
                billId: order.billId || order.bill_Id || order.Bill_Id,
                customerId: order.customerId || order.customer_Id || order.User_Id,
                customerName: order.customerName || order.customer_Name,
                status: order.status || order.Status,
                itemTotal: order.itemTotal || order.item_Total,
                orderTotal: order.orderTotal || order.order_Total,
                createdAt: order.createdAt || order.created_At,
                completedAt: order.completedAt || order.completed_At,
                tableNumbers:
                  session.Table_Numbers ||
                  session.table_Numbers ||
                  session.table_numbers ||
                  [],
                items: Array.isArray(order.items)
                  ? order.items.map((item) => ({
                      orderItemId: item.orderItemId || item.order_Item_Id,
                      itemId: item.itemId || item.item_Id,
                      name: item.name || item.Name,
                      quantity: item.quantity || item.Quantity,
                      status: item.status || item.Status,
                    }))
                  : [],
              }))
            );
          }
        } catch (err) {
          console.error(`Error fetching orders for session:`, err);
        }
      }
      setOrders(allOrders);
    } catch (err) {
      console.error("Error fetching sessions/orders:", err);
    } finally {
      setLoading(false);
    }
  }, []);

  const {
    approveOrder,
    completeOrder,
    removeOrderItem,
    completeOrderItem,
    updateOrderItem,
    actionError,
    isLoading,
    clearActionError,
  } = useOrderActions(fetchAllOrders);

  useEffect(() => {
    fetchAllOrders();
    const interval = setInterval(fetchAllOrders, 30000);
    return () => clearInterval(interval);
  }, [fetchAllOrders]);

  const selectedOrder = orders.find((o) => o.orderId === selectedOrderId);

  const toggleEditMode = (orderId) => {
    setEditMode((prev) => ({
      ...prev,
      [orderId]: !prev[orderId],
    }));

    if (!editMode[orderId]) {
      const order = orders.find((o) => o.orderId === orderId);
      const quantities = {};
      order?.items.forEach((item) => {
        quantities[`${orderId}-${item.itemId}`] = item.quantity;
      });
      setEditedQuantities((prev) => ({ ...prev, ...quantities }));
    }
  };

  const updateQuantity = (orderId, itemId, delta) => {
    const key = `${orderId}-${itemId}`;
    setEditedQuantities((prev) => {
      const currentQty = prev[key] || 0;
      const newQty = Math.max(1, currentQty + delta);
      return { ...prev, [key]: newQty };
    });
  };

  const handleMarkItemDelivered = async (orderId, orderItemId) => {
    await completeOrderItem(orderId, orderItemId);
  };

  const handleRemoveItem = async (orderId, itemId) => {
    const order = orders.find((o) => o.orderId === orderId);
    const item = order?.items.find((i) => i.itemId === itemId);

    if (!item?.orderItemId) return;

    await removeOrderItem(orderId, item.orderItemId);
  };

  const handleApproveOrder = async (orderId) => {
    await approveOrder(orderId);
    setSelectedOrderId(null);
  };

  const handleSaveAndApprove = async (orderId) => {
    // First save the quantities
    const order = orders.find((o) => o.orderId === orderId);
    // Edit the quantities and update if there are changes
    for (const item of order.items) {
      const key = `${orderId}-${item.itemId}`;
      const newQuantity = editedQuantities[key];
      
      if (newQuantity !== item.quantity) {
        await updateOrderItem(orderId, item.orderItemId, {
          quantity: newQuantity,
        });
      }
    }

    // Then approve the order
    await approveOrder(orderId);
    
    // Close dialog and reset edit mode
    setEditMode((prev) => ({ ...prev, [orderId]: false }));
    setSelectedOrderId(null);
  };

  const handleMarkAllDelivered = async (orderId) => {
    await completeOrder(orderId);
    setSelectedOrderId(null);
  };

  const getStatusColor = (status) => {
    if (!status) return "default";
    switch (status.toString().toUpperCase()) {
      case "PENDING":
        return "warning";
      case "APPROVED":
      case "PROCESSING":
        return "info";
      case "DELIVERED":
        return "success";
      case "CANCELLED":
        return "error";
      default:
        return "default";
    }
  };

  const filteredOrders = orders.filter((order) => {
    const status = order.status?.toString().toUpperCase();
    if (currentTab === "PENDING") return status === "PENDING";
    if (currentTab === "PROCESSING")
      return status === "PROCESSING" || status === "APPROVED";
    if (currentTab === "DELIVERED") return status === "DELIVERED";
    return false;
  });

  const tabCounts = {
    PENDING: orders.filter((o) => o.status === "PENDING").length,
    PROCESSING: orders.filter(
      (o) => o.status === "PROCESSING" || o.status === "APPROVED"
    ).length,
    DELIVERED: orders.filter((o) => o.status === "DELIVERED").length,
  };

  if (loading) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight="100vh"
      >
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box sx={{ minHeight: "100vh", bgcolor: "background.default" }}>
      <Header onRefresh={fetchAllOrders} isLoading={isLoading} />

      {actionError && (
        <Box sx={{ p: 2 }}>
          <Alert severity="error" onClose={clearActionError}>
            {actionError}
          </Alert>
        </Box>
      )}

      <TabBar
        currentTab={currentTab}
        onChange={setCurrentTab}
        counts={tabCounts}
      />

      <OrderGrid
        orders={filteredOrders}
        onOrderClick={setSelectedOrderId}
      />

      <OrderDetailDialog
        open={!!selectedOrderId}
        onClose={() => {
          setSelectedOrderId(null);
          setEditMode((prev) => ({ ...prev, [selectedOrderId]: false }));
        }}
        order={selectedOrder}
        currentTab={currentTab}
        editMode={editMode[selectedOrderId]}
        editedQuantities={editedQuantities}
        isLoading={isLoading}
        onToggleEdit={toggleEditMode}
        onQuantityChange={updateQuantity}
        onRemoveItem={handleRemoveItem}
        onMarkItemDelivered={handleMarkItemDelivered}
        onSaveAndApprove={handleSaveAndApprove}
        onApprove={handleApproveOrder}
        onMarkAll={handleMarkAllDelivered}
        getStatusColor={getStatusColor}
      />
    </Box>
  );
};

export default OrderDashboard;