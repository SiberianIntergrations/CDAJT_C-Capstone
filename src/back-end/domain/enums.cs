namespace back_end.domain
{
    // ==============================
    // User
    // ==============================

    public enum UserRoles
    // Represents the different roles a user can have in the system.
    {
        Admin,
        Customer,
        Staff
    }

    public enum UserStatus
    // Represents the user's active state.
    {
        Active,
        Inactive
    }

    // ==============================
    // Order & Table
    // ==============================

    public enum OrderStatus
    // Represents the different stages of an order's lifecycle.
    {
        Pending, // Order has been created but not yet confirmed.
        Processing, //** Confirmed  // Order has been confirmed and is being processed.
        Delivered, //** Completed // Order has been fulfilled.
        Cancelled // Order has been cancelled.
    }

    public enum TableStatus
    // Represents physical dining table states.
    {
        InUse,
        Empty
    }

    // ==============================
    // Menu & Billing
    // ==============================

    public enum MenuItemStatus
    // Represents availability of a menu item for customer to order.
    {
        Available,
        Unavailable,
        Seasonal //** Not in Python emuns.py
    }

    public enum BillStatus 
    // Represents the current state of a bill.
    {
        Open, // Bill is active.
        Paid, //** Not in Python emuns.py
        Closed, // Bill has been fully closed.
        Cancelled // Bill has been cancelled and is no longer active.
    }

    // ==============================
    // System State
    // ==============================

    public enum WebSocketState
    // WebSocket connection states.
    {
        Connected,
        Disconnected,
        Inactive,
        Error
    }
}