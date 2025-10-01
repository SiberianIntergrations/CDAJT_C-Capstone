namespace back_end.domain
{
    public enum UserStatus
    {
        Active,
        Inactive,
        Suspended,
        Deleted
    }

    public enum ServiceRequestStatus
    {
        Pending,
        Claimed,
        Completed,
        Cancelled
    }
    public enum UserRoles
    {
        Admin,
        Employee,
        Customer
    }
    public enum OrderStatus
    {
        Pending,
        Processing,
        Delivered,
        Cancelled
    }

    public enum MenuItemStatus
    {
        Available,
        Unavailable,
        Seasonal
    }
    public enum BillStatus
    {
        Open,
        Closed,
        Paid,
        Cancelled
    }
}