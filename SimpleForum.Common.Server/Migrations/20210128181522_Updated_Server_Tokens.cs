using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleForum.Common.Server.Migrations
{
    public partial class Updated_Server_Tokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiAddress",
                table: "OutgoingServerTokens",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CrossConnectionAddress",
                table: "OutgoingServerTokens",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApiAddress",
                table: "IncomingServerTokens",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CrossConnectionAddress",
                table: "IncomingServerTokens",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiAddress",
                table: "OutgoingServerTokens");

            migrationBuilder.DropColumn(
                name: "CrossConnectionAddress",
                table: "OutgoingServerTokens");

            migrationBuilder.DropColumn(
                name: "ApiAddress",
                table: "IncomingServerTokens");

            migrationBuilder.DropColumn(
                name: "CrossConnectionAddress",
                table: "IncomingServerTokens");
        }
    }
}
