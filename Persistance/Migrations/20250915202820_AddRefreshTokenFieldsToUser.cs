using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenFieldsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                schema: "Administration",
                table: "User");

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                schema: "Administration",
                table: "User",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RefreshTokenHash",
                schema: "Administration",
                table: "User",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                schema: "Administration",
                table: "User");

            migrationBuilder.DropColumn(
                name: "RefreshTokenHash",
                schema: "Administration",
                table: "User");

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                schema: "Administration",
                table: "User",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
