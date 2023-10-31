using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SomoTaskManagement.Data.Migrations
{
    public partial class BigUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Other_Task",
                table: "FarmTask");

            migrationBuilder.DropTable(
                name: "SubTask");

            migrationBuilder.DropColumn(
                name: "Iterations",
                table: "FarmTask");

            migrationBuilder.DropColumn(
                name: "Repeat",
                table: "FarmTask");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Employee_Task");

            migrationBuilder.AddColumn<bool>(
                name: "IsRepeat",
                table: "FarmTask",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ActualEffort",
                table: "Employee_Task",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Employee_Task",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FarmTask_Other_OtherId",
                table: "FarmTask",
                column: "OtherId",
                principalTable: "Other",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FarmTask_Other_OtherId",
                table: "FarmTask");

            migrationBuilder.DropColumn(
                name: "IsRepeat",
                table: "FarmTask");

            migrationBuilder.DropColumn(
                name: "ActualEffort",
                table: "Employee_Task");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Employee_Task");

            migrationBuilder.AddColumn<int>(
                name: "Iterations",
                table: "FarmTask",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Repeat",
                table: "FarmTask",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Employee_Task",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "SubTask",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    FarmTaskId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubTask", x => x.Id);
                    table.ForeignKey(
                        name: "Fk_Employee_Subtask",
                        column: x => x.EmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Task_Subtask",
                        column: x => x.FarmTaskId,
                        principalTable: "FarmTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubTask_EmployeeId",
                table: "SubTask",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_SubTask_FarmTaskId",
                table: "SubTask",
                column: "FarmTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Other_Task",
                table: "FarmTask",
                column: "OtherId",
                principalTable: "Other",
                principalColumn: "Id");
        }
    }
}
