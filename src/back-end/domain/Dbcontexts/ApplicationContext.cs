using Microsoft.EntityFrameworkCore;
using back_end.domain.Entities;

namespace back_end.domain.DbContexts
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        public DbSet<Billing> Billings { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;

        public DbSet<DiningSession> DiningSessions { get; set; } = null!;

        public DbSet<Locations> Locations { get; set; } = null!;


        public DbSet<MenuItemAssignment> MenuItemAssignments { get; set; } = null!;
        public DbSet<Menu_Item> MenuItems { get; set; } = null!;

        public DbSet<MenuLocations> MenuLocations { get; set; } = null!;
        public DbSet<Menu> Menus { get; set; } = null!;
        public DbSet<MenuItemTag> MenuItemTags { get; set; } = null!;
        public DbSet<OrderItems> OrderItems { get; set; } = null!;
        public DbSet<ServiceRequest> ServiceRequests { get; set; } = null!;
        public DbSet<SessionOrder> SessionOrders { get; set; } = null!;
        public DbSet<SessionParticipant> SessionParticipants { get; set; } = null!;

        public DbSet<Sessions> Sessions { get; set; } = null!;
        public DbSet<TableEntity> Tables { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Composite keys for junction tables - REQUIRED since EF can't infer these
            modelBuilder.Entity<MenuItemTag>()
                .HasKey(mit => new { mit.Menu_item_id, mit.Tag_id });

            modelBuilder.Entity<MenuItemAssignment>()
                .HasKey(mia => new { mia.Menu_Id, mia.Item_Id });

            modelBuilder.Entity<MenuLocations>()
                .HasKey(ml => new { ml.Menu_Id, ml.Location_Id });

            modelBuilder.Entity<SessionParticipant>()
                .HasKey(sp => new { sp.Session_Id, sp.User_Id });

            modelBuilder.Entity<Sessions>()
                .HasKey(s => new { s.Session_Id, s.Table_Id });

            // Configure ServiceRequest relationships to avoid ambiguity
            modelBuilder.Entity<ServiceRequest>()
                .HasOne(sr => sr.RequestedByUser)
                .WithMany()
                .HasForeignKey(sr => sr.Request_By)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ServiceRequest>()
                .HasOne(sr => sr.ClaimedByUser)
                .WithMany()
                .HasForeignKey(sr => sr.Claimed_By)
                .OnDelete(DeleteBehavior.SetNull);

            // SEED DATA - Added in dependency order
            SeedData(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // 1. Categories (no dependencies)
            modelBuilder.Entity<Category>().HasData(
                new Category { Category_id = 1, Category_name = "Appetizers", Description = "Start your meal with these delicious appetizers" },
                new Category { Category_id = 2, Category_name = "Main Courses", Description = "Hearty main dishes to satisfy your hunger" },
                new Category { Category_id = 3, Category_name = "Desserts", Description = "Sweet treats to end your meal" },
                new Category { Category_id = 4, Category_name = "Beverages", Description = "Refreshing drinks and cocktails" },
                new Category { Category_id = 5, Category_name = "Salads", Description = "Fresh and healthy salad options" }
            );

            // 2. Users (no dependencies)
            modelBuilder.Entity<User>().HasData(
                new User { User_id = 1, First_name = "John", Last_name = "Doe", Email = "john.doe@example.com", Password_hash = "hashed_password_1", Role = UserRoles.Customer },
                new User { User_id = 2, First_name = "Jane", Last_name = "Smith", Email = "jane.smith@example.com", Password_hash = "hashed_password_2", Role = UserRoles.Employee },
                new User { User_id = 3, First_name = "Bob", Last_name = "Johnson", Email = "bob.johnson@example.com", Password_hash = "hashed_password_3", Role = UserRoles.Employee },
                new User { User_id = 4, First_name = "Alice", Last_name = "Wilson", Email = "alice.wilson@example.com", Password_hash = "hashed_password_4", Role = UserRoles.Admin },
                new User { User_id = 5, First_name = "Mike", Last_name = "Brown", Email = "mike.brown@example.com", Password_hash = "hashed_password_5", Role = UserRoles.Customer }
            );

            // 3. Locations (no dependencies)
            modelBuilder.Entity<Locations>().HasData(
                new Locations { Location_Id = 1, Name = "Downtown Branch", Address_Primary = "123 Main St", City = "Edmonton", Province = "Alberta", Postal_Code = "T5K 2P6", Phone_Number = "780-555-0101", Created_At = new DateTime(2024, 1, 1) },
                new Locations { Location_Id = 2, Name = "West End Branch", Address_Primary = "456 West Ave", City = "Edmonton", Province = "Alberta", Postal_Code = "T5L 3R8", Phone_Number = "780-555-0102", Created_At = new DateTime(2024, 1, 1) },
                new Locations { Location_Id = 3, Name = "South Side Branch", Address_Primary = "789 South Rd", City = "Edmonton", Province = "Alberta", Postal_Code = "T6G 1M2", Phone_Number = "780-555-0103", Created_At = new DateTime(2024, 1, 1) }
            );

            // 4. Tables (no dependencies)
            modelBuilder.Entity<TableEntity>().HasData(
                new TableEntity { Table_Id = 1, table_number = 101, seat_count = 4, QR_Code = "QR101", is_active = true },
                new TableEntity { Table_Id = 2, table_number = 102, seat_count = 2, QR_Code = "QR102", is_active = true },
                new TableEntity { Table_Id = 3, table_number = 103, seat_count = 6, QR_Code = "QR103", is_active = true },
                new TableEntity { Table_Id = 4, table_number = 104, seat_count = 4, QR_Code = "QR104", is_active = true },
                new TableEntity { Table_Id = 5, table_number = 105, seat_count = 8, QR_Code = "QR105", is_active = true }
            );

            // 5. Tags (no dependencies)
            modelBuilder.Entity<Tag>().HasData(
                new Tag { tag_id = 1, tag_name = "Vegetarian", tag_color = "#00FF00" },
                new Tag { tag_id = 2, tag_name = "Vegan", tag_color = "#008000" },
                new Tag { tag_id = 3, tag_name = "Gluten-Free", tag_color = "#FFD700" },
                new Tag { tag_id = 4, tag_name = "Spicy", tag_color = "#FF0000" },
                new Tag { tag_id = 5, tag_name = "Popular", tag_color = "#FF69B4" },
                new Tag { tag_id = 6, tag_name = "Organic", tag_color = "#32CD32" }
            );

            // 6. Menu Items (depends on Categories)
            modelBuilder.Entity<Menu_Item>().HasData(
                // Appetizers
                new Menu_Item { item_id = 1, Name = "Buffalo Wings", Description = "Crispy chicken wings with spicy buffalo sauce", Category_id = 1, Status = MenuItemStatus.Available },
                new Menu_Item { item_id = 2, Name = "Mozzarella Sticks", Description = "Golden fried mozzarella with marinara sauce", Category_id = 1, Status = MenuItemStatus.Available },
                new Menu_Item { item_id = 3, Name = "Caesar Salad", Description = "Crisp romaine lettuce with Caesar dressing", Category_id = 5, Status = MenuItemStatus.Available },

                // Main Courses
                new Menu_Item { item_id = 4, Name = "Grilled Salmon", Description = "Fresh Atlantic salmon with lemon herb butter", Category_id = 2, Status = MenuItemStatus.Available },
                new Menu_Item { item_id = 5, Name = "Beef Burger", Description = "Juicy beef patty with all the fixings", Category_id = 2, Status = MenuItemStatus.Available },
                new Menu_Item { item_id = 6, Name = "Vegetarian Pasta", Description = "Penne pasta with seasonal vegetables", Category_id = 2, Status = MenuItemStatus.Available },

                // Desserts
                new Menu_Item { item_id = 7, Name = "Chocolate Cake", Description = "Rich chocolate layer cake", Category_id = 3, Status = MenuItemStatus.Available },
                new Menu_Item { item_id = 8, Name = "Ice Cream Sundae", Description = "Vanilla ice cream with toppings", Category_id = 3, Status = MenuItemStatus.Available },

                // Beverages
                new Menu_Item { item_id = 9, Name = "Craft Beer", Description = "Local brewery selection", Category_id = 4, Status = MenuItemStatus.Available },
                new Menu_Item { item_id = 10, Name = "Fresh Lemonade", Description = "House-made lemonade", Category_id = 4, Status = MenuItemStatus.Available }
            );

            // 7. Menus (no dependencies)
            modelBuilder.Entity<Menu>().HasData(
                new Menu { Menu_id = 1, Name = "Breakfast Menu", Description = "Available until 11 AM", Start_time = new TimeOnly(6, 0), End_time = new TimeOnly(11, 0), Is_active = true },
                new Menu { Menu_id = 2, Name = "Lunch Menu", Description = "Midday favorites", Start_time = new TimeOnly(11, 0), End_time = new TimeOnly(16, 0), Is_active = true },
                new Menu { Menu_id = 3, Name = "Dinner Menu", Description = "Evening specials", Start_time = new TimeOnly(16, 0), End_time = new TimeOnly(22, 0), Is_active = true },
                new Menu { Menu_id = 4, Name = "Weekend Brunch", Description = "Special weekend offerings", Start_time = new TimeOnly(9, 0), End_time = new TimeOnly(15, 0), Is_active = true }
            );

            // 8. Menu Item Assignments (depends on Menu and Menu_Item)
            modelBuilder.Entity<MenuItemAssignment>().HasData(
                // Lunch Menu Items
                new MenuItemAssignment { Menu_Id = 2, Item_Id = 1, Price = 12.99m, Status = MenuItemStatus.Available },
                new MenuItemAssignment { Menu_Id = 2, Item_Id = 2, Price = 8.99m, Status = MenuItemStatus.Available },
                new MenuItemAssignment { Menu_Id = 2, Item_Id = 3, Price = 9.99m, Status = MenuItemStatus.Available },
                new MenuItemAssignment { Menu_Id = 2, Item_Id = 5, Price = 15.99m, Status = MenuItemStatus.Available },

                // Dinner Menu Items
                new MenuItemAssignment { Menu_Id = 3, Item_Id = 1, Price = 13.99m, Status = MenuItemStatus.Available },
                new MenuItemAssignment { Menu_Id = 3, Item_Id = 4, Price = 24.99m, Status = MenuItemStatus.Available },
                new MenuItemAssignment { Menu_Id = 3, Item_Id = 5, Price = 16.99m, Status = MenuItemStatus.Available },
                new MenuItemAssignment { Menu_Id = 3, Item_Id = 6, Price = 18.99m, Status = MenuItemStatus.Available },
                new MenuItemAssignment { Menu_Id = 3, Item_Id = 7, Price = 7.99m, Status = MenuItemStatus.Available },

                // Weekend Brunch Items
                new MenuItemAssignment { Menu_Id = 4, Item_Id = 3, Price = 11.99m, Status = MenuItemStatus.Available },
                new MenuItemAssignment { Menu_Id = 4, Item_Id = 8, Price = 6.99m, Status = MenuItemStatus.Available },
                new MenuItemAssignment { Menu_Id = 4, Item_Id = 9, Price = 5.99m, Status = MenuItemStatus.Available },
                new MenuItemAssignment { Menu_Id = 4, Item_Id = 10, Price = 3.99m, Status = MenuItemStatus.Available }
            );

            // 9. Menu Item Tags (depends on Menu_Item and Tag)
            modelBuilder.Entity<MenuItemTag>().HasData(
                new MenuItemTag { Menu_item_id = 3, Tag_id = 1 }, // Caesar Salad - Vegetarian
                new MenuItemTag { Menu_item_id = 6, Tag_id = 1 }, // Vegetarian Pasta - Vegetarian
                new MenuItemTag { Menu_item_id = 6, Tag_id = 2 }, // Vegetarian Pasta - Vegan
                new MenuItemTag { Menu_item_id = 1, Tag_id = 4 }, // Buffalo Wings - Spicy
                new MenuItemTag { Menu_item_id = 1, Tag_id = 5 }, // Buffalo Wings - Popular
                new MenuItemTag { Menu_item_id = 5, Tag_id = 5 }, // Beef Burger - Popular
                new MenuItemTag { Menu_item_id = 10, Tag_id = 6 } // Fresh Lemonade - Organic
            );

            // 10. Menu Locations (depends on Menu and Locations)
            modelBuilder.Entity<MenuLocations>().HasData(
                new MenuLocations { Menu_Id = 1, Location_Id = 1 }, // Breakfast at Downtown
                new MenuLocations { Menu_Id = 2, Location_Id = 1 }, // Lunch at Downtown
                new MenuLocations { Menu_Id = 3, Location_Id = 1 }, // Dinner at Downtown
                new MenuLocations { Menu_Id = 2, Location_Id = 2 }, // Lunch at West End
                new MenuLocations { Menu_Id = 3, Location_Id = 2 }, // Dinner at West End
                new MenuLocations { Menu_Id = 4, Location_Id = 3 }  // Weekend Brunch at South Side
            );

            // 11. Dining Sessions (depends on Menu)
            modelBuilder.Entity<DiningSession>().HasData(
                new DiningSession { Session_Id = 1, Menu_Id = 2, Started_At = new DateTime(2024, 10, 1, 12, 0, 0), First_Order_At = new DateTime(2024, 10, 1, 12, 15, 0) },
                new DiningSession { Session_Id = 2, Menu_Id = 3, Started_At = new DateTime(2024, 10, 1, 18, 0, 0), First_Order_At = new DateTime(2024, 10, 1, 18, 30, 0) },
                new DiningSession { Session_Id = 3, Menu_Id = 4, Started_At = new DateTime(2024, 10, 2, 10, 0, 0), First_Order_At = new DateTime(2024, 10, 2, 10, 45, 0) }
            );

            // 12. Sessions (depends on DiningSession and TableEntity)
            modelBuilder.Entity<Sessions>().HasData(
                new Sessions { Session_Id = 1, Table_Id = 1 },
                new Sessions { Session_Id = 1, Table_Id = 2 }, // Session 1 uses tables 1 and 2
                new Sessions { Session_Id = 2, Table_Id = 3 },
                new Sessions { Session_Id = 3, Table_Id = 4 }
            );

            // 13. Session Participants (depends on DiningSession and User)
            modelBuilder.Entity<SessionParticipant>().HasData(
                new SessionParticipant { Participant_Id = 1, Session_Id = 1, User_Id = 1, Joined_At = new DateTime(2024, 10, 1, 12, 0, 0) },
                new SessionParticipant { Participant_Id = 2, Session_Id = 1, User_Id = 5, Joined_At = new DateTime(2024, 10, 1, 12, 5, 0) },
                new SessionParticipant { Participant_Id = 3, Session_Id = 2, User_Id = 1, Joined_At = new DateTime(2024, 10, 1, 18, 0, 0) },
                new SessionParticipant { Participant_Id = 4, Session_Id = 3, User_Id = 5, Joined_At = new DateTime(2024, 10, 2, 10, 0, 0) }
            );

            // 14. Billing (depends on DiningSession)
            modelBuilder.Entity<Billing>().HasData(
                new Billing { Bill_Id = 1, Session_Id = 1, Bill_Name = "Table 1-2 Lunch", Adult_Count = 2, Child_Count = 0, Senior_Count = 0, Total_Count = 2, Status = BillStatus.Open, Created_At = new DateTime(2024, 10, 1, 12, 0, 0) },
                new Billing { Bill_Id = 2, Session_Id = 2, Bill_Name = "Table 3 Dinner", Adult_Count = 1, Child_Count = 0, Senior_Count = 0, Total_Count = 1, Status = BillStatus.Open, Created_At = new DateTime(2024, 10, 1, 18, 0, 0) },
                new Billing { Bill_Id = 3, Session_Id = 3, Bill_Name = "Table 4 Brunch", Adult_Count = 1, Child_Count = 0, Senior_Count = 0, Total_Count = 1, Status = BillStatus.Closed, Created_At = new DateTime(2024, 10, 2, 10, 0, 0) }
            );

            // 15. Session Orders (depends on DiningSession, Billing, and User)
            modelBuilder.Entity<SessionOrder>().HasData(
                new SessionOrder { Order_Id = 1, session_id = 1, Bill_Id = 1, User_Id = 1, Status = OrderStatus.Delivered, Created_At = new DateTime(2024, 10, 1, 12, 15, 0), Completed_At = new DateTime(2024, 10, 1, 12, 45, 0) },
                new SessionOrder { Order_Id = 2, session_id = 1, Bill_Id = 1, User_Id = 5, Status = OrderStatus.Delivered, Created_At = new DateTime(2024, 10, 1, 12, 20, 0), Completed_At = new DateTime(2024, 10, 1, 12, 50, 0) },
                new SessionOrder { Order_Id = 3, session_id = 2, Bill_Id = 2, User_Id = 1, Status = OrderStatus.Processing, Created_At = new DateTime(2024, 10, 1, 18, 30, 0) },
                new SessionOrder { Order_Id = 4, session_id = 3, Bill_Id = 3, User_Id = 5, Status = OrderStatus.Delivered, Created_At = new DateTime(2024, 10, 2, 10, 45, 0), Completed_At = new DateTime(2024, 10, 2, 11, 15, 0) }
            );

            // 16. Order Items (depends on SessionOrder, Menu, and Menu_Item)
            modelBuilder.Entity<OrderItems>().HasData(
                new OrderItems { Order_Item_Id = 1, Order_Key = 1, Menu_Id = 2, Item_Id = 1, Quantity = 1, Price_At_Time = 12.99m, Order_Item_Status = OrderStatus.Delivered, Completed_At = new DateTime(2024, 10, 1, 12, 45, 0) },
                new OrderItems { Order_Item_Id = 2, Order_Key = 1, Menu_Id = 2, Item_Id = 3, Quantity = 1, Price_At_Time = 9.99m, Order_Item_Status = OrderStatus.Delivered, Completed_At = new DateTime(2024, 10, 1, 12, 45, 0) },
                new OrderItems { Order_Item_Id = 3, Order_Key = 2, Menu_Id = 2, Item_Id = 5, Quantity = 1, Price_At_Time = 15.99m, Order_Item_Status = OrderStatus.Delivered, Completed_At = new DateTime(2024, 10, 1, 12, 50, 0) },
                new OrderItems { Order_Item_Id = 4, Order_Key = 3, Menu_Id = 3, Item_Id = 4, Quantity = 1, Price_At_Time = 24.99m, Order_Item_Status = OrderStatus.Processing },
                new OrderItems { Order_Item_Id = 5, Order_Key = 4, Menu_Id = 4, Item_Id = 8, Quantity = 2, Price_At_Time = 6.99m, Order_Item_Status = OrderStatus.Delivered, Completed_At = new DateTime(2024, 10, 2, 11, 15, 0) }
            );

            // 17. Service Requests (depends on DiningSession, TableEntity, and User)
            modelBuilder.Entity<ServiceRequest>().HasData(
                new ServiceRequest { request_id = 1, Session_Id = 1, Table_Id = 1, Request_By = 1, Notes = "Need extra napkins", Status = ServiceRequestStatus.Completed, Created_At = new DateTime(2024, 10, 1, 12, 30, 0), Claimed_By = 2, Claimed_At = new DateTime(2024, 10, 1, 12, 32, 0), Completed_At = new DateTime(2024, 10, 1, 12, 35, 0) },
                new ServiceRequest { request_id = 2, Session_Id = 2, Table_Id = 3, Request_By = 1, Notes = "Check on food order", Status = ServiceRequestStatus.Pending, Created_At = new DateTime(2024, 10, 1, 18, 45, 0) },
                new ServiceRequest { request_id = 3, Session_Id = 3, Table_Id = 4, Request_By = 5, Notes = "Request the bill", Status = ServiceRequestStatus.Completed, Created_At = new DateTime(2024, 10, 2, 11, 0, 0), Claimed_By = 3, Claimed_At = new DateTime(2024, 10, 2, 11, 2, 0), Completed_At = new DateTime(2024, 10, 2, 11, 10, 0) }
            );
        }

    }

}