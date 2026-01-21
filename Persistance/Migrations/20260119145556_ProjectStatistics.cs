using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class ProjectStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "ProjectManagement",
                table: "Project",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "ProjectStatistics",
                schema: "ProjectManagement",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    TodoCount = table.Column<int>(type: "integer", nullable: false),
                    InProgressCount = table.Column<int>(type: "integer", nullable: false),
                    DoneCount = table.Column<int>(type: "integer", nullable: false),
                    LastActivityAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsOverloaded = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectStatistics", x => x.ProjectId);
                    table.ForeignKey(
                        name: "FK_ProjectStatistics_Project",
                        column: x => x.ProjectId,
                        principalSchema: "ProjectManagement",
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectStatistics",
                schema: "ProjectManagement");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "ProjectManagement",
                table: "Project",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);
        }
    }
}
