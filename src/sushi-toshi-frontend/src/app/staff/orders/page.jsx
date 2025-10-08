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
} from "@mui/material";
import { Trash2, Check, AlertCircle, Package, ShoppingBag } from "lucide-react";
import { axiosInstance, createApiUrl } from "@/config/api";

const OrderDashboard = () => {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [editingItems, setEditingItems] = useState({});

  useEffect(() => {
    fetchOrders();
    const interval = setInterval(fetchOrders, 30000); // Refresh every 30 seconds
    return () => clearInterval(interval);
  }, []);

  const fetchOrders = async () => {
    try {
      setLoading(true);
      const response = await axiosInstance.get(
        createApiUrl("/orders/?session_id=1")
      );

      if (response.statusText !== "OK")
        throw new Error("Failed to fetch orders");
      console.log("Response:", response);

      // Transform the data to match the component's expected structure
      const transformedData = response.data.map((order) => ({
        ...order,
        items: order.order_items, // Add this line to map order_items to items
      }));

      setOrders(transformedData);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleQuantityChange = (orderId, itemId, newQuantity) => {
    setEditingItems((prev) => ({
      ...prev,
      [`${orderId}-${itemId}`]: Math.max(1, parseInt(newQuantity) || 0),
    }));
  };

  const handleRemoveItem = async (orderId, itemId) => {
    try {
      const token = localStorage.getItem("access_token");
      await fetch(`http://localhost:8000/orders/${orderId}/items/${itemId}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      await fetchOrders();
    } catch (err) {
      setError("Failed to remove item");
    }
  };

  const handleApproveOrder = async (orderId) => {
    try {
      const token = localStorage.getItem("access_token");
      const itemUpdates = Object.entries(editingItems)
        .filter(([key]) => key.startsWith(`${orderId}-`))
        .map(([key, quantity]) => ({
          item_id: key.split("-")[1],
          quantity,
        }));

      await fetch(`http://localhost:8000/orders/${orderId}/approve`, {
        method: "POST",
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ items: itemUpdates }),
      });

      // Clear editing state for this order
      setEditingItems((prev) => {
        const newState = { ...prev };
        Object.keys(newState)
          .filter((key) => key.startsWith(`${orderId}-`))
          .forEach((key) => delete newState[key]);
        return newState;
      });

      await fetchOrders();
    } catch (err) {
      setError("Failed to approve order");
    }
  };

  const handleMarkDelivered = async (orderId) => {
    try {
      const token = localStorage.getItem("access_token");
      await fetch(`http://localhost:8000/orders/${orderId}/deliver`, {
        method: "POST",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      await fetchOrders();
    } catch (err) {
      setError("Failed to mark order as delivered");
    }
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" p={4}>
        <CircularProgress />
      </Box>
    );
  }

  const getStatusColor = (status) => {
    switch (status.toLowerCase()) {
      case "pending":
        return "warning";
      case "approved":
        return "info";
      case "delivered":
        return "success";
      default:
        return "default";
    }
  };

  return (
    <Box p={3}>
      <Typography variant="h4" gutterBottom>
        Order Management
      </Typography>

      {error && (
        <Alert severity="error" onClose={() => setError(null)} sx={{ mb: 3 }}>
          {error}
        </Alert>
      )}

      {orders.length === 0 ? (
        <Alert severity="info">No pending orders to display</Alert>
      ) : (
        (console.log(orders),
        orders.map((order) => (
          <Card key={order.order_id} sx={{ mb: 3, p: 2 }}>
            <Box
              sx={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
                mb: 2,
              }}
            >
              <Typography variant="h6">
                Order #{order.order_id}
                <Chip
                  label={order.status}
                  color={getStatusColor(order.status)}
                  size="small"
                  sx={{ ml: 2 }}
                />
              </Typography>
              <Box>
                {order.status === "PENDING" && (
                  <Button
                    variant="contained"
                    color="primary"
                    startIcon={<Check />}
                    onClick={() => handleApproveOrder(order.order_id)}
                    sx={{ mr: 1 }}
                  >
                    Approve
                  </Button>
                )}
                {order.status === "APPROVED" && (
                  <Button
                    variant="contained"
                    color="success"
                    startIcon={<Package />}
                    onClick={() => handleMarkDelivered(order.order_id)}
                  >
                    Mark Delivered
                  </Button>
                )}
              </Box>
            </Box>

            <TableContainer component={Paper}>
              <Table>
                <TableHead>
                  <TableRow>
                    <TableCell sx={{ color: "white" }}>Item</TableCell>
                    <TableCell sx={{ color: "white" }} align="center">
                      Quantity
                    </TableCell>
                    <TableCell sx={{ color: "white" }} align="right">
                      Actions
                    </TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {order.items.map((item) => (
                    <TableRow key={item.item_id}>
                      <TableCell>{item.name}</TableCell>
                      <TableCell align="center">
                        {order.status === "PENDING" ? (
                          <TextField
                            type="number"
                            value={
                              editingItems[
                                `${order.order_id}-${item.item_id}`
                              ] || item.quantity
                            }
                            onChange={(e) =>
                              handleQuantityChange(
                                order.order_id,
                                item.item_id,
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
                      <TableCell align="right">
                        {order.status === "PENDING" && (
                          <IconButton
                            color="error"
                            onClick={() =>
                              handleRemoveItem(order.order_id, item.item_id)
                            }
                          >
                            <Trash2 size={20} />
                          </IconButton>
                        )}
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>

            {order.status === "PENDING" && (
              <Box
                sx={{ mt: 2, display: "flex", alignItems: "center", gap: 1 }}
              >
                <AlertCircle size={20} color="orange" />
                <Typography variant="body2" color="text.secondary">
                  Review quantities and approve order when ready
                </Typography>
              </Box>
            )}
          </Card>
        )))
      )}
    </Box>
  );
};

export default OrderDashboard;
