using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SomoTaskManagement.Data.Migrations
{
    public partial class AddSubmitday : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDay",
                table: "SubTask");

            migrationBuilder.RenameColumn(
                name: "StartDay",
                table: "SubTask",
                newName: "DaySubmit");

            migrationBuilder.AddColumn<bool>(
                name: "IsCancel",
                table: "TaskEvidence",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCancel",
                table: "TaskEvidence");

            migrationBuilder.RenameColumn(
                name: "DaySubmit",
                table: "SubTask",
                newName: "StartDay");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDay",
                table: "SubTask",
                type: "datetime2",
                nullable: true);
        }
    }
}
