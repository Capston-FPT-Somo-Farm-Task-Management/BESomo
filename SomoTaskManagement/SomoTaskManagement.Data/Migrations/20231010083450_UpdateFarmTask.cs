using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SomoTaskManagement.Data.Migrations
{
    public partial class UpdateFarmTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Member_Task",
                table: "FarmTask");

            migrationBuilder.DropIndex(
                name: "IX_FarmTask_MemberId",
                table: "FarmTask");

            migrationBuilder.RenameColumn(
                name: "ReceiverId",
                table: "FarmTask",
                newName: "ManagerId");

            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "FarmTask",
                newName: "SuppervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_FarmTask_ManagerId",
                table: "FarmTask",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Member_Task",
                table: "FarmTask",
                column: "ManagerId",
                principalTable: "Member",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Member_Task",
                table: "FarmTask");

            migrationBuilder.DropIndex(
                name: "IX_FarmTask_ManagerId",
                table: "FarmTask");

            migrationBuilder.RenameColumn(
                name: "SuppervisorId",
                table: "FarmTask",
                newName: "MemberId");

            migrationBuilder.RenameColumn(
                name: "ManagerId",
                table: "FarmTask",
                newName: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_FarmTask_MemberId",
                table: "FarmTask",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Member_Task",
                table: "FarmTask",
                column: "MemberId",
                principalTable: "Member",
                principalColumn: "Id");
        }
    }
}
