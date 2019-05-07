using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Data.Entity.Migrations
{
    public partial class CardSupport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Card_Page_PageId",
                table: "Card");

            migrationBuilder.DropForeignKey(
                name: "FK_Page_Companies_CompanyId",
                table: "Page");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Page",
                table: "Page");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Card",
                table: "Card");

            migrationBuilder.RenameTable(
                name: "Page",
                newName: "Pages");

            migrationBuilder.RenameTable(
                name: "Card",
                newName: "Cards");

            migrationBuilder.RenameIndex(
                name: "IX_Page_CompanyId",
                table: "Pages",
                newName: "IX_Pages_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_Card_PageId",
                table: "Cards",
                newName: "IX_Cards_PageId");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Companies",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Companies",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BackgroundImage",
                table: "Pages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CallToAction",
                table: "Pages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CallToActionLink",
                table: "Pages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Header",
                table: "Pages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Pages",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PosterUrl",
                table: "Cards",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CallToActionUrl",
                table: "Cards",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CallToAction",
                table: "Cards",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BackgroundUrl",
                table: "Cards",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CardType",
                table: "Cards",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Cards",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Cards",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pages",
                table: "Pages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cards",
                table: "Cards",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Pages_PageId",
                table: "Cards",
                column: "PageId",
                principalTable: "Pages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pages_Companies_CompanyId",
                table: "Pages",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_Pages_PageId",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_Pages_Companies_CompanyId",
                table: "Pages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pages",
                table: "Pages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cards",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "BackgroundImage",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "CallToAction",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "CallToActionLink",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "Header",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "CardType",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "Cards");

            migrationBuilder.RenameTable(
                name: "Pages",
                newName: "Page");

            migrationBuilder.RenameTable(
                name: "Cards",
                newName: "Card");

            migrationBuilder.RenameIndex(
                name: "IX_Pages_CompanyId",
                table: "Page",
                newName: "IX_Page_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_Cards_PageId",
                table: "Card",
                newName: "IX_Card_PageId");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Companies",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Companies",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PosterUrl",
                table: "Card",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CallToActionUrl",
                table: "Card",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CallToAction",
                table: "Card",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BackgroundUrl",
                table: "Card",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Page",
                table: "Page",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Card",
                table: "Card",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Card_Page_PageId",
                table: "Card",
                column: "PageId",
                principalTable: "Page",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Page_Companies_CompanyId",
                table: "Page",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
