import { useState, useCallback } from "react";
import { createApiWrapper } from "@/utils/apiWrapper";
import { useNotification } from "@/contexts/NotificationContext";
import orderService from "@/services/orderService";

/**
 * Hook for managing order operations with loading and error states
 * @param {Function} onSuccess - Callback to run on successful operations
 * @returns {Object} Order action functions and state
 */
export const useOrderActions = (onSuccess) => {
  const [actionError, setActionError] = useState(null);
  const [isLoading, setIsLoading] = useState(false);

  const { notifySuccess, notifyError } = useNotification();

  const clearActionError = useCallback(() => {
    setActionError(null);
  }, []);

  // Centralized error handler
  const handleError = useCallback(
    (error, context) => {
      const errorMessage = error.userMessage || `Failed to ${context}`;

      setActionError(errorMessage);
      notifyError(errorMessage);
    },
    [notifyError]
  );

  // Wrapper that handles all the repetitive loading/error logic
  const apiWrapper = useCallback(
    createApiWrapper({
      setIsLoading,
      setActionError,
      handleError,
      onSuccess,
    }),
    [handleError, onSuccess]
  );

  // ORDER CREATION

  const createOrder = useCallback(
    (sessionId, billId) =>
      apiWrapper(
        "createOrder",
        async () => {
          const result = await orderService.createOrder({
            session_Id: sessionId,
            bill_Id: billId,
          });
          return result;
        },
        { triggerSuccess: true }
      ),
    [apiWrapper] // Removed notifySuccess to avoid duplicate notifications. Create order in full-menu triggers its own success message with Alert.
  );

  // ORDER MANAGEMENT

  const approveOrder = useCallback(
    (orderId) =>
      apiWrapper(
        "approveOrder",
        async () => {
          const result = await orderService.approveOrder(orderId);
          notifySuccess("Order approved successfully");
          return result;
        },
        { triggerSuccess: true }
      ),
    [apiWrapper, notifySuccess]
  );

  const cancelOrder = useCallback(
    (orderId) =>
      apiWrapper(
        "cancelOrder",
        async () => {
          const result = await orderService.cancelOrder(orderId);
          notifySuccess("Order cancelled successfully");
          return result;
        },
        { triggerSuccess: true }
      ),
    [apiWrapper, notifySuccess]
  );

  const completeOrder = useCallback(
    (orderId) =>
      apiWrapper(
        "completeOrder",
        async () => {
          const result = await orderService.completeOrder(orderId);
          notifySuccess("Order completed successfully");
          return result;
        },
        { triggerSuccess: true }
      ),
    [apiWrapper, notifySuccess]
  );

  // ORDER ITEMS MANAGEMENT

  const addOrderItems = useCallback(
    (orderId, items) =>
      apiWrapper(
        "addOrderItems",
        async () => {
          const result = await orderService.addOrderItems(orderId, items);
          notifySuccess(
            `${items.length} item${items.length > 1 ? "s" : ""} added to order`
          );
          return result;
        },
        { triggerSuccess: true }
      ),
    [apiWrapper, notifySuccess]
  );

  const updateOrderItem = useCallback(
    (orderId, itemId, updateData) =>
      apiWrapper(
        "updateOrderItem",
        async () => {
          const result = await orderService.updateOrderItem(
            orderId,
            itemId,
            updateData
          );
          notifySuccess("Order item updated successfully");
          return result;
        },
        { triggerSuccess: true }
      ),
    [apiWrapper, notifySuccess]
  );

  const completeOrderItem = useCallback(
    (orderId, itemId) =>
      apiWrapper(
        "completeOrderItem",
        async () => {
          const result = await orderService.completeOrderItem(orderId, itemId);
          notifySuccess("Order item marked as complete");
          return result;
        },
        { triggerSuccess: true }
      ),
    [apiWrapper, notifySuccess]
  );

  const removeOrderItem = useCallback(
    (orderId, orderItemId) =>
      apiWrapper(
        "removeOrderItem",
        async () => {
          const result = await orderService.removeOrderItem(
            orderId,
            orderItemId
          );
          notifySuccess("Order item removed successfully");
          return result;
        },
        { triggerSuccess: true }
      ),
    [apiWrapper, notifySuccess]
  );

  // ORDER RETRIEVAL

  const getAllOrders = useCallback(
    () =>
      apiWrapper(
        "getAllOrders",
        async () => orderService.getAllOrders(),
        { defaultReturn: [] }
      ),
    [apiWrapper]
  );

  const getOrderById = useCallback(
    (orderId) =>
      apiWrapper("getOrderById", async () => orderService.getOrderById(orderId)),
    [apiWrapper]
  );

  const getOrdersBySession = useCallback(
    (sessionId) =>
      apiWrapper(
        "getOrdersBySession",
        async () => orderService.getOrdersBySession(sessionId),
        { defaultReturn: [] }
      ),
    [apiWrapper]
  );

  const getOrdersByUser = useCallback(
    (userId) =>
      apiWrapper(
        "getOrdersByUser",
        async () => orderService.getOrdersByUser(userId),
        { defaultReturn: [] }
      ),
    [apiWrapper]
  );

  const getActiveSessionOrders = useCallback(
    (billId = null) =>
      apiWrapper(
        "getActiveSessionOrders",
        async () => orderService.getActiveSessionOrders(billId),
        { defaultReturn: [] }
      ),
    [apiWrapper]
  );

  return {
    // State
    actionError,
    isLoading,
    clearActionError,

    // Order Management
    createOrder,
    approveOrder,
    cancelOrder,
    completeOrder,

    // Order Items
    addOrderItems,
    updateOrderItem,
    completeOrderItem,
    removeOrderItem,

    // Order Retrieval
    getAllOrders,
    getOrderById,
    getOrdersBySession,
    getOrdersByUser,
    getActiveSessionOrders,
  };
};

export default useOrderActions;