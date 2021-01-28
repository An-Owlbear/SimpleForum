using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleForum.Common.Server.Migrations
{
    public partial class Renamed_Auth_Tokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthTokens");

            migrationBuilder.CreateTable(
                name: "RemoteAuthTokens",
                columns: table => new
                {
                    Token = table.Column<string>(nullable: false),
                    ValidUntil = table.Column<DateTime>(nullable: false),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemoteAuthTokens", x => x.Token);
                    table.ForeignKey(
                        name: "FK_RemoteAuthTokens_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RemoteAuthTokens_UserID",
                table: "RemoteAuthTokens",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RemoteAuthTokens");

            migrationBuilder.CreateTable(
                name: "AuthTokens",
                columns: table => new
                {
                    Token = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthTokens", x => x.Token);
                    table.ForeignKey(
                        name: "FK_AuthTokens_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthTokens_UserID",
                table: "AuthTokens",
                column: "UserID");
        }
    }
}
