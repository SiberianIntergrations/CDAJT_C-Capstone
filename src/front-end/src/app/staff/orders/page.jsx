"use client";
import React, { useState, useEffect } from "react";
import {
  Box,
  Card,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  IconButton,
  Button,
  TextField,
  Alert,
  CircularProgress,
  Paper,
  Chip,
  Tabs,
  Tab,
  Checkbox,
} from "@mui/material";
import { Trash2, Check, AlertCircle, Package, ShoppingBag, RefreshCw } from "lucide-react";
import api from "@/config/api";

const OrderDashboard = () => {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [editingItems, setEditingItems] = useState({});
  const [completedItems, setCompletedItems] = useState({});
  const [currentTab, setCurrentTab] = useState("PENDING");

  useEffect(() => {
    fetchAllOrders();
    const interval = setInterval(fetchAllOrders, 30000); // Refresh every 30 seconds
    return () => clearInterval(interval);
  }, []);

  const fetchAllOrders = async () => {
    try {
      setLoading(true);
      setError(null);

      // Get all active sessions
      const sessionsResponse = await api.get("/DiningSession/get_list_dining_sessions", {
        params: { activeOnly: true }
      });

      if (!Array.isArray(sessionsResponse.data) || sessionsResponse.data.length === 0) {
        setOrders([]);
        return;
      }

      // Fetch orders for all active sessions
      const allOrders = [];
      for (const session of sessionsResponse.data) {
        try {
          const sessionId = session.session_id || session.Session_Id || session.session_Id;
          if (!sessionId) {
          console.error("Invalid session object:", session);
          continue;
        }

        console.log(`Fetching orders for session ${sessionId}`);
        
        const ordersResponse = await api.get(`/Order/session/${sessionId}`);
        
        console.log(`Orders for session ${sessionId}:`, ordersResponse.data);

        if (Array.isArray(ordersResponse.data)) {
          allOrders.push(...ordersResponse.data.map(order => ({
            // Map backend response to frontend format
            orderId: order.orderId || order.order_Id || order.Order_Id,
            sessionId: order.sessionId || order.session_Id || order.Session_Id,
            billId: order.billId || order.bill_Id || order.Bill_Id,
            customerId: order.customerId || order.customer_Id || order.User_Id,
            customerName: order.customerName || order.customer_Name,
            status: order.status || order.Status,
            itemTotal: order.itemTotal || order.item_Total,
            orderTotal: order.orderTotal || order.order_Total,
            createdAt: order.createdAt || order.created_At,
            completedAt: order.completedAt || order.completed_At,
            tableNumbers: session.Table_Numbers || session.table_Numbers || session.table_numbers || [],
            // Map items array
            items: Array.isArray(order.items) 
              ? order.items.map(item => ({
                  orderItemId: item.orderItemId || item.order_Item_Id,
                  itemId: item.itemId || item.item_Id,
                  name: item.name || item.Name,
                  quantity: item.quantity || item.Quantity,
                  // priceAtTime: item.priceAtTime || item.price_At_Time || item.Price_At_Time,
                  status: item.status || item.Status
                }))
              : []
          })));
        }
      } catch (err) {
        console.error(`Error fetching orders for session:`, err);
        console.error("Session object:", session);
      }
    }

      console.log("All orders mapped:", allOrders);
      setOrders(allOrders);
    } catch (err) {
      console.error("Error fetching sessions/orders:", err);
      console.error("Error response:", err.response?.data);
      setError(
        err?.response?.data?.message || 
        err?.response?.data?.detail || 
        "Failed to fetch orders"
      );
    } finally {
      setLoading(false);
    }
  };

  const handleQuantityChange = (orderId, itemId, newQuantity) => {
    const key = `${orderId}-${itemId}`;
    const parsedQuantity = parseInt(newQuantity);

    // Only update if it's a valid number and greater than 0
    if (!isNaN(parsedQuantity) && parsedQuantity > 0) {
      setEditingItems((prev) => ({
        ...prev,
        [key]: parsedQuantity,
      }));
    } else if (newQuantity === "" || newQuantity === "0") {
      // Allow clearing the field
      setEditingItems((prev) => ({
        ...prev,
        [key]: "",
      }));
    }
  };

  const handleItemCompletionToggle = (orderId, itemId) => {
    const key = `${orderId}-${itemId}`;
    setCompletedItems(prev => ({
      ...prev,
      [key]: !prev[key]
    }));
  };

  // TODO: Update database and check if delete works properly now.
  const handleRemoveItem = async (orderId, itemId) => {
    try {
      // Find the order to get the orderItemId
      const order = orders.find(o => o.orderId === orderId);
      const item = order?.items.find(i => i.itemId === itemId);
    
      if (!item || !item.orderItemId) {
        setError("Order item not found");
        return;
      }

      await api.delete(`/Order/${orderId}/items/${item.orderItemId}`);
      console.log(`Removed item ${item.orderItemId} from order ${orderId}`);
      await fetchAllOrders();
    } catch (err) {
      console.error("Error removing item:", err);
      setError("Failed to remove item");
    }
  };

  const handleApproveOrder = async (orderId) => {
    try {
      // TODO: Have a single edit button per order to save all changes at once
      // Update any modified quantities
      const order = orders.find(o => o.orderId === orderId);

      if (!order) {
        setError("Order not found");
        return;
      }

      const updatedItems = order.items
        .map(item => {
          const key = `${orderId}-${item.itemId}`;
          const newQuantity = editingItems[key];
          if (newQuantity !== undefined && newQuantity !== "" && newQuantity !== item.quantity) {
            return {
              item_id: item.itemId,
              quantity: parseInt(newQuantity)
            };
          }
          return null;
        })
        .filter(Boolean);

        console.log("Updating quantities:", updatedItems);

        if (updatedItems.length > 0) {
          await api.patch(`/Order/${orderId}/items/quantities`, {
            items: updatedItems
          });
        }

      // Approve the order
      await api.patch(`/Order/${orderId}/approve`);

      const keysToDelete = order.items.map(item => `${orderId}-${item.itemId}`);
      setEditingItems(prev => {
        const newState = { ...prev };
        keysToDelete.forEach(key => delete newState[key]);
        return newState;
      });

      await fetchAllOrders();
    } catch (err) {
      console.error("Error approving order:", err);
      setError(err?.response?.data?.message || "Failed to approve order");
    }
  };

  // Marks items as delivered
  // TODO: Marking item delivered does not update order status when clicking mark as delivered
  const handleMarkItemsDelivered = async (orderId) => {
    try {
      const order = orders.find(o => o.orderId === orderId);
      
      const itemsToComplete = order.items.filter(item => {
        const key = `${orderId}-${item.itemId}`;
        return completedItems[key] === true;
      });

      if (itemsToComplete.length === 0) {
        setError("Please select at least one item to mark as delivered");
        return;
      }

      for (const item of itemsToComplete) {
        await api.patch(`/Order/${orderId}/items/${item.itemId}/complete`);
      }

      const clearedKeys = itemsToComplete.map(item => `${orderId}-${item.itemId}`);
      setCompletedItems(prev => {
        const newState = { ...prev };
        clearedKeys.forEach(key => delete newState[key]);
        return newState;
      });

      await fetchAllOrders();
    } catch (err) {
      console.error("Error marking items as delivered:", err);
      setError("Failed to mark items as delivered");
    }
  };

  // Marks entire order as delivered
  const handleMarkAllDelivered = async (orderId) => {
    try {
      await api.patch(`/Order/${orderId}/complete`);
      
      setCompletedItems(prev => {
        const newState = { ...prev };
        Object.keys(newState).forEach(key => {
          if (key.startsWith(`${orderId}-`)) {
            delete newState[key];
          }
        });
        return newState;
      });

      await fetchAllOrders();
    } catch (err) {
      console.error("Error marking order as delivered:", err);
      setError(err?.response?.data?.message || "Failed to mark order as delivered");
    }
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

  const filteredOrders = orders.filter(order => {
    const status = order.status?.toString().toUpperCase();
    if (currentTab === "PENDING") return status === "PENDING";
    if (currentTab === "PROCESSING") return status === "PROCESSING" || status === "APPROVED";
    if (currentTab === "DELIVERED") return status === "DELIVERED";
    return false;
  });

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="100vh">
        <CircularProgress />
      </Box>
    );
  }

  // TODO: Get Actions working for all tabs..


  return (
    <Box p={3}>
      <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 3 }}>
        <Typography variant="h4">Order Management</Typography>
        <Button
          startIcon={<RefreshCw />}
          onClick={fetchAllOrders}
          variant="outlined"
        >
          Refresh
        </Button>
      </Box>

      {error && (
        <Alert severity="error" onClose={() => setError(null)} sx={{ mb: 3 }}>
          {error}
        </Alert>
      )}

      <Tabs value={currentTab} onChange={(e, v) => setCurrentTab(v)} sx={{ mb: 3 }}>
        <Tab
          label={`Pending (${orders.filter(o => o.status === "PENDING").length})`}
          value="PENDING"
        />
        <Tab
          label={`Processing (${orders.filter(o => o.status === "PROCESSING" || o.status === "APPROVED").length})`}
          value="PROCESSING"
        />
        <Tab
          label={`Delivered (${orders.filter(o => o.status === "DELIVERED").length})`}
          value="DELIVERED"
        />
      </Tabs>

      {filteredOrders.length === 0 ? (
        <Alert severity="info">No orders to display for this status</Alert>
      ) : (
        filteredOrders.map((order) => (
          <Card key={order.orderId} sx={{ mb: 3, p: 2 }}>
            <Box
              sx={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
                mb: 2,
              }}
            >
              {order.tableNumbers && order.tableNumbers.length > 0 && (
              <Typography variant="h6">
                  Tables: {order.tableNumbers.join(", ")}
                  <Chip
                    label={order.status}
                    color={getStatusColor(order.status)}
                    size="small"
                    sx={{ ml: 2 }}
                  />
              </Typography>
              )}
              <Typography variant="body2" color="text.secondary">
                  Order #{order.orderId}
              </Typography>
            </Box>

            <TableContainer component={Paper}>
              <Table>
                <TableHead>
                  <TableRow>
                    {currentTab === "PROCESSING" && (
                      <TableCell sx={{ color: "white" }} padding="checkbox">
                        Done
                      </TableCell>
                    )}
                    <TableCell sx={{ color: "white" }}>Item</TableCell>
                    <TableCell sx={{ color: "white" }} align="center">
                      Quantity
                    </TableCell>
                    {currentTab === "PENDING" && (
                      <TableCell sx={{ color: "white" }} align="right">
                        Actions
                      </TableCell>
                    )}
                  </TableRow>
                </TableHead>
                <TableBody>
                  {order.items.map((item) => (
                    <TableRow key={item.itemId}>
                      {currentTab === "PROCESSING" && (
                        <TableCell padding="checkbox">
                          <Checkbox
                            checked={completedItems[`${order.orderId}-${item.itemId}`] || false}
                            onChange={() => handleItemCompletionToggle(order.orderId, item.itemId)}
                          />
                        </TableCell>
                      )}
                      <TableCell>{item.name}</TableCell>
                      <TableCell align="center">
                        {currentTab === "PENDING" ? (
                          <TextField
                            type="number"
                            value={
                              editingItems[`${order.orderId}-${item.itemId}`] || item.quantity
                            }
                            onChange={(e) =>
                              handleQuantityChange(
                                order.orderId,
                                item.itemId,
                                e.target.value
                              )
                            }
                            sx={{ width: "80px" }}
                            inputProps={{ min: 1 }}
                          />
                        ) : (
                          item.quantity
                        )}
                      </TableCell>
                      {currentTab === "PENDING" && (
                        <TableCell align="right">
                          <IconButton
                            color="error"
                            onClick={() =>
                              handleRemoveItem(order.orderId, item.itemId)
                            }
                          >
                            <Trash2 size={20} />
                          </IconButton>
                        </TableCell>
                      )}
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>

            {/* Single action section per order */}
            {currentTab === "PENDING" && (
              <Box sx={{ mt: 2 }}>
                <Box sx={{ display: "flex", alignItems: "center", gap: 1, mb: 2 }}>
                  <AlertCircle size={20} color="orange" />
                  <Typography variant="body2" color="text.secondary">
                    Review quantities and approve order when ready
                  </Typography>
                </Box>
                <Button
                  variant="contained"
                  color="primary"
                  startIcon={<Check />}
                  onClick={() => handleApproveOrder(order.orderId)}
                  fullWidth
                  size="large"
                >
                  Approve Order
                </Button>
              </Box>
            )}
            
            {currentTab === "PROCESSING" && (
              <Box sx={{ mt: 2 }}>
                <Box sx={{ display: "flex", alignItems: "center", gap: 1, mb: 2 }}>
                  <Package size={20} color="blue" />
                  <Typography variant="body2" color="text.secondary">
                    Check items as they are prepared and delivered
                  </Typography>
                </Box>
                <Box sx={{ display: "flex", gap: 2 }}>
                  <Button
                    variant="contained"
                    color="success"
                    startIcon={<Check />}
                    onClick={() => handleMarkItemsDelivered(order.orderId)}
                    fullWidth
                    size="large"
                  >
                    Mark Selected as Delivered
                  </Button>
                  <Button
                    variant="outlined"
                    color="success"
                    startIcon={<Package />}
                    onClick={() => handleMarkAllDelivered(order.orderId)}
                    fullWidth
                    size="large"
                  >
                    Mark All as Delivered
                  </Button>
                </Box>
              </Box>
            )}
          </Card>
        ))
      )}
    </Box>
  );
};

export default OrderDashboard;