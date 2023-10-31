using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SomoTaskManagement.Data.Migrations
{
    public partial class Notification_Member : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Member",
                table: "Member");

            migrationBuilder.DropIndex(
                name: "IX_Member_NotificcationId",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "NotificcationId",
                table: "Member");

            migrationBuilder.CreateTable(
                name: "Notification_Member",
                columns: table => new
                {
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    NotificationId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification_Member", x => new { x.NotificationId, x.MemberId });
                    table.ForeignKey(
                        name: "FK_Notification_Member_Member",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TNotification_Member_Notification",
                        column: x => x.NotificationId,
                        principalTable: "Notification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notification_Member_MemberId",
                table: "Notification_Member",
                column: "MemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notification_Member");

            migrationBuilder.AddColumn<int>(
                name: "NotificcationId",
                table: "Member",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Member_NotificcationId",
                table: "Member",
                column: "NotificcationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Member",
                table: "Member",
                column: "NotificcationId",
                principalTable: "Notification",
                principalColumn: "Id");
        }
    }
}
