using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class EntitiesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ProjectManagement");

            migrationBuilder.EnsureSchema(
                name: "BasicCatalog");

            migrationBuilder.EnsureSchema(
                name: "Lookups");

            migrationBuilder.CreateTable(
                name: "Project",
                schema: "ProjectManagement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Project_CreatedBy_User",
                        column: x => x.CreatedById,
                        principalSchema: "Administration",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskPriority",
                schema: "BasicCatalog",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskPriority", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskStatus",
                schema: "Lookups",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectMember",
                schema: "ProjectManagement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectMember_Project",
                        column: x => x.ProjectId,
                        principalSchema: "ProjectManagement",
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectMember_User",
                        column: x => x.UserId,
                        principalSchema: "Administration",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Task",
                schema: "ProjectManagement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    TaskStatusId = table.Column<byte>(type: "smallint", nullable: false),
                    TaskPriorityId = table.Column<byte>(type: "smallint", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: false),
                    AssignedUserId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Task_AssignedUser",
                        column: x => x.AssignedUserId,
                        principalSchema: "Administration",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Task_CreatedByUser",
                        column: x => x.CreatedByUserId,
                        principalSchema: "Administration",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Task_Project",
                        column: x => x.ProjectId,
                        principalSchema: "ProjectManagement",
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Task_TaskPriority_TaskPriorityId",
                        column: x => x.TaskPriorityId,
                        principalSchema: "BasicCatalog",
                        principalTable: "TaskPriority",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Task_TaskStatus",
                        column: x => x.TaskStatusId,
                        principalSchema: "Lookups",
                        principalTable: "TaskStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                schema: "ProjectManagement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Text = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TaskId = table.Column<int>(type: "integer", nullable: false),
                    AuthorId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_Author_User",
                        column: x => x.AuthorId,
                        principalSchema: "Administration",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comment_Task",
                        column: x => x.TaskId,
                        principalSchema: "ProjectManagement",
                        principalTable: "Task",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_AuthorId",
                schema: "ProjectManagement",
                table: "Comment",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_TaskId",
                schema: "ProjectManagement",
                table: "Comment",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_CreatedById",
                schema: "ProjectManagement",
                table: "Project",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMember_UserId",
                schema: "ProjectManagement",
                table: "ProjectMember",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ_ProjectMember_ProjectId_UserId",
                schema: "ProjectManagement",
                table: "ProjectMember",
                columns: new[] { "ProjectId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Task_AssignedUserId",
                schema: "ProjectManagement",
                table: "Task",
                column: "AssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_CreatedByUserId",
                schema: "ProjectManagement",
                table: "Task",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_ProjectId",
                schema: "ProjectManagement",
                table: "Task",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_TaskPriorityId",
                schema: "ProjectManagement",
                table: "Task",
                column: "TaskPriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_TaskStatusId",
                schema: "ProjectManagement",
                table: "Task",
                column: "TaskStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comment",
                schema: "ProjectManagement");

            migrationBuilder.DropTable(
                name: "ProjectMember",
                schema: "ProjectManagement");

            migrationBuilder.DropTable(
                name: "Task",
                schema: "ProjectManagement");

            migrationBuilder.DropTable(
                name: "Project",
                schema: "ProjectManagement");

            migrationBuilder.DropTable(
                name: "TaskPriority",
                schema: "BasicCatalog");

            migrationBuilder.DropTable(
                name: "TaskStatus",
                schema: "Lookups");
        }
    }
}
