using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Data.Entity.Migrations
{
    public partial class RemovePmuColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowPhoto",
                schema: "ModuleBrowTricks",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Conditions",
                schema: "ModuleBrowTricks",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Initials",
                schema: "ModuleBrowTricks",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "IsPmuCompleted",
                schema: "ModuleBrowTricks",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "IsTakingBloodThinner",
                schema: "ModuleBrowTricks",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "IsUnderCareOfPhysician",
                schema: "ModuleBrowTricks",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "PhysicianName",
                schema: "ModuleBrowTricks",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "PhysicianPhoneNumber",
                schema: "ModuleBrowTricks",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Signature",
                schema: "ModuleBrowTricks",
                table: "Clients");

            migrationBuilder.AddColumn<int>(
                name: "PmuStatus",
                schema: "ModuleBrowTricks",
                table: "Clients",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PmuStatus",
                schema: "ModuleBrowTricks",
                table: "Clients");

            migrationBuilder.AddColumn<bool>(
                name: "AllowPhoto",
                schema: "ModuleBrowTricks",
                table: "Clients",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Conditions",
                schema: "ModuleBrowTricks",
                table: "Clients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Initials",
                schema: "ModuleBrowTricks",
                table: "Clients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPmuCompleted",
                schema: "ModuleBrowTricks",
                table: "Clients",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTakingBloodThinner",
                schema: "ModuleBrowTricks",
                table: "Clients",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderCareOfPhysician",
                schema: "ModuleBrowTricks",
                table: "Clients",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhysicianName",
                schema: "ModuleBrowTricks",
                table: "Clients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhysicianPhoneNumber",
                schema: "ModuleBrowTricks",
                table: "Clients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Signature",
                schema: "ModuleBrowTricks",
                table: "Clients",
                type: "text",
                nullable: true);
        }
    }
}
