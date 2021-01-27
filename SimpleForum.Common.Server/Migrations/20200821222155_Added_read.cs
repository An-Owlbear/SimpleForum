﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleForum.Common.Server.Migrations
{
    public partial class Added_read : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Read",
                table: "Notifications",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Read",
                table: "Notifications");
        }
    }
}
