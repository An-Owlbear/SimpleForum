using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleForum.Internal.Migrations
{
    public partial class Added_pinned_and_locked : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Locked",
                table: "Threads",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Pinned",
                table: "Threads",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Locked",
                table: "Threads");

            migrationBuilder.DropColumn(
                name: "Pinned",
                table: "Threads");
        }
    }
}
