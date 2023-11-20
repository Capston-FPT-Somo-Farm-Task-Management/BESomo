using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SomoTaskManagement.Data.Migrations
{
    public partial class AddIsplant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPlant",
                table: "FarmTask",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPlant",
                table: "FarmTask");
        }
    }
}
