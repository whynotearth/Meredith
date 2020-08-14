using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class RemoveRecipientNameColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "ModuleVolkswagen",
                table: "Recipients");

            migrationBuilder.DropColumn(
                name: "LastName",
                schema: "ModuleVolkswagen",
                table: "Recipients");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliverDateTime",
                schema: "ModuleVolkswagen",
                table: "MemoRecipients",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OpenDateTime",
                schema: "ModuleVolkswagen",
                table: "MemoRecipients",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliverDateTime",
                schema: "ModuleVolkswagen",
                table: "MemoRecipients");

            migrationBuilder.DropColumn(
                name: "OpenDateTime",
                schema: "ModuleVolkswagen",
                table: "MemoRecipients");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "ModuleVolkswagen",
                table: "Recipients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                schema: "ModuleVolkswagen",
                table: "Recipients",
                type: "text",
                nullable: true);
        }
    }
}