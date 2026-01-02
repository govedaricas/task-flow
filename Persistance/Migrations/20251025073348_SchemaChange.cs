using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class SchemaChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "TaskStatus",
                schema: "Lookups",
                newName: "TaskStatus",
                newSchema: "BasicCatalog");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Lookups");

            migrationBuilder.RenameTable(
                name: "TaskStatus",
                schema: "BasicCatalog",
                newName: "TaskStatus",
                newSchema: "Lookups");
        }
    }
}
