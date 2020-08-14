using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class RemoveStatFieldsFromEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClickDateTime",
                schema: "public",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "DeliverDateTime",
                schema: "public",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "OpenDateTime",
                schema: "public",
                table: "Emails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ClickDateTime",
                schema: "public",
                table: "Emails",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliverDateTime",
                schema: "public",
                table: "Emails",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OpenDateTime",
                schema: "public",
                table: "Emails",
                type: "timestamp without time zone",
                nullable: true);
        }
    }
}