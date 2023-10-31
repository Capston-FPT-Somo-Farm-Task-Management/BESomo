using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SomoTaskManagement.Data.Migrations
{
    public partial class UpdateInformationFarm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Employee_Task",
                table: "Employee_Task");

            migrationBuilder.RenameTable(
                name: "Employee_Task",
                newName: "SubTask");

            migrationBuilder.RenameIndex(
                name: "IX_Employee_Task_TaskId",
                table: "SubTask",
                newName: "IX_SubTask_TaskId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Farm",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UrlImage",
                table: "Farm",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubTask",
                table: "SubTask",
                columns: new[] { "EmployeeId", "TaskId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SubTask",
                table: "SubTask");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Farm");

            migrationBuilder.DropColumn(
                name: "UrlImage",
                table: "Farm");

            migrationBuilder.RenameTable(
                name: "SubTask",
                newName: "Employee_Task");

            migrationBuilder.RenameIndex(
                name: "IX_SubTask_TaskId",
                table: "Employee_Task",
                newName: "IX_Employee_Task_TaskId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employee_Task",
                table: "Employee_Task",
                columns: new[] { "EmployeeId", "TaskId" });
        }
    }
}
