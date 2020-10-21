using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class AddPersonalInfoToClient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                schema: "ModuleBrowTricks",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "ModuleBrowTricks",
                table: "Clients",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "ModuleBrowTricks",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                schema: "ModuleBrowTricks",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                schema: "ModuleBrowTricks",
                table: "Clients",
                nullable: true);

            migrationBuilder.Sql(@"UPDATE ""ModuleBrowTricks"".""Clients"" SET
                ""AvatarUrl"" = ""u"".""ImageUrl"", ""Email"" = ""u"".""Email"", ""FirstName"" = ""u"".""FirstName"", ""LastName"" = ""u"".""LastName"", ""PhoneNumber"" = ""u"".""PhoneNumber""
                FROM ""public"".""Users"" AS ""u""
                WHERE ""u"".""Id"" = ""Clients"".""UserId""");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                schema: "ModuleBrowTricks",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "ModuleBrowTricks",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "ModuleBrowTricks",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "LastName",
                schema: "ModuleBrowTricks",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                schema: "ModuleBrowTricks",
                table: "Clients");
        }
    }
}
