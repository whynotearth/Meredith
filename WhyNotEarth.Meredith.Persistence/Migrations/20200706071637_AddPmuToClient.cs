using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class AddPmuToClient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowPhoto",
                schema: "ModuleBrowTricks",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Conditions",
                schema: "ModuleBrowTricks",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Initials",
                schema: "ModuleBrowTricks",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTakingBloodThinner",
                schema: "ModuleBrowTricks",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderCareOfPhysician",
                schema: "ModuleBrowTricks",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhysicianName",
                schema: "ModuleBrowTricks",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhysicianPhoneNumber",
                schema: "ModuleBrowTricks",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Signature",
                schema: "ModuleBrowTricks",
                table: "Clients",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}