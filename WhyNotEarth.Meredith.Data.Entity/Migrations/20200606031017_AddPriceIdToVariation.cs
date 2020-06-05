using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Data.Entity.Migrations
{
    public partial class AddPriceIdToVariation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PriceId",
                schema: "ModuleShop",
                table: "Variations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Variations_PriceId",
                schema: "ModuleShop",
                table: "Variations",
                column: "PriceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Variations_Prices_PriceId",
                schema: "ModuleShop",
                table: "Variations",
                column: "PriceId",
                principalSchema: "ModuleShop",
                principalTable: "Prices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Variations_Prices_PriceId",
                schema: "ModuleShop",
                table: "Variations");

            migrationBuilder.DropIndex(
                name: "IX_Variations_PriceId",
                schema: "ModuleShop",
                table: "Variations");

            migrationBuilder.DropColumn(
                name: "PriceId",
                schema: "ModuleShop",
                table: "Variations");
        }
    }
}
