using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleForum.Internal.Migrations
{
    public partial class added_server_tokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IncomingServerTokens",
                columns: table => new
                {
                    Address = table.Column<string>(nullable: false),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomingServerTokens", x => x.Address);
                });

            migrationBuilder.CreateTable(
                name: "OutgoingServerTokens",
                columns: table => new
                {
                    Address = table.Column<string>(nullable: false),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutgoingServerTokens", x => x.Address);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncomingServerTokens");

            migrationBuilder.DropTable(
                name: "OutgoingServerTokens");
        }
    }
}
