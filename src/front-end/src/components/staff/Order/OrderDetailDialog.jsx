import React from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Box,
  Typography,
  Button,
  IconButton,
  Chip,
  List,
  ListItem,
  Divider,
} from "@mui/material";
import {
  X,
  Check,
  Trash2,
  Plus,
  Minus,
  Package,
  Edit2,
} from "lucide-react";

export const OrderDetailDialog = ({
  open,
  onClose,
  order,
  currentTab,
  editMode,
  editedQuantities,
  completedItems,
  isLoading,
  onToggleEdit,
  onQuantityChange,
  onRemoveItem,
  onMarkItemDelivered,
  onSaveAndApprove,
  onApprove,
  onMarkAll,
  getStatusColor,
}) => {
  if (!order) return null;

  return (
    <Dialog
      open={open}
      onClose={onClose}
      maxWidth="sm"
      fullWidth
      PaperProps={{
        sx: {
          maxHeight: "90vh",
        },
      }}
    >
      <DialogTitle>
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
          }}
        >
          <Box>
            <Typography variant="h6">
              {order.tableNumbers && order.tableNumbers.length > 0
                ? `Tables ${order.tableNumbers.join(", ")}`
                : `Order #${order.orderId}`}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Order Items ({order.items.length})
            </Typography>
          </Box>
          <IconButton onClick={onClose} size="small">
            <X size={20} />
          </IconButton>
        </Box>
      </DialogTitle>

      <DialogContent dividers sx={{ px: 0 }}>
        <List sx={{ py: 0 }}>
          {order.items.map((item, index) => (
            <Box key={item.itemId}>
              <ListItem
                sx={{
                  py: 2,
                  px: 3,
                  display: "flex",
                  justifyContent: "space-between",
                  alignItems: "center",
                }}
              >
                {/* Item Name and Status */}
                <Box sx={{ flex: 1 }}>
                  <Typography variant="body1" fontWeight="medium">
                    {item.name}
                  </Typography>
                  {currentTab === "PROCESSING" && (
                    <Chip
                      label={item.status || "Pending"}
                      color={getStatusColor(item.status)}
                      size="small"
                      sx={{ mt: 0.5 }}
                    />
                  )}
                </Box>

                {/* Quantity and Actions */}
                <Box
                  sx={{
                    display: "flex",
                    alignItems: "center",
                    gap: 1,
                  }}
                >
                  {/* Pending Tab - Edit Mode */}
                  {currentTab === "PENDING" && editMode ? (
                    <>
                      <IconButton
                        onClick={() =>
                          onQuantityChange(order.orderId, item.itemId, -1)
                        }
                        size="small"
                        sx={{
                          border: "1px solid",
                          borderColor: "divider",
                          borderRadius: 1,
                        }}
                      >
                        <Minus size={16} />
                      </IconButton>

                      <Typography
                        sx={{
                          minWidth: "40px",
                          textAlign: "center",
                          fontWeight: "medium",
                        }}
                      >
                        {editedQuantities[`${order.orderId}-${item.itemId}`] ||
                          item.quantity}
                      </Typography>

                      <IconButton
                        onClick={() =>
                          onQuantityChange(order.orderId, item.itemId, 1)
                        }
                        size="small"
                        sx={{
                          border: "1px solid",
                          borderColor: "divider",
                          borderRadius: 1,
                        }}
                      >
                        <Plus size={16} />
                      </IconButton>

                      <IconButton
                        color="error"
                        size="small"
                        onClick={() =>
                          onRemoveItem(order.orderId, item.itemId)
                        }
                      >
                        <Trash2 size={18} />
                      </IconButton>
                    </>
                  ) : currentTab === "PROCESSING" ? (
                    /* Processing Tab - Show Quantity and Mark Delivered Button */
                    <>
                      <Chip
                        label={`× ${item.quantity}`}
                        size="small"
                        sx={{
                          bgcolor: "grey.200",
                          color: "text.primary",
                          fontWeight: "medium",
                          minWidth: "50px",
                        }}
                      />

                      {item.status?.toUpperCase() !== "DELIVERED" && (
                        <Button
                          variant="outlined"
                          color="success"
                          size="small"
                          startIcon={<Check size={16} />}
                          onClick={() =>
                            onMarkItemDelivered(order.orderId, item.orderItemId)
                          }
                          sx={{
                            minWidth: "120px",
                            textTransform: "none",
                          }}
                        >
                          Mark Delivered
                        </Button>
                      )}
                    </>
                  ) : (
                    /* Pending Tab - View Mode */
                    <Chip
                      label={`× ${item.quantity}`}
                      size="small"
                      sx={{
                        bgcolor: "grey.200",
                        color: "text.primary",
                        fontWeight: "medium",
                        minWidth: "50px",
                      }}
                    />
                  )}
                </Box>
              </ListItem>
              {index < order.items.length - 1 && <Divider />}
            </Box>
          ))}
        </List>
      </DialogContent>

      <DialogActions sx={{ p: 2, flexDirection: "column", gap: 1 }}>
        {currentTab === "PENDING" && (
          <>
            {editMode ? (
              <Box sx={{ display: "flex", gap: 1, width: "100%" }}>
                <Button
                  variant="contained"
                  color="success"
                  startIcon={<Check />}
                  onClick={() => onSaveAndApprove(order.orderId)}
                  disabled={isLoading}
                  fullWidth
                  size="large"
                >
                  Save & Approve
                </Button>
                <Button
                  variant="outlined"
                  onClick={() => onToggleEdit(order.orderId)}
                  fullWidth
                >
                  Cancel
                </Button>
              </Box>
            ) : (
              <>
                <Button
                  variant="outlined"
                  startIcon={<Edit2 />}
                  onClick={() => onToggleEdit(order.orderId)}
                  fullWidth
                >
                  Edit Order
                </Button>
                <Button
                  variant="contained"
                  color="success"
                  startIcon={<Check />}
                  onClick={() => onApprove(order.orderId)}
                  disabled={isLoading}
                  fullWidth
                  size="large"
                >
                  Approve Order
                </Button>
              </>
            )}
          </>
        )}

        {currentTab === "PROCESSING" && (
          <Button
            variant="contained"
            color="success"
            startIcon={<Package />}
            onClick={() => onMarkAll(order.orderId)}
            disabled={isLoading}
            fullWidth
            size="large"
          >
            Mark All Delivered
          </Button>
        )}
      </DialogActions>
    </Dialog>
  );
};