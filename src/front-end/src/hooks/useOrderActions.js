import { useState } from "react";
import { createApiWrapper } from "@/utils/apiWrapper";
import orderService from "@/services/orderService";

/**
 * Hook for managing order operations with loading and error states
 * @param {Function} onSuccess - Callback to run on successful operations
 * @returns {Object} Order action functions and state
 */
export const useOrderActions = (onSuccess) => {
  const [actionError, setActionError] = useState(null);
  const [isLoading, setIsLoading] = useState(false);

  const handleError = (err, context) => {
    const errorMessage =
      err?.response?.data?.message ||
      err?.response?.data?.detail ||
      err?.message ||
      `Failed to ${context}`;
    
    console.error(`Error in ${context}:`, err);
    setActionError(errorMessage);
  };

  const apiWrapper = createApiWrapper({
    setIsLoading,
    setActionError,
    handleError,
    onSuccess,
  });

  // ORDER CREATION

  const createOrder = (sessionId, billId) =>
    apiWrapper(
      "createOrder",
      async () => orderService.createOrder({ session_Id: sessionId, bill_Id: billId }),
      { triggerSuccess: true }
    );

  // ORDER MANAGEMENT

  const approveOrder = (orderId) =>
    apiWrapper(
      "approveOrder",
      async () => orderService.approveOrder(orderId),
      { triggerSuccess: true }
    );

  const cancelOrder = (orderId) =>
    apiWrapper(
      "cancelOrder",
      async () => orderService.cancelOrder(orderId),
      { triggerSuccess: true }
    );

  const completeOrder = (orderId) =>
    apiWrapper(
      "completeOrder",
      async () => orderService.completeOrder(orderId),
      { triggerSuccess: true }
    );

  // ORDER ITEMS MANAGEMENT

  const addOrderItems = (orderId, items) =>
    apiWrapper(
      "addOrderItems",
      async () => orderService.addOrderItems(orderId, items),
      { triggerSuccess: true }
    );

  const updateOrderItem = (orderId, itemId, updateData) =>
    apiWrapper(
      "updateOrderItem",
      async () => orderService.updateOrderItem(orderId, itemId, updateData),
      { triggerSuccess: true }
    );

  const completeOrderItem = (orderId, itemId) =>
    apiWrapper(
      "completeOrderItem",
      async () => orderService.completeOrderItem(orderId, itemId),
      { triggerSuccess: true }
    );

  const removeOrderItem = (orderId, orderItemId) =>
    apiWrapper(
      "removeOrderItem",
      async () => orderService.removeOrderItem(orderId, orderItemId),
      { triggerSuccess: true }
    );

  // ORDER RETRIEVAL

  const getAllOrders = () =>
    apiWrapper(
      "getAllOrders",
      async () => orderService.getAllOrders(),
      { defaultReturn: [] }
    );

  const getOrderById = (orderId) =>
    apiWrapper(
      "getOrderById",
      async () => orderService.getOrderById(orderId)
    );

  const getOrdersBySession = (sessionId) =>
    apiWrapper(
      "getOrdersBySession",
      async () => orderService.getOrdersBySession(sessionId),
      { defaultReturn: [] }
    );

  const getOrdersByUser = (userId) =>
    apiWrapper(
      "getOrdersByUser",
      async () => orderService.getOrdersByUser(userId),
      { defaultReturn: [] }
    );

  const getActiveSessionOrders = (billId = null) =>
    apiWrapper(
      "getActiveSessionOrders",
      async () => orderService.getActiveSessionOrders(billId),
      { defaultReturn: [] }
    );

  const clearActionError = () => setActionError(null);

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