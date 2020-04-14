using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WhyNotEarth.Meredith.Data.Entity.Migrations
{
    public partial class Page_Add_CreatedAdd_And_LastEdited_And_PageKeywords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Pages",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEdited",
                table: "Pages",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PageKeyword",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Keyword = table.Column<string>(nullable: true),
                    PageId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageKeyword", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageKeyword_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PageKeyword_PageId",
                table: "PageKeyword",
                column: "PageId");

            //Update the createddate for todays date
            migrationBuilder.Sql(@"UPDATE ""Pages"" SET ""CreatedDate"" = now()");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageKeyword");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "LastEdited",
                table: "Pages");
        }
    }
}
