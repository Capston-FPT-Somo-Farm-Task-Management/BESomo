using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SomoTaskManagement.Data.Migrations
{
    public partial class IsExpired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsExpired",
                table: "FarmTask",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExpired",
                table: "FarmTask");
        }
    }
}
