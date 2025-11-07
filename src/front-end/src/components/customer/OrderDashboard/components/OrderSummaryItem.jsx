import React, { useState } from "react";
import { Box, Typography, Button } from "@mui/material";
import { Minus, Plus } from "lucide-react";

const QuantityButton = ({ children, onClick, ...props }) => (
  <Button
    size="small"
    variant="outlined"
    onClick={onClick}
    sx={{
      borderColor: "#C01E2E",
      color: "#C01E2E",
      minWidth: "48px",
      width: "48px",
      height: "40px",
      padding: 0,
      "&:hover": {
        borderColor: "#A01725",
        color: "#A01725",
      },
      ...props.sx,
    }}
  >
    {children}
  </Button>
);

const OrderSummaryItem = ({ item, quantity, category, onQuantityChange }) => {
  const [expanded, setExpanded] = useState(false);
  const unitPrice = item.is_add_on ? item.price : 0;
  const totalPrice = unitPrice * quantity;
  //To show items that are included in the all you can eat ($0)
  const includedPrice = unitPrice === 0;

  return (
    <Box sx={{ borderBottom: "1px solid", borderColor: "divider" }}>
      <Box
        onClick={() => setExpanded(!expanded)}
        sx={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          py: 1.5,
          cursor: "pointer",
          px: 0.5,
          "&:hover": {
            bgcolor: "rgba(0, 0, 0, 0.02)",
          },
        }}
      >
        <Typography variant="body2" sx={{ flex: 2, textAlign: "left" }}>
          {item.name}
        </Typography>
        <Typography variant="body2" sx={{ flex: 1, textAlign: "center" }}>
          x{quantity}
        </Typography>
        <Typography variant="body2" sx={{ flex: 1, textAlign: "right" }}>
          {includedPrice ? "Included" : `$${unitPrice.toFixed(2)}`}
        </Typography>
        <Typography variant="body2" sx={{ flex: 1, textAlign: "right" }}>
          {includedPrice ? "Included" : `$${totalPrice.toFixed(2)}`}
        </Typography>
      </Box>

      {expanded && (
        <Box
          sx={{
            p: 2,
            bgcolor: "rgba(0, 0, 0, 0.02)",
            display: "flex",
            flexDirection: "column",
            gap: 2,
          }}
        >
          <Typography variant="body2" color="text.secondary">
            {item.description}
          </Typography>

          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Box sx={{ display: "flex", alignItems: "center" }}>
              <QuantityButton
                onClick={() =>
                  onQuantityChange(category.category_id, item.item_id, -1)
                }
                sx={{ borderTopRightRadius: 0, borderBottomRightRadius: 0 }}
              >
                <Minus size={16} />
              </QuantityButton>

              <Typography
                sx={{
                  width: "32px",
                  textAlign: "center",
                  borderTop: "1px solid #C01E2E",
                  borderBottom: "1px solid #C01E2E",
                  height: "40px",
                  lineHeight: "40px",
                }}
              >
                {quantity}
              </Typography>

              <QuantityButton
                onClick={() =>
                  onQuantityChange(category.category_id, item.item_id, 1)
                }
                sx={{ borderTopLeftRadius: 0, borderBottomLeftRadius: 0 }}
              >
                <Plus size={16} />
              </QuantityButton>
            </Box>

            <Button
              size="small"
              variant="outlined"
              color="error"
              onClick={() =>
                onQuantityChange(category.category_id, item.item_id, -quantity)
              }
              sx={{ height: "40px" }}
            >
              Remove
            </Button>
          </Box>
        </Box>
      )}
    </Box>
  );
};

const OrderSummary = ({
  categories,
  quantities,
  menuItems,
  onQuantityChange,
}) => {
  const totalPrice = categories.reduce(
    (total, category) =>
      total +
      Object.entries(quantities[category.category_id] || {}).reduce(
        (catTotal, [itemId, quantity]) => {
          const item = menuItems[category.category_id]?.find(
            (item) => item.item_id === parseInt(itemId)
          );
          return catTotal + (item?.is_add_on ? item.price * quantity : 0);
        },
        0
      ),
    0
  );

  return (
    <Box>
      <Box
        sx={{
          bgcolor: "#f8f9fa",
          p: 2,
          borderBottom: "1px solid",
          borderColor: "divider",
        }}
      >
        <Typography variant="h6">Order Summary</Typography>
      </Box>

      <Box sx={{ p: 2 }}>
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            borderBottom: "2px solid",
            borderColor: "divider",
            pb: 1,
            mb: 2,
            px: 0.5,
          }}
        >
          <Typography
            variant="subtitle2"
            sx={{ flex: 2, fontWeight: "bold", textAlign: "left" }}
          >
            Item
          </Typography>
          <Typography
            variant="subtitle2"
            sx={{ flex: 1, textAlign: "center", fontWeight: "bold" }}
          >
            Quantity
          </Typography>
          <Typography
            variant="subtitle2"
            sx={{ flex: 1, textAlign: "right", fontWeight: "bold" }}
          >
            Unit Price
          </Typography>
          <Typography
            variant="subtitle2"
            sx={{ flex: 1, textAlign: "right", fontWeight: "bold" }}
          >
            Total
          </Typography>
        </Box>

        {categories.map((category) =>
          Object.entries(quantities[category.category_id] || {})
            .filter(([_, quantity]) => quantity > 0)
            .map(([itemId, quantity]) => {
              const item = menuItems[category.category_id]?.find(
                (item) => item.item_id === parseInt(itemId)
              );
              if (!item) return null;

              return (
                <OrderSummaryItem
                  key={itemId}
                  item={item}
                  quantity={quantity}
                  category={category}
                  onQuantityChange={onQuantityChange}
                />
              );
            })
        )}

        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            borderTop: "2px solid",
            borderColor: "divider",
            pt: 2,
            mt: 2,
            px: 0.5,
            fontWeight: "bold",
          }}
        >
          <Typography variant="subtitle1">Total</Typography>
          <Typography variant="subtitle1">${totalPrice.toFixed(2)}</Typography>
        </Box>
      </Box>
    </Box>
  );
};

export default OrderSummary;
