using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleForum.Internal.Migrations
{
    public partial class IPost_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "UserComments",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Comments",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "UserComments");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Comments");
        }
    }
}
