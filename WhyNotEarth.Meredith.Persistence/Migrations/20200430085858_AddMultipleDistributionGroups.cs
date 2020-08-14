using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class AddMultipleDistributionGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DistributionGroup",
                schema: "ModuleVolkswagen",
                table: "Memos",
                newName: "DistributionGroups");

            migrationBuilder.AlterColumn<string>(
                name: "To",
                schema: "ModuleVolkswagen",
                table: "Memos",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                schema: "ModuleVolkswagen",
                table: "Memos",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "ModuleVolkswagen",
                table: "Memos",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Date",
                schema: "ModuleVolkswagen",
                table: "Memos",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DistributionGroups",
                schema: "ModuleVolkswagen",
                table: "Memos",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DistributionGroups",
                schema: "ModuleVolkswagen",
                table: "Memos",
                newName: "DistributionGroup");

            migrationBuilder.AlterColumn<string>(
                name: "To",
                schema: "ModuleVolkswagen",
                table: "Memos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                schema: "ModuleVolkswagen",
                table: "Memos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "ModuleVolkswagen",
                table: "Memos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Date",
                schema: "ModuleVolkswagen",
                table: "Memos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "DistributionGroup",
                schema: "ModuleVolkswagen",
                table: "Memos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}