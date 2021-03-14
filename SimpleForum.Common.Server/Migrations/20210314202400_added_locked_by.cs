using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleForum.Common.Server.Migrations
{
    public partial class added_locked_by : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LockedBy",
                table: "Threads",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LockedBy",
                table: "Threads");
        }
    }
}
