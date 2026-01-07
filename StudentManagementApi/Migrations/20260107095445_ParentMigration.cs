using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StudentManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class ParentMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "parent_id",
                table: "students",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "parents",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_parents", x => x.id);
                    table.ForeignKey(
                        name: "fk_parents_users_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_students_parent_id",
                table: "students",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_parents_user_id",
                table: "parents",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_students_parents_parent_id",
                table: "students",
                column: "parent_id",
                principalTable: "parents",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_students_parents_parent_id",
                table: "students");

            migrationBuilder.DropTable(
                name: "parents");

            migrationBuilder.DropIndex(
                name: "ix_students_parent_id",
                table: "students");

            migrationBuilder.DropColumn(
                name: "parent_id",
                table: "students");
        }
    }
}
