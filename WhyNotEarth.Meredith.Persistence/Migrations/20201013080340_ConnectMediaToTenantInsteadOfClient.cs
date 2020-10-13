using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class ConnectMediaToTenantInsteadOfClient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                schema: "public",
                table: "Videos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Images",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Videos_TenantId",
                schema: "public",
                table: "Videos",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_TenantId",
                table: "Images",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Tenants_TenantId",
                table: "Images",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_Tenants_TenantId",
                schema: "public",
                table: "Videos",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql(@"UPDATE ""public"".""Images"" as i SET ""Discriminator"" = 'BrowTricksImage', ""TenantId"" = (SELECT c.""TenantId"" FROM ""ModuleBrowTricks"".""Clients"" AS c where c.""Id"" = i.""ClientId"")
                WHERE ""ClientId"" IS NOT NULL");

            migrationBuilder.Sql(@"UPDATE ""public"".""Videos"" as v SET ""Discriminator"" = 'BrowTricksVideo', ""TenantId"" = (SELECT c.""TenantId"" FROM ""ModuleBrowTricks"".""Clients"" AS c where c.""Id"" = v.""ClientId"")
                WHERE ""ClientId"" IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Tenants_TenantId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Videos_Tenants_TenantId",
                schema: "public",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_Videos_TenantId",
                schema: "public",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_Images_TenantId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "public",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Images");
        }
    }
}
