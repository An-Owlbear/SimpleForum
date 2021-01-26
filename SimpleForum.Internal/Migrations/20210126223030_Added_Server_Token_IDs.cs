using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleForum.Internal.Migrations
{
    public partial class Added_Server_Token_IDs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OutgoingServerTokens",
                table: "OutgoingServerTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IncomingServerTokens",
                table: "IncomingServerTokens");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "OutgoingServerTokens",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255) CHARACTER SET utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "OutgoingServerTokenID",
                table: "OutgoingServerTokens",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "IncomingServerTokens",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255) CHARACTER SET utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "IncomingServerTokenID",
                table: "IncomingServerTokens",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutgoingServerTokens",
                table: "OutgoingServerTokens",
                column: "OutgoingServerTokenID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IncomingServerTokens",
                table: "IncomingServerTokens",
                column: "IncomingServerTokenID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OutgoingServerTokens",
                table: "OutgoingServerTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IncomingServerTokens",
                table: "IncomingServerTokens");

            migrationBuilder.DropColumn(
                name: "OutgoingServerTokenID",
                table: "OutgoingServerTokens");

            migrationBuilder.DropColumn(
                name: "IncomingServerTokenID",
                table: "IncomingServerTokens");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "OutgoingServerTokens",
                type: "varchar(255) CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "IncomingServerTokens",
                type: "varchar(255) CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutgoingServerTokens",
                table: "OutgoingServerTokens",
                column: "Address");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IncomingServerTokens",
                table: "IncomingServerTokens",
                column: "Address");
        }
    }
}
