using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class AddJumpStart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Pages_PageId",
                table: "Images");

            migrationBuilder.EnsureSchema(
                name: "ModuleVolkswagen");

            migrationBuilder.AlterColumn<int>(
                name: "PageId",
                table: "Images",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            // Custom
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Images",
                nullable: false,
                defaultValue: "PageImage");

            migrationBuilder.AddColumn<int>(
                name: "PostId",
                table: "Images",
                nullable: true);

            // Custom
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Categories",
                nullable: false,
                defaultValue: "PageCategory");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Categories",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "JumpStarts",
                schema: "ModuleVolkswagen",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    DateTime = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    HasPdf = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JumpStarts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                schema: "ModuleVolkswagen",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CategoryId = table.Column<int>(nullable: false),
                    Headline = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    JumpStartId = table.Column<int>(nullable: true),
                    Order = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posts_JumpStarts_JumpStartId",
                        column: x => x.JumpStartId,
                        principalSchema: "ModuleVolkswagen",
                        principalTable: "JumpStarts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Images_PostId",
                table: "Images",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CategoryId",
                schema: "ModuleVolkswagen",
                table: "Posts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_JumpStartId",
                schema: "ModuleVolkswagen",
                table: "Posts",
                column: "JumpStartId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Posts_PostId",
                table: "Images",
                column: "PostId",
                principalSchema: "ModuleVolkswagen",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Pages_PageId",
                table: "Images",
                column: "PageId",
                principalTable: "Pages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // Custom
            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "Images",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "Categories",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData("Roles",
                new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                new object[] { 1, "Developer", "DEVELOPER", "ad35c612-7f0e-4f7e-82c6-d5a82764f325" });

            migrationBuilder.InsertData("Roles",
                new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                new object[] { 2, "VolkswagenAdmin", "VOLKSWAGENADMIN", "e4b3490d-c55d-43bb-88b0-f73ad5d447c5" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Posts_PostId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Pages_PageId",
                table: "Images");

            migrationBuilder.DropTable(
                name: "Posts",
                schema: "ModuleVolkswagen");

            migrationBuilder.DropTable(
                name: "JumpStarts",
                schema: "ModuleVolkswagen");

            migrationBuilder.DropIndex(
                name: "IX_Images_PostId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Categories");

            migrationBuilder.AlterColumn<int>(
                name: "PageId",
                table: "Images",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Pages_PageId",
                table: "Images",
                column: "PageId",
                principalTable: "Pages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // Custom
            migrationBuilder.DeleteData("AspNetRoleClaims", "RoleId", 1);
            migrationBuilder.DeleteData("AspNetUserRoles", "RoleId", 1);
            migrationBuilder.DeleteData("Roles", "Id", 1);

            migrationBuilder.DeleteData("AspNetRoleClaims", "RoleId", 2);
            migrationBuilder.DeleteData("AspNetUserRoles", "RoleId", 2);
            migrationBuilder.DeleteData("Roles", "Id", 2);
        }
    }
}