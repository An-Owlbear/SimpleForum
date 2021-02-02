using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleForum.Common.Server.Migrations
{
    public partial class Added_Server_Foreign_Key : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IncomingServerTokenID",
                table: "IncomingServerTokens",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);;

            migrationBuilder.DropPrimaryKey(
                name: "PK_IncomingServerTokens",
                table: "IncomingServerTokens");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IncomingServerTokens",
                table: "IncomingServerTokens",
                column: "IncomingServerTokenID");
            
            migrationBuilder.AddColumn<int>(
                name: "OutgoingServerTokenID",
                table: "OutgoingServerTokens",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);;

            migrationBuilder.DropPrimaryKey(
                name: "PK_OutgoingServerTokens",
                table: "OutgoingServerTokens");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutgoingServerTokens",
                table: "OutgoingServerTokens",
                column: "OutgoingServerTokenID");
            
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
            
            migrationBuilder.DropPrimaryKey(
                name: "PK_OutgoingServerTokens",
                table: "OutgoingServerTokens");
            
            migrationBuilder.AddPrimaryKey(
                name: "PK_OutgoingServerTokens",
                table: "OutgoingServerTokens",
                column: "address");

            migrationBuilder.DropColumn(
                name: "OutgoingServerTokenID",
                table: "OutgoingServerTokens");
            
            migrationBuilder.DropPrimaryKey(
                name: "PK_IncomingServerTokens",
                table: "IncomingServerTokens");
            
            migrationBuilder.AddPrimaryKey(
                name: "PK_IncomingServerTokens",
                table: "IncomingServerTokens",
                column: "Address");

            migrationBuilder.DropColumn(
                name: "IncomingServerTokenID",
                table: "IncomingServerTokens");
        }
    }
}
