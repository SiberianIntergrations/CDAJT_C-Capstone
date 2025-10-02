// File: sushi-toshi-frontend/pages/dashboard/orders.jsx
import React, { useState, useEffect } from "react";
import {
  Accordion,
  AccordionSummary,
  AccordionDetails,
  Typography,
  Box,
  CircularProgress,
  Alert,
  Chip,
  IconButton,
} from "@mui/material";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import { Plus } from "lucide-react";
import { useRouter } from "next/router";
import BillSelect from "../../components/customer/OderDashboard/components/BillSelect";
import { axiosInstance, createApiUrl } from "../../config/api";

const OrdersAccordion = () => {
  const router = useRouter();
  const [groupedOrders, setGroupedOrders] = useState({});
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [sessionId, setSessionId] = useState(null);
  const [selectedBillId, setSelectedBillId] = useState("");

  const statusOrder = ["PENDING", "CONFIRMED", "COMPLETED", "CANCELLED"];
  const statusColors = {
    PENDING: "#ff9800",
    CONFIRMED: "#2196f3",
    COMPLETED: "#4caf50",
    CANCELLED: "#f44336",
  };

  const getToken = () => {
    if (typeof window !== "undefined") {
      return localStorage.getItem("access_token");
    }
    return null;
  };

  // Fetch active session ID
  useEffect(() => {
    const getActiveSession = async () => {
      try {
        const response = await axiosInstance.get(
          createApiUrl("/dining-sessions/participants/active-session-id")
        );

        if (response.statusText !== "OK") {
          throw new Error("Failed to fetch active session");
        }

        if (response.data) {
          setSessionId(response.data);
        } else {
          setError("No active session found");
        }
      } catch (err) {
        console.error("Error fetching session:", err);
        setError(
          typeof err.response?.data?.detail === 'string' 
            ? err.response.data.detail 
            : "Error fetching session"
        );
      }
    };

    getActiveSession();
  }, []);

  const fetchOrders = async (billId = "") => {
    try {
      const token = getToken();
      if (!token) {
        setError("No authentication token found");
        setLoading(false);
        return;
      }

      let url = "/orders/my-active-session/orders";
      if (billId) {
        url += `?bill_id=${billId}`;
      }

      const response = await axiosInstance.get(createApiUrl(url));

      if (response.statusText !== "OK") {
        throw new Error("Failed to fetch orders");
      }

      if (!response.data) {
        setGroupedOrders({});
        setLoading(false);
        return;
      }

      const orderDetailsPromises = response.data.map((order) =>
        axiosInstance.get(createApiUrl(`/orders/${order.order_id}/order-items`))
      );

      const orderDetails = await Promise.all(orderDetailsPromises);

      const ordersWithItems = response.data.map((order, index) => ({
        ...order,
        items: orderDetails[index]?.data || [],
      }));

      const grouped = ordersWithItems.reduce((acc, order) => {
        const status = order.status.toUpperCase();
        if (!acc[status]) {
          acc[status] = [];
        }
        acc[status].push(order);
        return acc;
      }, {});

      setGroupedOrders(grouped);
      setLoading(false);
    } catch (err) {
      console.error("Error fetching orders:", err);
      setError(
        typeof err.response?.data?.detail === 'string'
          ? err.response.data.detail
          : "Error loading orders"
      );
      setLoading(false);
    }
  };

  const handleBillChange = (event) => {
    const billId = event.target.value;
    setSelectedBillId(billId);
    fetchOrders(billId);
  };

  const handleNewOrder = (event) => {
    event.preventDefault();
    router.push("/menu/full-menu");
  };
  useEffect(() => {
    fetchOrders(selectedBillId);
    const interval = setInterval(() => fetchOrders(selectedBillId), 30000);
    return () => clearInterval(interval);
  }, [selectedBillId]);

  const renderOrderAccordion = (order, groupStatus) => (
    <Accordion
      key={order.order_id}
      sx={{
        mb: 1,
        border: groupStatus === "PENDING" ? "1px solid #ff9800" : "none",
      }}
    >
      <AccordionSummary
        expandIcon={<ExpandMoreIcon />}
        sx={{
          backgroundColor:
            groupStatus === "PENDING"
              ? "rgba(255, 152, 0, 0.05)"
              : "rgba(0, 0, 0, 0.03)",
        }}
      >
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
            width: "100%",
            pr: 2,
          }}
        >
          <Typography>
            Order #{order.order_id}
            {order.items && (
              <Typography component="span" color="textSecondary" sx={{ ml: 2 }}>
                ({order.items.length} items)
              </Typography>
            )}
          </Typography>
        </Box>
      </AccordionSummary>
      <AccordionDetails>
        {order.items && order.items.length > 0 ? (
          <Box component="ul" sx={{ listStyle: "none", p: 0, m: 0 }}>
            {order.items.map((item) => (
              <Box
                component="li"
                key={item.order_item_id}
                sx={{
                  py: 1.5,
                  borderBottom: "1px solid rgba(0, 0, 0, 0.12)",
                  "&:last-child": { borderBottom: "none" },
                  display: "flex",
                  justifyContent: "space-between",
                  alignItems: "center",
                }}
              >
                <Box>
                  <Typography variant="body1" align="left">
                    {item.name || `Item #${item.item_id}`} - Quantity:{" "}
                    {item.quantity}
                  </Typography>
                  <Typography
                    variant="body2"
                    color="textSecondary"
                    align="left"
                  >
                    {item.price_at_time === 0
                      ? "Included item"
                      : `Price: $${item.price_at_time.toFixed(2)}`}
                  </Typography>
                </Box>

                <Chip
                  label={item.status}
                  size="small"
                  sx={{
                    backgroundColor: statusColors[item.status.toUpperCase()],
                    color: "white",
                  }}
                />
              </Box>
            ))}
          </Box>
        ) : (
          <Typography color="textSecondary">
            No items found for this order.
          </Typography>
        )}
      </AccordionDetails>
    </Accordion>
  );

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" p={4}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Box p={2}>
        <Alert severity="error">{typeof error === 'string' ? error : 'An unexpected error occurred'}</Alert>
      </Box>
    );
  }

  return (
    <Box p={2}>
      <Box
        sx={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          mb: 3,
        }}
      >
        <Typography variant="h4">Table Orders</Typography>
        <IconButton
          onClick={() => router.push("/menu/full-menu")}
          sx={{
            backgroundColor: "primary.main",
            color: "white",
            width: 48,
            height: 48,
            "&:hover": {
              backgroundColor: "primary.dark",
            },
          }}
        >
          <Plus size={24} />
        </IconButton>
      </Box>

      <Box mb={3}>
        {sessionId ? (
          <BillSelect
            session_id={sessionId.session_id}
            value={selectedBillId}
            onChange={handleBillChange}
            message="Select Bill To Filter Orders"
          />
        ) : (
          <Alert severity="info">No active session found</Alert>
        )}
      </Box>

      {Object.keys(groupedOrders).length === 0 ? (
        <Alert severity="info">
          {selectedBillId
            ? "No orders found for selected bill."
            : "No orders found. Select a bill to view its orders."}
        </Alert>
      ) : (
        statusOrder.map((status) => {
          if (!groupedOrders[status]?.length) return null;

          return (
            <Box key={status} sx={{ mb: 4 }}>
              <Box
                sx={{
                  display: "flex",
                  alignItems: "center",
                  gap: 2,
                  mb: 2,
                }}
              >
                <Typography variant="h6">
                  {status.charAt(0) + status.slice(1).toLowerCase()} Orders
                </Typography>
                <Chip
                  label={groupedOrders[status].length}
                  size="small"
                  sx={{
                    backgroundColor: statusColors[status],
                    color: "white",
                    fontWeight: "bold",
                  }}
                />
              </Box>
              {groupedOrders[status].map((order) =>
                renderOrderAccordion(order, status)
              )}
            </Box>
          );
        })
      )}
    </Box>
  );
};

export default OrdersAccordion;
