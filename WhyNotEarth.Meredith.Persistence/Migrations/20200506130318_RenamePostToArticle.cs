using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class RenamePostToArticle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Categories_CategoryId",
                schema: "ModuleVolkswagen",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Images_ImageId",
                schema: "ModuleVolkswagen",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_JumpStarts_JumpStartId",
                schema: "ModuleVolkswagen",
                table: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Posts",
                schema: "ModuleVolkswagen",
                table: "Posts");

            migrationBuilder.RenameTable(
                name: "Posts",
                schema: "ModuleVolkswagen",
                newName: "Articles",
                newSchema: "ModuleVolkswagen");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_JumpStartId",
                schema: "ModuleVolkswagen",
                table: "Articles",
                newName: "IX_Articles_JumpStartId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_ImageId",
                schema: "ModuleVolkswagen",
                table: "Articles",
                newName: "IX_Articles_ImageId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_CategoryId",
                schema: "ModuleVolkswagen",
                table: "Articles",
                newName: "IX_Articles_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Articles",
                schema: "ModuleVolkswagen",
                table: "Articles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Categories_CategoryId",
                schema: "ModuleVolkswagen",
                table: "Articles",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Images_ImageId",
                schema: "ModuleVolkswagen",
                table: "Articles",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_JumpStarts_JumpStartId",
                schema: "ModuleVolkswagen",
                table: "Articles",
                column: "JumpStartId",
                principalSchema: "ModuleVolkswagen",
                principalTable: "JumpStarts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(
                "UPDATE \"public\".\"Images\" set \"Discriminator\" = 'ArticleImage' WHERE \"Discriminator\" = 'PostImage'");

            migrationBuilder.Sql(
                "UPDATE \"public\".\"Categories\" set \"Discriminator\" = 'ArticleCategory' WHERE \"Discriminator\" = 'PostCategory'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Categories_CategoryId",
                schema: "ModuleVolkswagen",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Images_ImageId",
                schema: "ModuleVolkswagen",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK_Articles_JumpStarts_JumpStartId",
                schema: "ModuleVolkswagen",
                table: "Articles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Articles",
                schema: "ModuleVolkswagen",
                table: "Articles");

            migrationBuilder.RenameTable(
                name: "Articles",
                schema: "ModuleVolkswagen",
                newName: "Posts",
                newSchema: "ModuleVolkswagen");

            migrationBuilder.RenameIndex(
                name: "IX_Articles_JumpStartId",
                schema: "ModuleVolkswagen",
                table: "Posts",
                newName: "IX_Posts_JumpStartId");

            migrationBuilder.RenameIndex(
                name: "IX_Articles_ImageId",
                schema: "ModuleVolkswagen",
                table: "Posts",
                newName: "IX_Posts_ImageId");

            migrationBuilder.RenameIndex(
                name: "IX_Articles_CategoryId",
                schema: "ModuleVolkswagen",
                table: "Posts",
                newName: "IX_Posts_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Posts",
                schema: "ModuleVolkswagen",
                table: "Posts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Categories_CategoryId",
                schema: "ModuleVolkswagen",
                table: "Posts",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Images_ImageId",
                schema: "ModuleVolkswagen",
                table: "Posts",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_JumpStarts_JumpStartId",
                schema: "ModuleVolkswagen",
                table: "Posts",
                column: "JumpStartId",
                principalSchema: "ModuleVolkswagen",
                principalTable: "JumpStarts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(
                "UPDATE \"public\".\"Images\" set \"Discriminator\" = 'PostImage' WHERE \"Discriminator\" = 'ArticleImage'");

            migrationBuilder.Sql(
                "UPDATE \"public\".\"Categories\" set \"Discriminator\" = 'PostCategory' WHERE \"Discriminator\" = 'ArticleCategory'");
        }
    }
}