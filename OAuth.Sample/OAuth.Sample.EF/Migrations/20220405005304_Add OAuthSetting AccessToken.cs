using Microsoft.EntityFrameworkCore.Migrations;

namespace OAuth.Sample.EF.Migrations
{
    public partial class AddOAuthSettingAccessToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccessToken",
                table: "UserOAuthSetting",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessToken",
                table: "UserOAuthSetting");
        }
    }
}
