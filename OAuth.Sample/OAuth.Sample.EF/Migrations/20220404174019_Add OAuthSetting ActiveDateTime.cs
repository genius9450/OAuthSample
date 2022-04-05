using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OAuth.Sample.EF.Migrations
{
    public partial class AddOAuthSettingActiveDateTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActiveDateTime",
                table: "UserOAuthSetting",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveDateTime",
                table: "UserOAuthSetting");
        }
    }
}
