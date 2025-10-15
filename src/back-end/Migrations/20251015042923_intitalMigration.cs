using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace back_end.Migrations
{
    /// <inheritdoc />
    public partial class intitalMigration : Migration
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
                    LastViewedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Total_Views = table.Column<int>(type: "int", nullable: false),
                    Total_View_Seconds = table.Column<int>(type: "int", nullable: false),
                    Is_Add_On = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Adult_Limit = table.Column<int>(type: "int", nullable: false),
                    Child_limit = table.Column<int>(type: "int", nullable: false),
                    Senior_limit = table.Column<int>(type: "int", nullable: false),
                    Tot_Limit = table.Column<int>(type: "int", nullable: false)
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
                    Closed_At = table.Column<DateTime>(type: "datetime(6)", nullable: true)
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
                name: "table_entity",
                columns: table => new
                {
                    Table_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    table_number = table.Column<int>(type: "int", nullable: false),
                    QR_Code = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    seat_count = table.Column<int>(type: "int", nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DiningSessionSession_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_entity", x => x.Table_Id);
                    table.ForeignKey(
                        name: "FK_table_entity_dining_sessions_DiningSessionSession_Id",
                        column: x => x.DiningSessionSession_Id,
                        principalTable: "dining_sessions",
                        principalColumn: "Session_Id");
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
                    Completed_At = table.Column<DateTime>(type: "datetime(6)", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_table_entity_DiningSessionSession_Id",
                table: "table_entity",
                column: "DiningSessionSession_Id");
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
