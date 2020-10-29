using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleForum.Internal.Migrations
{
    public partial class Updated_Auth_Token : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthTokens",
                table: "AuthTokens");

            migrationBuilder.DropColumn(
                name: "AuthTokenID",
                table: "AuthTokens");

            migrationBuilder.DropColumn(
                name: "AccessToken",
                table: "AuthTokens");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "AuthTokens");

            migrationBuilder.DropColumn(
                name: "Time",
                table: "AuthTokens");

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "AuthTokens",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidUntil",
                table: "AuthTokens",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthTokens",
                table: "AuthTokens",
                column: "Token");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthTokens",
                table: "AuthTokens");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "AuthTokens");

            migrationBuilder.DropColumn(
                name: "ValidUntil",
                table: "AuthTokens");

            migrationBuilder.AddColumn<int>(
                name: "AuthTokenID",
                table: "AuthTokens",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<string>(
                name: "AccessToken",
                table: "AuthTokens",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "AuthTokens",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Time",
                table: "AuthTokens",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthTokens",
                table: "AuthTokens",
                column: "AuthTokenID");
        }
    }
}
