using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Data.Entity.Migrations
{
    public partial class RefactorCreatedAtColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreationDateTime",
                schema: "public",
                table: "Emails",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreationDateTime",
                schema: "ModuleVolkswagen",
                table: "Recipients",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreationDateTime",
                schema: "ModuleVolkswagen",
                table: "Memos",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Created",
                schema: "ModuleShop",
                table: "Reservations",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Created",
                schema: "ModuleShop",
                table: "Payments",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreationDateTime",
                table: "Pages",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "EditDateTime",
                table: "Pages",
                newName: "EditedAt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "public",
                table: "Emails",
                newName: "CreationDateTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "ModuleVolkswagen",
                table: "Recipients",
                newName: "CreationDateTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "ModuleVolkswagen",
                table: "Memos",
                newName: "CreationDateTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "ModuleShop",
                table: "Reservations",
                newName: "Created");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "ModuleShop",
                table: "Payments",
                newName: "Created");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Pages",
                newName: "CreationDateTime");

            migrationBuilder.RenameColumn(
                name: "EditedAt",
                table: "Pages",
                newName: "EditDateTime");
        }
    }
}
