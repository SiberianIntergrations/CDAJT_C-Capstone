import React, { useState } from "react";
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
  Snackbar,
  Alert,
  CircularProgress,
} from "@mui/material";
import {
  X,
  Check,
  Trash2,
  Plus,
  Minus,
  Package,
  Edit2,
  RotateCcw,
  Save,
  CheckCircle,
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
  onCancelItem,
  onUndoCancelItem,
  onMarkItemDelivered,
  onSaveChanges,
  onSaveAndApprove,
  onApprove,
  onMarkAll,
  getStatusColor,
}) => {
  if (!order) return null;

  const [cancelDialogOpen, setCancelDialogOpen] = useState(false);
  const [pendingCancel, setPendingCancel] = useState({
    orderId: null,
    itemId: null,
  });
  const [undoSnackbarOpen, setUndoSnackbarOpen] = useState(false);
  const [lastCanceled, setLastCanceled] = useState(null);

  // Show confirmation dialog
  const handleRequestCancel = (orderId, itemId) => {
    setPendingCancel({ orderId, itemId });
    setCancelDialogOpen(true);
  };

  // Confirm cancel
  const handleConfirmCancel = async () => {
    await onCancelItem(pendingCancel.orderId, pendingCancel.itemId);
    setLastCanceled({
      orderId: pendingCancel.orderId,
      itemId: pendingCancel.itemId,
    });
    setCancelDialogOpen(false);
    setPendingCancel({ orderId: null, itemId: null });
    setUndoSnackbarOpen(true);
  };

  // Undo cancel
  const handleUndoCancel = async () => {
    if (lastCanceled) {
      await onUndoCancelItem(lastCanceled.orderId, lastCanceled.itemId);
      setUndoSnackbarOpen(false);
      setLastCanceled(null);
    }
  };

  return (
    <>
      <Dialog
        open={open}
        onClose={onClose}
        maxWidth="sm"
        fullWidth
        slotProps={{
          paper: {
            sx: {
              maxHeight: "90vh",
            },
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
                Order # {order.orderId} -{" "}
                {order.tableNumbers && order.tableNumbers.length > 0
                  ? `Tables ${order.tableNumbers.join(", ")}`
                  : ` `}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Session # {order.sessionId}
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
                    <Chip
                      label={item.status || "Pending"}
                      color={getStatusColor(item.status)}
                      size="small"
                      sx={{ mt: 0.5 }}
                    />
                  </Box>

                  {/* Quantity and Actions */}
                  <Box
                    sx={{
                      display: "flex",
                      alignItems: "center",
                      gap: 1,
                    }}
                  >
                    {/* PENDING Tab - Cancelled Item (with Undo) */}
                    {item.status.toUpperCase() === "CANCELLED" ? (
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
                        {currentTab === "PENDING" && (
                          <Button
                            variant="outlined"
                            color="info"
                            size="small"
                            startIcon={<RotateCcw size={16} />}
                            onClick={() =>
                              onUndoCancelItem(order.orderId, item.itemId)
                            }
                            sx={{ ml: 1 }}
                          >
                            Undo
                          </Button>
                        )}
                      </>
                    ) : currentTab === "PENDING" && editMode ? (
                      /* PENDING Tab - Edit Mode */
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
                          {editedQuantities[
                            `${order.orderId}-${item.itemId}`
                          ] || item.quantity}
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
                            handleRequestCancel(order.orderId, item.itemId)
                          }
                        >
                          <Trash2 size={18} />
                        </IconButton>
                      </>
                    ) : currentTab === "PROCESSING" ? (
                      /* PROCESSING Tab */
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
                              onMarkItemDelivered(
                                order.orderId,
                                item.orderItemId
                              )
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
                      /* PENDING Tab - View Mode or DONE Tab */
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

        <DialogActions sx={{ p: 2, gap: 1 }}>
          {/* PENDING Tab Actions */}
          {currentTab === "PENDING" && (
            <>
              {editMode ? (
                // Edit Mode: Show Cancel, Save Changes, and Save & Approve
                <>
                  <Button
                    onClick={() => onToggleEdit(order.orderId)}
                    disabled={isLoading}
                    color="inherit"
                  >
                    Cancel
                  </Button>
                  <Box sx={{ flex: 1 }} />
                  <Button
                    variant="outlined"
                    onClick={() => onSaveChanges(order.orderId)}
                    disabled={isLoading}
                    startIcon={isLoading ? <CircularProgress size={16} /> : <Save size={16} />}
                  >
                    Save Changes
                  </Button>
                  <Button
                    variant="contained"
                    color="success"
                    onClick={() => onSaveAndApprove(order.orderId)}
                    disabled={isLoading}
                    startIcon={isLoading ? <CircularProgress size={16} /> : <CheckCircle size={16} />}
                  >
                    Save & Approve
                  </Button>
                </>
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

      {/* Cancel Confirmation Dialog */}
      <Dialog
        open={cancelDialogOpen}
        onClose={() => setCancelDialogOpen(false)}
      >
        <DialogTitle>Remove Order Item</DialogTitle>
        <DialogContent>
          <Typography gutterBottom>
            Are you sure you want to to remove this item?
          </Typography>
          <Typography variant="body2" color="text.secondary">
            The order item will be cancelled and cannot be delivered.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setCancelDialogOpen(false)}>No</Button>
          <Button
            onClick={handleConfirmCancel}
            color="error"
            variant="contained"
          >
            Yes, Cancel Item
          </Button>
        </DialogActions>
      </Dialog>

      {/* Undo Snackbar */}
      <Snackbar
        open={undoSnackbarOpen}
        autoHideDuration={6000}
        onClose={() => setUndoSnackbarOpen(false)}
        anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
      >
        <Alert
          severity="info"
          action={
            <Button
              color="inherit"
              size="small"
              startIcon={<RotateCcw size={16} />}
              onClick={handleUndoCancel}
            >
              Undo
            </Button>
          }
        >
          Item canceled. You can undo this action.
        </Alert>
      </Snackbar>
    </>
  );
};
