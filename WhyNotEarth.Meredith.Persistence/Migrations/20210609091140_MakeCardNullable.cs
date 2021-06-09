using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class MakeCardNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Cards_CardId",
                schema: "Platform",
                table: "Subscriptions");

            migrationBuilder.AlterColumn<int>(
                name: "CardId",
                schema: "Platform",
                table: "Subscriptions",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Cards_CardId",
                schema: "Platform",
                table: "Subscriptions",
                column: "CardId",
                principalSchema: "Platform",
                principalTable: "Cards",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Cards_CardId",
                schema: "Platform",
                table: "Subscriptions");

            migrationBuilder.AlterColumn<int>(
                name: "CardId",
                schema: "Platform",
                table: "Subscriptions",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Cards_CardId",
                schema: "Platform",
                table: "Subscriptions",
                column: "CardId",
                principalSchema: "Platform",
                principalTable: "Cards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
