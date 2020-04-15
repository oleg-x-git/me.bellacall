using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace me.bellacall.Core.Migrations
{
    public partial class UserLog_Operation_Id : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Operation_Id",
                table: "AspNetUserLogs",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Operation_Id",
                table: "AspNetUserLogs");
        }
    }
}
