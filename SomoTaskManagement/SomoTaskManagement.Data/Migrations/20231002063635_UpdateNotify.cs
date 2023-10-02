using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SomoTaskManagement.Data.Migrations
{
    public partial class UpdateNotify : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HubConnectionId",
                table: "Member",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotificcationId",
                table: "Member",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Member_HubConnectionId",
                table: "Member",
                column: "HubConnectionId",
                unique: true,
                filter: "[HubConnectionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Member_NotificcationId",
                table: "Member",
                column: "NotificcationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Member_HubConnection_HubConnectionId",
                table: "Member",
                column: "HubConnectionId",
                principalTable: "HubConnection",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Member",
                table: "Member",
                column: "NotificcationId",
                principalTable: "Notification",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Member_HubConnection_HubConnectionId",
                table: "Member");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Member",
                table: "Member");

            migrationBuilder.DropIndex(
                name: "IX_Member_HubConnectionId",
                table: "Member");

            migrationBuilder.DropIndex(
                name: "IX_Member_NotificcationId",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "HubConnectionId",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "NotificcationId",
                table: "Member");
        }
    }
}
