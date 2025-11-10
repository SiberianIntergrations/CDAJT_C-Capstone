using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace back_end.Migrations
{
    /// <inheritdoc />
    public partial class OauthRemodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_locations_users_User_id",
                table: "locations");

            migrationBuilder.DropForeignKey(
                name: "FK_service_request_users_Claimed_By",
                table: "service_request");

            migrationBuilder.DropForeignKey(
                name: "FK_service_request_users_Request_By",
                table: "service_request");

            migrationBuilder.DropForeignKey(
                name: "FK_session_order_users_User_Id",
                table: "session_order");

            migrationBuilder.DropForeignKey(
                name: "FK_session_participant_users_User_Id",
                table: "session_participant");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropIndex(
                name: "IX_session_participant_User_Id",
                table: "session_participant");

            migrationBuilder.DropIndex(
                name: "IX_session_order_User_Id",
                table: "session_order");

            migrationBuilder.DropIndex(
                name: "IX_service_request_Claimed_By",
                table: "service_request");

            migrationBuilder.DropIndex(
                name: "IX_service_request_Request_By",
                table: "service_request");

            migrationBuilder.DropIndex(
                name: "IX_locations_User_id",
                table: "locations");

            migrationBuilder.DropColumn(
                name: "User_Id",
                table: "session_participant");

            migrationBuilder.DropColumn(
                name: "User_Id",
                table: "session_order");

            migrationBuilder.DropColumn(
                name: "Claimed_By",
                table: "service_request");

            migrationBuilder.DropColumn(
                name: "Request_By",
                table: "service_request");

            migrationBuilder.DropColumn(
                name: "User_id",
                table: "locations");

            migrationBuilder.AddColumn<string>(
                name: "User_Name",
                table: "session_participant",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "User_Oid",
                table: "session_participant",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "User_Name",
                table: "session_order",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "User_Oid",
                table: "session_order",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Claimed_By_Name",
                table: "service_request",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Claimed_By_Oid",
                table: "service_request",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Request_By_Name",
                table: "service_request",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Request_By_Oid",
                table: "service_request",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "User_Name",
                table: "session_participant");

            migrationBuilder.DropColumn(
                name: "User_Oid",
                table: "session_participant");

            migrationBuilder.DropColumn(
                name: "User_Name",
                table: "session_order");

            migrationBuilder.DropColumn(
                name: "User_Oid",
                table: "session_order");

            migrationBuilder.DropColumn(
                name: "Claimed_By_Name",
                table: "service_request");

            migrationBuilder.DropColumn(
                name: "Claimed_By_Oid",
                table: "service_request");

            migrationBuilder.DropColumn(
                name: "Request_By_Name",
                table: "service_request");

            migrationBuilder.DropColumn(
                name: "Request_By_Oid",
                table: "service_request");

            migrationBuilder.AddColumn<int>(
                name: "User_Id",
                table: "session_participant",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "User_Id",
                table: "session_order",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Claimed_By",
                table: "service_request",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Request_By",
                table: "service_request",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "User_id",
                table: "locations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    User_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Location_id = table.Column<int>(type: "int", nullable: true),
                    Created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    First_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Is_email_confirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Last_Interaction_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Last_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Normalized_email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password_hash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Role = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.User_id);
                    table.ForeignKey(
                        name: "FK_users_locations_Location_id",
                        column: x => x.Location_id,
                        principalTable: "locations",
                        principalColumn: "Location_Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_session_participant_User_Id",
                table: "session_participant",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_session_order_User_Id",
                table: "session_order",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_service_request_Claimed_By",
                table: "service_request",
                column: "Claimed_By");

            migrationBuilder.CreateIndex(
                name: "IX_service_request_Request_By",
                table: "service_request",
                column: "Request_By");

            migrationBuilder.CreateIndex(
                name: "IX_locations_User_id",
                table: "locations",
                column: "User_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_Location_id",
                table: "users",
                column: "Location_id");

            migrationBuilder.AddForeignKey(
                name: "FK_locations_users_User_id",
                table: "locations",
                column: "User_id",
                principalTable: "users",
                principalColumn: "User_id");

            migrationBuilder.AddForeignKey(
                name: "FK_service_request_users_Claimed_By",
                table: "service_request",
                column: "Claimed_By",
                principalTable: "users",
                principalColumn: "User_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_service_request_users_Request_By",
                table: "service_request",
                column: "Request_By",
                principalTable: "users",
                principalColumn: "User_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_session_order_users_User_Id",
                table: "session_order",
                column: "User_Id",
                principalTable: "users",
                principalColumn: "User_id");

            migrationBuilder.AddForeignKey(
                name: "FK_session_participant_users_User_Id",
                table: "session_participant",
                column: "User_Id",
                principalTable: "users",
                principalColumn: "User_id");
        }
    }
}
