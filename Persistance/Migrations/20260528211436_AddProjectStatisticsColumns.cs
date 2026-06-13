using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectStatisticsColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DoneCount",
                schema: "ProjectManagement",
                table: "ProjectStatistics",
                newName: "OnHoldCount");

            migrationBuilder.AddColumn<int>(
                name: "CancelledCount",
                schema: "ProjectManagement",
                table: "ProjectStatistics",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompletedCount",
                schema: "ProjectManagement",
                table: "ProjectStatistics",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelledCount",
                schema: "ProjectManagement",
                table: "ProjectStatistics");

            migrationBuilder.DropColumn(
                name: "CompletedCount",
                schema: "ProjectManagement",
                table: "ProjectStatistics");

            migrationBuilder.RenameColumn(
                name: "OnHoldCount",
                schema: "ProjectManagement",
                table: "ProjectStatistics",
                newName: "DoneCount");
        }
    }
}
