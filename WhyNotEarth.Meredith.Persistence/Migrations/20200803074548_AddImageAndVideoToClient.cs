using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WhyNotEarth.Meredith.Data.Entity.Migrations
{
    public partial class AddImageAndVideoToClient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "Images",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CloudinaryPublicId",
                table: "Images",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Video",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CloudinaryPublicId = table.Column<string>(nullable: false),
                    Url = table.Column<string>(nullable: false),
                    ClientId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Video", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Video_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "ModuleBrowTricks",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Images_ClientId",
                table: "Images",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Video_ClientId",
                table: "Video",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Clients_ClientId",
                table: "Images",
                column: "ClientId",
                principalSchema: "ModuleBrowTricks",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Clients_ClientId",
                table: "Images");

            migrationBuilder.DropTable(
                name: "Video");

            migrationBuilder.DropIndex(
                name: "IX_Images_ClientId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "CloudinaryPublicId",
                table: "Images");
        }
    }
}
