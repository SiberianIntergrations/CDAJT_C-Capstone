using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace back_end.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSessionConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_DiningSession_TableAssignment",
                table: "dining_sessions");

            migrationBuilder.AddCheckConstraint(
                name: "CK_DiningSession_TableAssignment",
                table: "dining_sessions",
                sql: "(Table_Id IS NOT NULL AND TableGroup_Id IS NULL) OR (Table_Id IS NULL AND TableGroup_Id IS NOT NULL) OR (Table_Id IS NULL AND TableGroup_Id IS NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_DiningSession_TableAssignment",
                table: "dining_sessions");

            migrationBuilder.AddCheckConstraint(
                name: "CK_DiningSession_TableAssignment",
                table: "dining_sessions",
                sql: "(Table_Id IS NOT NULL AND TableGroup_Id IS NULL) OR (Table_Id IS NULL AND TableGroup_Id IS NOT NULL)");
        }
    }
}
