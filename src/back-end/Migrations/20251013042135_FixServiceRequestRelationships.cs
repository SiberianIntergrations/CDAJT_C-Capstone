using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace back_end.Migrations
{
    /// <inheritdoc />
    public partial class FixServiceRequestRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_service_request_User_User_id",
                table: "service_request");

            migrationBuilder.DropForeignKey(
                name: "FK_service_request_User_User_id1",
                table: "service_request");

            migrationBuilder.DropIndex(
                name: "IX_service_request_User_id",
                table: "service_request");

            migrationBuilder.DropIndex(
                name: "IX_service_request_User_id1",
                table: "service_request");

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "User_id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "menu_item_assignment",
                keyColumns: new[] { "item_id", "menu_id" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "menu_item_assignment",
                keyColumns: new[] { "item_id", "menu_id" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "menu_item_assignment",
                keyColumns: new[] { "item_id", "menu_id" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.DeleteData(
                table: "menu_item_assignment",
                keyColumns: new[] { "item_id", "menu_id" },
                keyValues: new object[] { 5, 2 });

            migrationBuilder.DeleteData(
                table: "menu_item_assignment",
                keyColumns: new[] { "item_id", "menu_id" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "menu_item_assignment",
                keyColumns: new[] { "item_id", "menu_id" },
                keyValues: new object[] { 4, 3 });

            migrationBuilder.DeleteData(
                table: "menu_item_assignment",
                keyColumns: new[] { "item_id", "menu_id" },
                keyValues: new object[] { 5, 3 });

            migrationBuilder.DeleteData(
                table: "menu_item_assignment",
                keyColumns: new[] { "item_id", "menu_id" },
                keyValues: new object[] { 6, 3 });

            migrationBuilder.DeleteData(
                table: "menu_item_assignment",
                keyColumns: new[] { "item_id", "menu_id" },
                keyValues: new object[] { 7, 3 });

            migrationBuilder.DeleteData(
                table: "menu_item_assignment",
                keyColumns: new[] { "item_id", "menu_id" },
                keyValues: new object[] { 3, 4 });

            migrationBuilder.DeleteData(
                table: "menu_item_assignment",
                keyColumns: new[] { "item_id", "menu_id" },
                keyValues: new object[] { 8, 4 });

            migrationBuilder.DeleteData(
                table: "menu_item_assignment",
                keyColumns: new[] { "item_id", "menu_id" },
                keyValues: new object[] { 9, 4 });

            migrationBuilder.DeleteData(
                table: "menu_item_assignment",
                keyColumns: new[] { "item_id", "menu_id" },
                keyValues: new object[] { 10, 4 });

            migrationBuilder.DeleteData(
                table: "menu_item_tag",
                keyColumns: new[] { "Menu_item_id", "Tag_id" },
                keyValues: new object[] { 1, 4 });

            migrationBuilder.DeleteData(
                table: "menu_item_tag",
                keyColumns: new[] { "Menu_item_id", "Tag_id" },
                keyValues: new object[] { 1, 5 });

            migrationBuilder.DeleteData(
                table: "menu_item_tag",
                keyColumns: new[] { "Menu_item_id", "Tag_id" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "menu_item_tag",
                keyColumns: new[] { "Menu_item_id", "Tag_id" },
                keyValues: new object[] { 5, 5 });

            migrationBuilder.DeleteData(
                table: "menu_item_tag",
                keyColumns: new[] { "Menu_item_id", "Tag_id" },
                keyValues: new object[] { 6, 1 });

            migrationBuilder.DeleteData(
                table: "menu_item_tag",
                keyColumns: new[] { "Menu_item_id", "Tag_id" },
                keyValues: new object[] { 6, 2 });

            migrationBuilder.DeleteData(
                table: "menu_item_tag",
                keyColumns: new[] { "Menu_item_id", "Tag_id" },
                keyValues: new object[] { 10, 6 });

            migrationBuilder.DeleteData(
                table: "menu_location",
                keyColumns: new[] { "Location_Id", "Menu_Id" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "menu_location",
                keyColumns: new[] { "Location_Id", "Menu_Id" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "menu_location",
                keyColumns: new[] { "Location_Id", "Menu_Id" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "menu_location",
                keyColumns: new[] { "Location_Id", "Menu_Id" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "menu_location",
                keyColumns: new[] { "Location_Id", "Menu_Id" },
                keyValues: new object[] { 2, 3 });

            migrationBuilder.DeleteData(
                table: "menu_location",
                keyColumns: new[] { "Location_Id", "Menu_Id" },
                keyValues: new object[] { 3, 4 });

            migrationBuilder.DeleteData(
                table: "order_item",
                keyColumn: "Order_Item_Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "order_item",
                keyColumn: "Order_Item_Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "order_item",
                keyColumn: "Order_Item_Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "order_item",
                keyColumn: "Order_Item_Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "order_item",
                keyColumn: "Order_Item_Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "service_request",
                keyColumn: "request_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "service_request",
                keyColumn: "request_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "service_request",
                keyColumn: "request_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "session_participant",
                keyColumns: new[] { "Session_Id", "User_Id" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "session_participant",
                keyColumns: new[] { "Session_Id", "User_Id" },
                keyValues: new object[] { 1, 5 });

            migrationBuilder.DeleteData(
                table: "session_participant",
                keyColumns: new[] { "Session_Id", "User_Id" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "session_participant",
                keyColumns: new[] { "Session_Id", "User_Id" },
                keyValues: new object[] { 3, 5 });

            migrationBuilder.DeleteData(
                table: "sessions",
                keyColumns: new[] { "Session_Id", "Table_Id" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "sessions",
                keyColumns: new[] { "Session_Id", "Table_Id" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "sessions",
                keyColumns: new[] { "Session_Id", "Table_Id" },
                keyValues: new object[] { 2, 3 });

            migrationBuilder.DeleteData(
                table: "sessions",
                keyColumns: new[] { "Session_Id", "Table_Id" },
                keyValues: new object[] { 3, 4 });

            migrationBuilder.DeleteData(
                table: "table_entity",
                keyColumn: "Table_Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "tag",
                keyColumn: "tag_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "User_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "User_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "locations",
                keyColumn: "Location_Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "locations",
                keyColumn: "Location_Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "locations",
                keyColumn: "Location_Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "menu",
                keyColumn: "Menu_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "menu_item",
                keyColumn: "item_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "menu_item",
                keyColumn: "item_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "menu_item",
                keyColumn: "item_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "menu_item",
                keyColumn: "item_id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "menu_item",
                keyColumn: "item_id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "menu_item",
                keyColumn: "item_id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "menu_item",
                keyColumn: "item_id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "menu_item",
                keyColumn: "item_id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "menu_item",
                keyColumn: "item_id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "menu_item",
                keyColumn: "item_id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "session_order",
                keyColumn: "Order_Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "session_order",
                keyColumn: "Order_Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "session_order",
                keyColumn: "Order_Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "session_order",
                keyColumn: "Order_Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "table_entity",
                keyColumn: "Table_Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "table_entity",
                keyColumn: "Table_Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "table_entity",
                keyColumn: "Table_Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "table_entity",
                keyColumn: "Table_Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "tag",
                keyColumn: "tag_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "tag",
                keyColumn: "tag_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "tag",
                keyColumn: "tag_id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "tag",
                keyColumn: "tag_id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "tag",
                keyColumn: "tag_id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Category_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Category_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Category_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Category_id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Category_id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "User_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "User_id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "bills",
                keyColumn: "Bill_Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "bills",
                keyColumn: "Bill_Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "bills",
                keyColumn: "Bill_Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "dining_sessions",
                keyColumn: "Session_Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "dining_sessions",
                keyColumn: "Session_Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "dining_sessions",
                keyColumn: "Session_Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "menu",
                keyColumn: "Menu_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "menu",
                keyColumn: "Menu_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "menu",
                keyColumn: "Menu_id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "User_id",
                table: "service_request");

            migrationBuilder.DropColumn(
                name: "User_id1",
                table: "service_request");

            migrationBuilder.AddColumn<int>(
                name: "DiningSessionSession_Id",
                table: "table_entity",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Closed_At",
                table: "bills",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_table_entity_DiningSessionSession_Id",
                table: "table_entity",
                column: "DiningSessionSession_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_table_entity_dining_sessions_DiningSessionSession_Id",
                table: "table_entity",
                column: "DiningSessionSession_Id",
                principalTable: "dining_sessions",
                principalColumn: "Session_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_table_entity_dining_sessions_DiningSessionSession_Id",
                table: "table_entity");

            migrationBuilder.DropIndex(
                name: "IX_table_entity_DiningSessionSession_Id",
                table: "table_entity");

            migrationBuilder.DropColumn(
                name: "DiningSessionSession_Id",
                table: "table_entity");

            migrationBuilder.AddColumn<int>(
                name: "User_id",
                table: "service_request",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "User_id1",
                table: "service_request",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Closed_At",
                table: "bills",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Category_id", "Category_name", "Description", "adult_limit", "child_limit", "image_url", "last_viewed_at", "senior_limit", "total_limit", "total_view_seconds", "total_views" },
                values: new object[,]
                {
                    { 1, "Appetizers", "Start your meal with these delicious appetizers", 0, 0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 0, 0, 0 },
                    { 2, "Main Courses", "Hearty main dishes to satisfy your hunger", 0, 0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 0, 0, 0 },
                    { 3, "Desserts", "Sweet treats to end your meal", 0, 0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 0, 0, 0 },
                    { 4, "Beverages", "Refreshing drinks and cocktails", 0, 0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 0, 0, 0 },
                    { 5, "Salads", "Fresh and healthy salad options", 0, 0, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 0, 0, 0 }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "User_id", "Created_at", "Email", "First_name", "Is_email_confirmed", "Last_Interaction_at", "Last_name", "Normalized_email", "Password_hash", "Role", "Status" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 5, 9, 24, 1, 126, DateTimeKind.Local).AddTicks(2579), "john.doe@example.com", "John", false, null, "Doe", "JOHN.DOE@EXAMPLE.COM", "hashed_password_1", 2, 0 },
                    { 2, new DateTime(2025, 10, 5, 9, 24, 1, 126, DateTimeKind.Local).AddTicks(3622), "jane.smith@example.com", "Jane", false, null, "Smith", "JANE.SMITH@EXAMPLE.COM", "hashed_password_2", 1, 0 },
                    { 3, new DateTime(2025, 10, 5, 9, 24, 1, 126, DateTimeKind.Local).AddTicks(3635), "bob.johnson@example.com", "Bob", false, null, "Johnson", "BOB.JOHNSON@EXAMPLE.COM", "hashed_password_3", 1, 0 },
                    { 4, new DateTime(2025, 10, 5, 9, 24, 1, 126, DateTimeKind.Local).AddTicks(3638), "alice.wilson@example.com", "Alice", false, null, "Wilson", "ALICE.WILSON@EXAMPLE.COM", "hashed_password_4", 0, 0 },
                    { 5, new DateTime(2025, 10, 5, 9, 24, 1, 126, DateTimeKind.Local).AddTicks(3641), "mike.brown@example.com", "Mike", false, null, "Brown", "MIKE.BROWN@EXAMPLE.COM", "hashed_password_5", 2, 0 }
                });

            migrationBuilder.InsertData(
                table: "locations",
                columns: new[] { "Location_Id", "Address_Primary", "Address_Secondary", "City", "Created_At", "Name", "Phone_Number", "Postal_Code", "Province", "Updated_At" },
                values: new object[,]
                {
                    { 1, "123 Main St", "", "Edmonton", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Downtown Branch", "780-555-0101", "T5K 2P6", "Alberta", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "456 West Ave", "", "Edmonton", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "West End Branch", "780-555-0102", "T5L 3R8", "Alberta", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "789 South Rd", "", "Edmonton", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "South Side Branch", "780-555-0103", "T6G 1M2", "Alberta", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "menu",
                columns: new[] { "Menu_id", "Description", "End_time", "Is_active", "Name", "Start_time" },
                values: new object[,]
                {
                    { 1, "Available until 11 AM", new TimeOnly(11, 0, 0), true, "Breakfast Menu", new TimeOnly(6, 0, 0) },
                    { 2, "Midday favorites", new TimeOnly(16, 0, 0), true, "Lunch Menu", new TimeOnly(11, 0, 0) },
                    { 3, "Evening specials", new TimeOnly(22, 0, 0), true, "Dinner Menu", new TimeOnly(16, 0, 0) },
                    { 4, "Special weekend offerings", new TimeOnly(15, 0, 0), true, "Weekend Brunch", new TimeOnly(9, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "table_entity",
                columns: new[] { "Table_Id", "QR_Code", "is_active", "seat_count", "table_number" },
                values: new object[,]
                {
                    { 1, "QR101", true, 4, 101 },
                    { 2, "QR102", true, 2, 102 },
                    { 3, "QR103", true, 6, 103 },
                    { 4, "QR104", true, 4, 104 },
                    { 5, "QR105", true, 8, 105 }
                });

            migrationBuilder.InsertData(
                table: "tag",
                columns: new[] { "tag_id", "tag_color", "tag_name" },
                values: new object[,]
                {
                    { 1, "#00FF00", "Vegetarian" },
                    { 2, "#008000", "Vegan" },
                    { 3, "#FFD700", "Gluten-Free" },
                    { 4, "#FF0000", "Spicy" },
                    { 5, "#FF69B4", "Popular" },
                    { 6, "#32CD32", "Organic" }
                });

            migrationBuilder.InsertData(
                table: "dining_sessions",
                columns: new[] { "Session_Id", "Ended_At", "First_Order_At", "Menu_Id", "Started_At" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2024, 10, 1, 12, 15, 0, 0, DateTimeKind.Unspecified), 2, new DateTime(2024, 10, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, null, new DateTime(2024, 10, 1, 18, 30, 0, 0, DateTimeKind.Unspecified), 3, new DateTime(2024, 10, 1, 18, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, null, new DateTime(2024, 10, 2, 10, 45, 0, 0, DateTimeKind.Unspecified), 4, new DateTime(2024, 10, 2, 10, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "menu_item",
                columns: new[] { "item_id", "Category_id", "Description", "Name", "Status", "image_url" },
                values: new object[,]
                {
                    { 1, 1, "Crispy chicken wings with spicy buffalo sauce", "Buffalo Wings", 0, null },
                    { 2, 1, "Golden fried mozzarella with marinara sauce", "Mozzarella Sticks", 0, null },
                    { 3, 5, "Crisp romaine lettuce with Caesar dressing", "Caesar Salad", 0, null },
                    { 4, 2, "Fresh Atlantic salmon with lemon herb butter", "Grilled Salmon", 0, null },
                    { 5, 2, "Juicy beef patty with all the fixings", "Beef Burger", 0, null },
                    { 6, 2, "Penne pasta with seasonal vegetables", "Vegetarian Pasta", 0, null },
                    { 7, 3, "Rich chocolate layer cake", "Chocolate Cake", 0, null },
                    { 8, 3, "Vanilla ice cream with toppings", "Ice Cream Sundae", 0, null },
                    { 9, 4, "Local brewery selection", "Craft Beer", 0, null },
                    { 10, 4, "House-made lemonade", "Fresh Lemonade", 0, null }
                });

            migrationBuilder.InsertData(
                table: "menu_location",
                columns: new[] { "Location_Id", "Menu_Id" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 2, 2 },
                    { 1, 3 },
                    { 2, 3 },
                    { 3, 4 }
                });

            migrationBuilder.InsertData(
                table: "bills",
                columns: new[] { "Bill_Id", "Adult_Count", "Bill_Name", "Child_Count", "Closed_At", "Created_At", "Senior_Count", "Session_Id", "Status", "Total_Count" },
                values: new object[,]
                {
                    { 1, 2, "Table 1-2 Lunch", 0, null, new DateTime(2024, 10, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), 0, 1, 0, 2 },
                    { 2, 1, "Table 3 Dinner", 0, null, new DateTime(2024, 10, 1, 18, 0, 0, 0, DateTimeKind.Unspecified), 0, 2, 0, 1 },
                    { 3, 1, "Table 4 Brunch", 0, null, new DateTime(2024, 10, 2, 10, 0, 0, 0, DateTimeKind.Unspecified), 0, 3, 1, 1 }
                });

            migrationBuilder.InsertData(
                table: "menu_item_assignment",
                columns: new[] { "item_id", "menu_id", "Adult_Limit", "Child_limit", "Is_Add_On", "LastOrdered", "Price", "Senior_limit", "Status", "Total_Limit", "Total_Units_Ordered", "Total_View_Seconds", "Total_Views" },
                values: new object[,]
                {
                    { 1, 2, 0, 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 12.99m, 0, 0, 0, 0, 0, 0 },
                    { 2, 2, 0, 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 8.99m, 0, 0, 0, 0, 0, 0 },
                    { 3, 2, 0, 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 9.99m, 0, 0, 0, 0, 0, 0 },
                    { 5, 2, 0, 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 15.99m, 0, 0, 0, 0, 0, 0 },
                    { 1, 3, 0, 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 13.99m, 0, 0, 0, 0, 0, 0 },
                    { 4, 3, 0, 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 24.99m, 0, 0, 0, 0, 0, 0 },
                    { 5, 3, 0, 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 16.99m, 0, 0, 0, 0, 0, 0 },
                    { 6, 3, 0, 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 18.99m, 0, 0, 0, 0, 0, 0 },
                    { 7, 3, 0, 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 7.99m, 0, 0, 0, 0, 0, 0 },
                    { 3, 4, 0, 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 11.99m, 0, 0, 0, 0, 0, 0 },
                    { 8, 4, 0, 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 6.99m, 0, 0, 0, 0, 0, 0 },
                    { 9, 4, 0, 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5.99m, 0, 0, 0, 0, 0, 0 },
                    { 10, 4, 0, 0, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3.99m, 0, 0, 0, 0, 0, 0 }
                });

            migrationBuilder.InsertData(
                table: "menu_item_tag",
                columns: new[] { "Menu_item_id", "Tag_id" },
                values: new object[,]
                {
                    { 1, 4 },
                    { 1, 5 },
                    { 3, 1 },
                    { 5, 5 },
                    { 6, 1 },
                    { 6, 2 },
                    { 10, 6 }
                });

            migrationBuilder.InsertData(
                table: "service_request",
                columns: new[] { "request_id", "Claimed_At", "Claimed_By", "Completed_At", "Created_At", "Notes", "Request_By", "Session_Id", "Status", "Table_Id", "User_id", "User_id1" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 10, 1, 12, 32, 0, 0, DateTimeKind.Unspecified), 2, new DateTime(2024, 10, 1, 12, 35, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 1, 12, 30, 0, 0, DateTimeKind.Unspecified), "Need extra napkins", 1, 1, 2, 1, null, null },
                    { 2, null, null, null, new DateTime(2024, 10, 1, 18, 45, 0, 0, DateTimeKind.Unspecified), "Check on food order", 1, 2, 0, 3, null, null },
                    { 3, new DateTime(2024, 10, 2, 11, 2, 0, 0, DateTimeKind.Unspecified), 3, new DateTime(2024, 10, 2, 11, 10, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 2, 11, 0, 0, 0, DateTimeKind.Unspecified), "Request the bill", 5, 3, 2, 4, null, null }
                });

            migrationBuilder.InsertData(
                table: "session_participant",
                columns: new[] { "Session_Id", "User_Id", "Joined_At", "Left_At", "Participant_Id" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 10, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), null, 1 },
                    { 1, 5, new DateTime(2024, 10, 1, 12, 5, 0, 0, DateTimeKind.Unspecified), null, 2 },
                    { 2, 1, new DateTime(2024, 10, 1, 18, 0, 0, 0, DateTimeKind.Unspecified), null, 3 },
                    { 3, 5, new DateTime(2024, 10, 2, 10, 0, 0, 0, DateTimeKind.Unspecified), null, 4 }
                });

            migrationBuilder.InsertData(
                table: "sessions",
                columns: new[] { "Session_Id", "Table_Id" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 2, 3 },
                    { 3, 4 }
                });

            migrationBuilder.InsertData(
                table: "session_order",
                columns: new[] { "Order_Id", "Bill_Id", "Completed_At", "Created_At", "Status", "User_Id", "session_id" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 10, 1, 12, 45, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 1, 12, 15, 0, 0, DateTimeKind.Unspecified), 2, 1, 1 },
                    { 2, 1, new DateTime(2024, 10, 1, 12, 50, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 1, 12, 20, 0, 0, DateTimeKind.Unspecified), 2, 5, 1 },
                    { 3, 2, null, new DateTime(2024, 10, 1, 18, 30, 0, 0, DateTimeKind.Unspecified), 1, 1, 2 },
                    { 4, 3, new DateTime(2024, 10, 2, 11, 15, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 10, 2, 10, 45, 0, 0, DateTimeKind.Unspecified), 2, 5, 3 }
                });

            migrationBuilder.InsertData(
                table: "order_item",
                columns: new[] { "Order_Item_Id", "Completed_At", "Item_Id", "Menu_Id", "Order_Item_Status", "Order_Key", "Price_At_Time", "Quantity" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 10, 1, 12, 45, 0, 0, DateTimeKind.Unspecified), 1, 2, 2, 1, 12.99m, 1 },
                    { 2, new DateTime(2024, 10, 1, 12, 45, 0, 0, DateTimeKind.Unspecified), 3, 2, 2, 1, 9.99m, 1 },
                    { 3, new DateTime(2024, 10, 1, 12, 50, 0, 0, DateTimeKind.Unspecified), 5, 2, 2, 2, 15.99m, 1 },
                    { 4, null, 4, 3, 1, 3, 24.99m, 1 },
                    { 5, new DateTime(2024, 10, 2, 11, 15, 0, 0, DateTimeKind.Unspecified), 8, 4, 2, 4, 6.99m, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_service_request_User_id",
                table: "service_request",
                column: "User_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_request_User_id1",
                table: "service_request",
                column: "User_id1");

            migrationBuilder.AddForeignKey(
                name: "FK_service_request_User_User_id",
                table: "service_request",
                column: "User_id",
                principalTable: "User",
                principalColumn: "User_id");

            migrationBuilder.AddForeignKey(
                name: "FK_service_request_User_User_id1",
                table: "service_request",
                column: "User_id1",
                principalTable: "User",
                principalColumn: "User_id");
        }
    }
}
