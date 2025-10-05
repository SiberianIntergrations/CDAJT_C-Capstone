import React, { useState } from "react";
import { Container, TextField, Button, Typography } from "@mui/material";
import PendingItemsAccordion from "@/components/customer/OderDashboard/components/PendingItemsAccordion";

const OrdersPage = () => {
  const [orderId, setOrderId] = useState("");
  const [selectedOrderId, setSelectedOrderId] = useState(null);

  const handleOrderIdChange = (event) => {
    setOrderId(event.target.value);
  };

  const handleLoadOrder = () => {
    if (orderId) {
      setSelectedOrderId(orderId);
    }
  };

  return (
    <Container maxWidth="sm" style={{ marginTop: "2rem" }}>
      <Typography variant="h4" gutterBottom>
        View Pending Items
      </Typography>

      <TextField
        label="Order ID"
        variant="outlined"
        fullWidth
        value={orderId}
        onChange={handleOrderIdChange}
        style={{ marginBottom: "1rem" }}
      />
      <Button variant="contained" color="primary" onClick={handleLoadOrder}>
        Load Order
      </Button>

      {selectedOrderId && (
        <div style={{ marginTop: "2rem" }}>
          <PendingItemsAccordion orderId={selectedOrderId} />
        </div>
      )}
    </Container>
  );
};

export default OrdersPage;
