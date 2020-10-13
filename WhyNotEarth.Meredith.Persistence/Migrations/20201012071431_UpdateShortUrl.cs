using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class UpdateShortUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM \"public\".\"ShortUrls\"");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ShortUrls");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ShortUrls",
                nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ShortUrls",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "ShortUrls",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ShortUrls");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "ShortUrls");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "ShortUrls",
                type: "text",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);
        }
    }
}
