using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleForum.Common.Server.Migrations
{
    public partial class Added_Indexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserComments_DatePosted",
                table: "UserComments",
                column: "DatePosted");

            migrationBuilder.CreateIndex(
                name: "IX_Threads_DatePosted",
                table: "Threads",
                column: "DatePosted");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_DatePosted",
                table: "Comments",
                column: "DatePosted");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserComments_DatePosted",
                table: "UserComments");

            migrationBuilder.DropIndex(
                name: "IX_Threads_DatePosted",
                table: "Threads");

            migrationBuilder.DropIndex(
                name: "IX_Comments_DatePosted",
                table: "Comments");
        }
    }
}
