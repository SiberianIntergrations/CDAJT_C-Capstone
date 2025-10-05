import React, { useState, useEffect } from "react";
import Accordion from "@mui/material/Accordion";
import AccordionSummary from "@mui/material/AccordionSummary";
import AccordionDetails from "@mui/material/AccordionDetails";
import Typography from "@mui/material/Typography";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import CircularProgress from "@mui/material/CircularProgress";
import Alert from "@mui/material/Alert";
import Box from "@mui/material/Box";
import { axiosInstance, createApiUrl } from "@/config/api";

const ActiveOrdersAccordion = () => {
  const [activeOrderIds, setActiveOrderIds] = useState([]);
  const [pendingItemsByOrder, setPendingItemsByOrder] = useState({});
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const getToken = () => localStorage.getItem("access_token");

  useEffect(() => {
    const fetchActiveOrders = async () => {
      try {
        const response = await axiosInstance.get(
          createApiUrl("/orders/my-active-session/pending-orders")
        );
        setActiveOrderIds(response.data);

        const itemPromises = response.data.map((orderId) =>
          axiosInstance.get(createApiUrl(`/orders/${orderId}/pending-items`))
        );

        const itemResponses = await Promise.all(itemPromises);
        const itemsByOrder = itemResponses.reduce((acc, response, index) => {
          acc[response.data[0]?.order_id || activeOrderIds[index]] =
            response.data;
          return acc;
        }, {});

        setPendingItemsByOrder(itemsByOrder);
        setLoading(false);
      } catch (err) {
        console.error("Error fetching orders:", err);
        setError(err.response?.data?.detail || "Error fetching orders");
        setLoading(false);
      }
    };

    fetchActiveOrders();
    // Set up polling every 30 seconds
    const intervalId = setInterval(fetchActiveOrders, 30000);

    return () => clearInterval(intervalId);
  }, []);

  if (loading) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight="200px"
      >
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return <Alert severity="error">{error}</Alert>;
  }

  if (activeOrderIds.length === 0) {
    return <Alert severity="info">No active orders found.</Alert>;
  }

  return (
    <div>
      <Typography variant="h5" gutterBottom>
        Active Orders
      </Typography>
      {activeOrderIds.map((orderId) => (
        <Accordion key={orderId}>
          <AccordionSummary
            expandIcon={<ExpandMoreIcon />}
            aria-controls={`order-${orderId}-content`}
            id={`order-${orderId}-header`}
          >
            <Typography variant="h6">
              Order #{orderId}
              <Typography
                component="span"
                variant="subtitle1"
                color="textSecondary"
                sx={{ ml: 2 }}
              >
                ({pendingItemsByOrder[orderId]?.length || 0} items pending)
              </Typography>
            </Typography>
          </AccordionSummary>
          <AccordionDetails>
            {pendingItemsByOrder[orderId]?.length > 0 ? (
              <Box component="ul" sx={{ listStyle: "none", p: 0, m: 0 }}>
                {pendingItemsByOrder[orderId].map((item) => (
                  <Box
                    component="li"
                    key={item.order_item_id}
                    sx={{
                      py: 1,
                      borderBottom: "1px solid rgba(0, 0, 0, 0.12)",
                      "&:last-child": { borderBottom: "none" },
                    }}
                  >
                    <Typography variant="body1">
                      {item.menu_item?.name || "Unknown Item"} × {item.quantity}
                    </Typography>
                    <Typography variant="body2" color="textSecondary">
                      Status: {item.status}
                    </Typography>
                  </Box>
                ))}
              </Box>
            ) : (
              <Typography color="textSecondary">
                No pending items found for this order.
              </Typography>
            )}
          </AccordionDetails>
        </Accordion>
      ))}
    </div>
  );
};

export default ActiveOrdersAccordion;
