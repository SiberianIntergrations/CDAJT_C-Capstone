#nullable enable
namespace back_end.domain.enums
{
    // ==============================
    // User
    // ==============================

    /// <summary>
    /// Represents the different roles a user can have in the system.
    /// </summary>
    public enum UserRoles
    {
        /// <summary>
        /// Administrator with full system access.
        /// </summary>
        Admin,

        /// <summary>
        /// Regular customer account.
        /// </summary>
        Customer,

        /// <summary>
        /// Staff or employee role within the organization.
        /// </summary>
        Staff
    }

    /// <summary>
    /// Represents the user's current active state.
    /// </summary>
    public enum UserStatus
    {
        /// <summary>
        /// The user account is active.
        /// </summary>
        Active,

        /// <summary>
        /// The user account is inactive or disabled.
        /// </summary>
        Inactive
    }

    // ==============================
    // Order & Table
    // ==============================

    /// <summary>
    /// Represents the different stages of an order's lifecycle.
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// Order has been created but not yet confirmed.
        /// </summary>
        Pending,

        /// <summary>
        /// Order has been confirmed and is being processed.
        /// </summary>
        Processing, //** Confirmed

        /// <summary>
        /// Order has been fulfilled and delivered.
        /// </summary>
        Delivered, //** Completed

        /// <summary>
        /// Order has been cancelled by staff or customer.
        /// </summary>
        Cancelled
    }

    /// <summary>
    /// Represents the current state of a physical dining table.
    /// </summary>
    public enum TableStatus
    {
        /// <summary>
        /// Table is currently occupied.
        /// </summary>
        InUse,

        /// <summary>
        /// Table is empty and available.
        /// </summary>
        Empty
    }

    // ==============================
    // Menu & Billing
    // ==============================

    /// <summary>
    /// Represents availability of a menu item for customer orders.
    /// </summary>
    public enum MenuItemStatus
    {
        /// <summary>
        /// Item is available to order.
        /// </summary>
        Available,

        /// <summary>
        /// Item is currently unavailable.
        /// </summary>
        Unavailable,

        /// <summary>
        /// Item is seasonal (not always available).
        /// </summary>
        Seasonal //** Not in Python enums.py
    }

    /// <summary>
    /// Represents the current state of a bill.
    /// </summary>
    public enum BillStatus
    {
        /// <summary>
        /// Bill is active and open.
        /// </summary>
        Open,

        /// <summary>
        /// Bill has been paid //** (not present in Python enums.py).
        /// </summary>
        Paid, //** Not in Python enums.py

        /// <summary>
        /// Bill has been fully closed.
        /// </summary>
        Closed,

        /// <summary>
        /// Bill has been cancelled and is no longer active.
        /// </summary>
        Cancelled
    }

    // ==============================
    // System State
    // ==============================

    /// <summary>
    /// Represents WebSocket connection states for active clients.
    /// </summary>
    public enum WebSocketState
    {
        /// <summary>
        /// WebSocket connection is active.
        /// </summary>
        Connected,

        /// <summary>
        /// WebSocket connection is disconnected.
        /// </summary>
        Disconnected,

        /// <summary>
        /// WebSocket connection is inactive (idle timeout).
        /// </summary>
        Inactive,

        /// <summary>
        /// WebSocket connection encountered an error.
        /// </summary>
        Error
    }
}