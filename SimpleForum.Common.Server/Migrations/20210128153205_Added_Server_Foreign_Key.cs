using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleForum.Common.Server.Migrations
{
    public partial class Added_Server_Foreign_Key : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServerID",
                table: "Users",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ServerID",
                table: "Users",
                column: "ServerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_IncomingServerTokens_ServerID",
                table: "Users",
                column: "ServerID",
                principalTable: "IncomingServerTokens",
                principalColumn: "IncomingServerTokenID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_IncomingServerTokens_ServerID",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ServerID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ServerID",
                table: "Users");
        }
    }
}
