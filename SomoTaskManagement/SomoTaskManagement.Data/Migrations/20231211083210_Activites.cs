using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SomoTaskManagement.Data.Migrations
{
    public partial class Activites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SubTask",
                table: "SubTask");

            migrationBuilder.RenameTable(
                name: "SubTask",
                newName: "Activites");

            migrationBuilder.RenameColumn(
                name: "SubtaskId",
                table: "Activites",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_SubTask_TaskId",
                table: "Activites",
                newName: "IX_Activites_TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_SubTask_EmployeeId",
                table: "Activites",
                newName: "IX_Activites_EmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Activites",
                table: "Activites",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Activites",
                table: "Activites");

            migrationBuilder.RenameTable(
                name: "Activites",
                newName: "SubTask");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "SubTask",
                newName: "SubtaskId");

            migrationBuilder.RenameIndex(
                name: "IX_Activites_TaskId",
                table: "SubTask",
                newName: "IX_SubTask_TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_Activites_EmployeeId",
                table: "SubTask",
                newName: "IX_SubTask_EmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubTask",
                table: "SubTask",
                column: "SubtaskId");
        }
    }
}
