/**
 * Format a date to a readable time string
 */
export const formatTime = (dateString) => {
  try {
    return new Date(dateString).toLocaleTimeString([], {
      hour: "2-digit",
      minute: "2-digit",
    });
  } catch (error) {
    console.error("Error formatting time:", error);
    return "Invalid time";
  }
};

/**
 * Format UTC date to local date and time
 */
export const formatDateTime = (dateString) => {
  try {
    const date = new Date(dateString);
    return date.toLocaleString([], {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  } catch (error) {
    console.error("Error formatting datetime:", error);
    return "Invalid date";
  }
};

/**
 * Format just the date
 */
export const formatDate = (dateString) => {
  try {
    const date = new Date(dateString);
    return date.toLocaleDateString([], {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  } catch (error) {
    console.error("Error formatting date:", error);
    return "Invalid date";
  }
};

/**
 * Check if a session can be closed
 */
export const canCloseSession = (session) => {
  // Session can be closed if all bills are closed or cancelled
  return session.bills.every(
    (bill) => bill.status === "CLOSED" || bill.status === "CANCELLED"
  );
};

/**
 * Format bill name with guest count
 */
export const formatBillName = (bill) => {
  const total = bill.total_count || 0;
  return `${bill.bill_name} (${total} guest${total !== 1 ? 's' : ''})`;
};

/**
 * Get status color for a bill
 */
export const getBillStatusColor = (status) => {
  switch (status) {
    case "OPEN":
      return "primary";
    case "CLOSED":
      return "default";
    case "CANCELLED":
      return "error";
    default:
      return "default";
  }
};

/**
 * Check if a session has any pending bills
 */
export const hasOpenBills = (session) => {
  return session.bills.some((bill) => bill.status === "OPEN");
};

/**
 * Check if a session has reached maximum capacity
 */
export const isAtCapacity = (session, maxCapacity = 20) => {
  const totalGuests = session.bills.reduce(
    (sum, bill) => sum + (bill.total_count || 0),
    0
  );
  return totalGuests >= maxCapacity;
};

/**
 * Get table assignment description
 */
export const getTableDescription = (tableNumbers) => {
  if (!tableNumbers || tableNumbers.length === 0) return "No tables assigned";
  if (tableNumbers.length === 1) return `Table ${tableNumbers[0]}`;
  return `Tables ${tableNumbers.join(", ")}`;
};

/**
 * Sort bills by status (OPEN first, then CLOSED, then CANCELLED)
 */
export const sortBillsByStatus = (bills) => {
  const statusOrder = {
    OPEN: 0,
    CLOSED: 1,
    CANCELLED: 2,
  };

  return [...bills].sort(
    (a, b) => statusOrder[a.status] - statusOrder[b.status]
  );
};

/**
 * Format bill status for display
 */
export const formatBillStatus = (status) => {
  switch (status) {
    case "OPEN":
      return "Active";
    case "CLOSED":
      return "Completed";
    case "CANCELLED":
      return "Cancelled";
    default:
      return status;
  }
};

/**
 * Check if a table can be removed from a session
 */
export const canRemoveTable = (session, tableNumber) => {
  // Can't remove tables if there are any bills
  if (session.bills && session.bills.length > 0) return false;

  // Can't remove the last table
  if (session.table_Numbers.length <= 1) return false;

  return true;
};

/**
 * Get session duration
 */
export const getSessionDuration = (startTime, endTime = null) => {
  const start = new Date(startTime);
  const end = endTime ? new Date(endTime) : new Date();
  const diff = end - start;
  const minutes = Math.floor(diff / 1000 / 60);
  const hours = Math.floor(minutes / 60);

  if (hours > 0) {
    return `${hours}h ${minutes % 60}m`;
  }
  return `${minutes}m`;
};

/**
 * Format currency amount
 */
export const formatCurrency = (amount) => {
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD",
  }).format(amount);
};

/**
 * Format pricing type for display
 */
export const formatPricingType = (pricingType) => {
  switch (pricingType) {
    case "Weekday":
      return "Weekday Pricing";
    case "Weekend":
      return "Weekend Pricing";
    case "Holiday":
      return "Holiday Pricing";
    default:
      return pricingType;
  }
};

/**
 * Format guest breakdown for display
 */
export const formatGuestBreakdown = (bill) => {
  const parts = [];
  if (bill.adult_count > 0) parts.push(`${bill.adult_count} Adult${bill.adult_count > 1 ? 's' : ''}`);
  if (bill.senior_count > 0) parts.push(`${bill.senior_count} Senior${bill.senior_count > 1 ? 's' : ''}`);
  if (bill.child_count > 0) parts.push(`${bill.child_count > 1 ? 'ren' : ''}`);
  if (bill.tot_count > 0) parts.push(`${bill.tot_count} Toddler${bill.tot_count > 1 ? 's' : ''}`);
  
  return parts.length > 0 ? parts.join(', ') : 'No guests';
};

/**
 * Validate bill data before creation
 */
export const validateBillData = (billData) => {
  const errors = {};

  if (!billData.billName?.trim()) {
    errors.billName = "Bill name is required";
  }

  const totalGuests =
    (Number(billData.adultCount) || 0) +
    (Number(billData.childCount) || 0) +
    (Number(billData.seniorCount) || 0) +
    (Number(billData.totCount) || 0);

  if (totalGuests <= 0) {
    errors.guests = "At least one guest is required";
  }

  return {
    isValid: Object.keys(errors).length === 0,
    errors,
  };
};

/**
 * Group bills by status
 */
export const groupBillsByStatus = (bills) => {
  return bills.reduce((acc, bill) => {
    if (!acc[bill.status]) {
      acc[bill.status] = [];
    }
    acc[bill.status].push(bill);
    return acc;
  }, {});
};
