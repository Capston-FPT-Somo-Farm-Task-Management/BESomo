using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SomoTaskManagement.Data.Migrations
{
    public partial class EvidenceType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCancel",
                table: "TaskEvidence");

            migrationBuilder.AddColumn<int>(
                name: "EvidenceType",
                table: "TaskEvidence",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EvidenceType",
                table: "TaskEvidence");

            migrationBuilder.AddColumn<bool>(
                name: "IsCancel",
                table: "TaskEvidence",
                type: "bit",
                nullable: true);
        }
    }
}
