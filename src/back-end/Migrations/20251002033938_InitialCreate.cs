using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace back_end.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Category_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    image_url = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    total_views = table.Column<int>(type: "int", maxLength: 255, nullable: false),
                    total_view_seconds = table.Column<int>(type: "int", nullable: false),
                    last_viewed_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    adult_limit = table.Column<int>(type: "int", nullable: false),
                    child_limit = table.Column<int>(type: "int", nullable: false),
                    senior_limit = table.Column<int>(type: "int", nullable: false),
                    total_limit = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Category_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "locations",
                columns: table => new
                {
                    Location_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address_Primary = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address_Secondary = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    City = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Province = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Postal_Code = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Phone_Number = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Created_At = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_locations", x => x.Location_Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "menu",
                columns: table => new
                {
                    Menu_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Start_time = table.Column<TimeOnly>(type: "time(6)", nullable: false),
                    End_time = table.Column<TimeOnly>(type: "time(6)", nullable: false),
                    Is_active = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menu", x => x.Menu_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "table_entity",
                columns: table => new
                {
                    Table_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    table_number = table.Column<int>(type: "int", nullable: false),
                    QR_Code = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    seat_count = table.Column<int>(type: "int", nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_entity", x => x.Table_Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tag",
                columns: table => new
                {
                    tag_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tag_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tag_color = table.Column<string>(type: "varchar(7)", maxLength: 7, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tag", x => x.tag_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    User_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Normalized_email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password_hash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    First_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Last_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Role = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Last_Interaction_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Is_email_confirmed = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.User_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "menu_item",
                columns: table => new
                {
                    item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Category_id = table.Column<int>(type: "int", nullable: false),
                    image_url = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menu_item", x => x.item_id);
                    table.ForeignKey(
                        name: "FK_menu_item_Categories_Category_id",
                        column: x => x.Category_id,
                        principalTable: "Categories",
                        principalColumn: "Category_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "dining_sessions",
                columns: table => new
                {
                    Session_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Menu_Id = table.Column<int>(type: "int", nullable: false),
                    Started_At = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Ended_At = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    First_Order_At = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dining_sessions", x => x.Session_Id);
                    table.ForeignKey(
                        name: "FK_dining_sessions_menu_Menu_Id",
                        column: x => x.Menu_Id,
                        principalTable: "menu",
                        principalColumn: "Menu_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "menu_location",
                columns: table => new
                {
                    Menu_Id = table.Column<int>(type: "int", nullable: false),
                    Location_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menu_location", x => new { x.Menu_Id, x.Location_Id });
                    table.ForeignKey(
                        name: "FK_menu_location_locations_Location_Id",
                        column: x => x.Location_Id,
                        principalTable: "locations",
                        principalColumn: "Location_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_menu_location_menu_Menu_Id",
                        column: x => x.Menu_Id,
                        principalTable: "menu",
                        principalColumn: "Menu_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "menu_item_assignment",
                columns: table => new
                {
                    menu_id = table.Column<int>(type: "int", nullable: false),
                    item_id = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Total_Units_Ordered = table.Column<int>(type: "int", nullable: false),
                    LastOrdered = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Total_Views = table.Column<int>(type: "int", nullable: false),
                    Total_View_Seconds = table.Column<int>(type: "int", nullable: false),
                    Is_Add_On = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Adult_Limit = table.Column<int>(type: "int", nullable: false),
                    Child_limit = table.Column<int>(type: "int", nullable: false),
                    Senior_limit = table.Column<int>(type: "int", nullable: false),
                    Total_Limit = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menu_item_assignment", x => new { x.menu_id, x.item_id });
                    table.ForeignKey(
                        name: "FK_menu_item_assignment_menu_item_item_id",
                        column: x => x.item_id,
                        principalTable: "menu_item",
                        principalColumn: "item_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_menu_item_assignment_menu_menu_id",
                        column: x => x.menu_id,
                        principalTable: "menu",
                        principalColumn: "Menu_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "menu_item_tag",
                columns: table => new
                {
                    Menu_item_id = table.Column<int>(type: "int", nullable: false),
                    Tag_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menu_item_tag", x => new { x.Menu_item_id, x.Tag_id });
                    table.ForeignKey(
                        name: "FK_menu_item_tag_menu_item_Menu_item_id",
                        column: x => x.Menu_item_id,
                        principalTable: "menu_item",
                        principalColumn: "item_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_menu_item_tag_tag_Tag_id",
                        column: x => x.Tag_id,
                        principalTable: "tag",
                        principalColumn: "tag_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "bills",
                columns: table => new
                {
                    Bill_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Session_Id = table.Column<int>(type: "int", nullable: false),
                    Bill_Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Senior_Count = table.Column<int>(type: "int", nullable: false),
                    Adult_Count = table.Column<int>(type: "int", nullable: false),
                    Child_Count = table.Column<int>(type: "int", nullable: false),
                    Total_Count = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Closed_At = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bills", x => x.Bill_Id);
                    table.ForeignKey(
                        name: "FK_bills_dining_sessions_Session_Id",
                        column: x => x.Session_Id,
                        principalTable: "dining_sessions",
                        principalColumn: "Session_Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "service_request",
                columns: table => new
                {
                    request_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Session_Id = table.Column<int>(type: "int", nullable: false),
                    Table_Id = table.Column<int>(type: "int", nullable: false),
                    Request_By = table.Column<int>(type: "int", nullable: false),
                    Claimed_By = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Claimed_At = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Completed_At = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    User_id = table.Column<int>(type: "int", nullable: true),
                    User_id1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_request", x => x.request_id);
                    table.ForeignKey(
                        name: "FK_service_request_User_Claimed_By",
                        column: x => x.Claimed_By,
                        principalTable: "User",
                        principalColumn: "User_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_service_request_User_Request_By",
                        column: x => x.Request_By,
                        principalTable: "User",
                        principalColumn: "User_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_service_request_User_User_id",
                        column: x => x.User_id,
                        principalTable: "User",
                        principalColumn: "User_id");
                    table.ForeignKey(
                        name: "FK_service_request_User_User_id1",
                        column: x => x.User_id1,
                        principalTable: "User",
                        principalColumn: "User_id");
                    table.ForeignKey(
                        name: "FK_service_request_dining_sessions_Session_Id",
                        column: x => x.Session_Id,
                        principalTable: "dining_sessions",
                        principalColumn: "Session_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_service_request_table_entity_Table_Id",
                        column: x => x.Table_Id,
                        principalTable: "table_entity",
                        principalColumn: "Table_Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "session_participant",
                columns: table => new
                {
                    Session_Id = table.Column<int>(type: "int", nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    Participant_Id = table.Column<int>(type: "int", nullable: false),
                    Joined_At = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Left_At = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_session_participant", x => new { x.Session_Id, x.User_Id });
                    table.ForeignKey(
                        name: "FK_session_participant_User_User_Id",
                        column: x => x.User_Id,
                        principalTable: "User",
                        principalColumn: "User_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_session_participant_dining_sessions_Session_Id",
                        column: x => x.Session_Id,
                        principalTable: "dining_sessions",
                        principalColumn: "Session_Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sessions",
                columns: table => new
                {
                    Session_Id = table.Column<int>(type: "int", nullable: false),
                    Table_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sessions", x => new { x.Session_Id, x.Table_Id });
                    table.ForeignKey(
                        name: "FK_sessions_dining_sessions_Session_Id",
                        column: x => x.Session_Id,
                        principalTable: "dining_sessions",
                        principalColumn: "Session_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sessions_table_entity_Table_Id",
                        column: x => x.Table_Id,
                        principalTable: "table_entity",
                        principalColumn: "Table_Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "session_order",
                columns: table => new
                {
                    Order_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    session_id = table.Column<int>(type: "int", nullable: false),
                    Bill_Id = table.Column<int>(type: "int", nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Created_At = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Completed_At = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_session_order", x => x.Order_Id);
                    table.ForeignKey(
                        name: "FK_session_order_User_User_Id",
                        column: x => x.User_Id,
                        principalTable: "User",
                        principalColumn: "User_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_session_order_bills_Bill_Id",
                        column: x => x.Bill_Id,
                        principalTable: "bills",
                        principalColumn: "Bill_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_session_order_dining_sessions_session_id",
                        column: x => x.session_id,
                        principalTable: "dining_sessions",
                        principalColumn: "Session_Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "order_item",
                columns: table => new
                {
                    Order_Item_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Order_Key = table.Column<int>(type: "int", nullable: false),
                    Menu_Id = table.Column<int>(type: "int", nullable: false),
                    Item_Id = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price_At_Time = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Order_Item_Status = table.Column<int>(type: "int", nullable: false),
                    Completed_At = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_item", x => x.Order_Item_Id);
                    table.ForeignKey(
                        name: "FK_order_item_menu_Menu_Id",
                        column: x => x.Menu_Id,
                        principalTable: "menu",
                        principalColumn: "Menu_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_item_menu_item_Item_Id",
                        column: x => x.Item_Id,
                        principalTable: "menu_item",
                        principalColumn: "item_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_item_session_order_Order_Key",
                        column: x => x.Order_Key,
                        principalTable: "session_order",
                        principalColumn: "Order_Id",
                        onDelete: ReferentialAction.Cascade);
                })
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
                    { 1, new DateTime(2025, 10, 1, 21, 39, 37, 656, DateTimeKind.Local).AddTicks(9514), "john.doe@example.com", "John", false, null, "Doe", "JOHN.DOE@EXAMPLE.COM", "hashed_password_1", 2, 0 },
                    { 2, new DateTime(2025, 10, 1, 21, 39, 37, 657, DateTimeKind.Local).AddTicks(295), "jane.smith@example.com", "Jane", false, null, "Smith", "JANE.SMITH@EXAMPLE.COM", "hashed_password_2", 1, 0 },
                    { 3, new DateTime(2025, 10, 1, 21, 39, 37, 657, DateTimeKind.Local).AddTicks(302), "bob.johnson@example.com", "Bob", false, null, "Johnson", "BOB.JOHNSON@EXAMPLE.COM", "hashed_password_3", 1, 0 },
                    { 4, new DateTime(2025, 10, 1, 21, 39, 37, 657, DateTimeKind.Local).AddTicks(304), "alice.wilson@example.com", "Alice", false, null, "Wilson", "ALICE.WILSON@EXAMPLE.COM", "hashed_password_4", 0, 0 },
                    { 5, new DateTime(2025, 10, 1, 21, 39, 37, 657, DateTimeKind.Local).AddTicks(307), "mike.brown@example.com", "Mike", false, null, "Brown", "MIKE.BROWN@EXAMPLE.COM", "hashed_password_5", 2, 0 }
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
                name: "IX_bills_Session_Id",
                table: "bills",
                column: "Session_Id");

            migrationBuilder.CreateIndex(
                name: "IX_dining_sessions_Menu_Id",
                table: "dining_sessions",
                column: "Menu_Id");

            migrationBuilder.CreateIndex(
                name: "IX_menu_item_Category_id",
                table: "menu_item",
                column: "Category_id");

            migrationBuilder.CreateIndex(
                name: "IX_menu_item_assignment_item_id",
                table: "menu_item_assignment",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "IX_menu_item_tag_Tag_id",
                table: "menu_item_tag",
                column: "Tag_id");

            migrationBuilder.CreateIndex(
                name: "IX_menu_location_Location_Id",
                table: "menu_location",
                column: "Location_Id");

            migrationBuilder.CreateIndex(
                name: "IX_order_item_Item_Id",
                table: "order_item",
                column: "Item_Id");

            migrationBuilder.CreateIndex(
                name: "IX_order_item_Menu_Id",
                table: "order_item",
                column: "Menu_Id");

            migrationBuilder.CreateIndex(
                name: "IX_order_item_Order_Key",
                table: "order_item",
                column: "Order_Key");

            migrationBuilder.CreateIndex(
                name: "IX_service_request_Claimed_By",
                table: "service_request",
                column: "Claimed_By");

            migrationBuilder.CreateIndex(
                name: "IX_service_request_Request_By",
                table: "service_request",
                column: "Request_By");

            migrationBuilder.CreateIndex(
                name: "IX_service_request_Session_Id",
                table: "service_request",
                column: "Session_Id");

            migrationBuilder.CreateIndex(
                name: "IX_service_request_Table_Id",
                table: "service_request",
                column: "Table_Id");

            migrationBuilder.CreateIndex(
                name: "IX_service_request_User_id",
                table: "service_request",
                column: "User_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_request_User_id1",
                table: "service_request",
                column: "User_id1");

            migrationBuilder.CreateIndex(
                name: "IX_session_order_Bill_Id",
                table: "session_order",
                column: "Bill_Id");

            migrationBuilder.CreateIndex(
                name: "IX_session_order_session_id",
                table: "session_order",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "IX_session_order_User_Id",
                table: "session_order",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_session_participant_User_Id",
                table: "session_participant",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_sessions_Table_Id",
                table: "sessions",
                column: "Table_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "menu_item_assignment");

            migrationBuilder.DropTable(
                name: "menu_item_tag");

            migrationBuilder.DropTable(
                name: "menu_location");

            migrationBuilder.DropTable(
                name: "order_item");

            migrationBuilder.DropTable(
                name: "service_request");

            migrationBuilder.DropTable(
                name: "session_participant");

            migrationBuilder.DropTable(
                name: "sessions");

            migrationBuilder.DropTable(
                name: "tag");

            migrationBuilder.DropTable(
                name: "locations");

            migrationBuilder.DropTable(
                name: "menu_item");

            migrationBuilder.DropTable(
                name: "session_order");

            migrationBuilder.DropTable(
                name: "table_entity");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "bills");

            migrationBuilder.DropTable(
                name: "dining_sessions");

            migrationBuilder.DropTable(
                name: "menu");
        }
    }
}
