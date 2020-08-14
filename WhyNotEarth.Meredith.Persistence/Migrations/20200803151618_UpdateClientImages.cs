using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class UpdateClientImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Clients_ClientId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Video_Clients_ClientId",
                table: "Video");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Video",
                table: "Video");

            migrationBuilder.RenameTable(
                name: "Video",
                newName: "Videos",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_Video_ClientId",
                schema: "public",
                table: "Videos",
                newName: "IX_Videos_ClientId");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                schema: "public",
                table: "Videos",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Videos",
                schema: "public",
                table: "Videos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Clients_ClientId",
                table: "Images",
                column: "ClientId",
                principalSchema: "ModuleBrowTricks",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_Clients_ClientId",
                schema: "public",
                table: "Videos",
                column: "ClientId",
                principalSchema: "ModuleBrowTricks",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Clients_ClientId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Videos_Clients_ClientId",
                schema: "public",
                table: "Videos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Videos",
                schema: "public",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                schema: "public",
                table: "Videos");

            migrationBuilder.RenameTable(
                name: "Videos",
                schema: "public",
                newName: "Video");

            migrationBuilder.RenameIndex(
                name: "IX_Videos_ClientId",
                table: "Video",
                newName: "IX_Video_ClientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Video",
                table: "Video",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Clients_ClientId",
                table: "Images",
                column: "ClientId",
                principalSchema: "ModuleBrowTricks",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Video_Clients_ClientId",
                table: "Video",
                column: "ClientId",
                principalSchema: "ModuleBrowTricks",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}