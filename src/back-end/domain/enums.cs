namespace back_end.domain
{
    public enum UserRoles
    {
        Admin,
        Manager,
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