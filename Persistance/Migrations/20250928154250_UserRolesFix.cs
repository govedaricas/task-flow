using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class UserRolesFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_User_Role",
                schema: "Administration",
                table: "UserRole");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRole",
                schema: "Administration",
                table: "UserRole",
                columns: new[] { "UserId", "RoleId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRole",
                schema: "Administration",
                table: "UserRole");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User_Role",
                schema: "Administration",
                table: "UserRole",
                columns: new[] { "UserId", "RoleId" });
        }
    }
}
