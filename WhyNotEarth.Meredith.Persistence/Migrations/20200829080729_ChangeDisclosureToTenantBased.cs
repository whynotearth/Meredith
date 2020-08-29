using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class ChangeDisclosureToTenantBased : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Disclosures_Clients_ClientId",
                schema: "ModuleBrowTricks",
                table: "Disclosures");

            migrationBuilder.DropIndex(
                name: "IX_Disclosures_ClientId",
                schema: "ModuleBrowTricks",
                table: "Disclosures");

            migrationBuilder.DropColumn(
                name: "ClientId",
                schema: "ModuleBrowTricks",
                table: "Disclosures");

            migrationBuilder.DropColumn(
                name: "SignatureRequestId",
                schema: "ModuleBrowTricks",
                table: "Clients");

            migrationBuilder.Sql("TRUNCATE TABLE \"ModuleBrowTricks\".\"Disclosures\"");

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                schema: "ModuleBrowTricks",
                table: "Disclosures",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SignedAt",
                schema: "ModuleBrowTricks",
                table: "Clients",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Disclosures_TenantId",
                schema: "ModuleBrowTricks",
                table: "Disclosures",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Disclosures_Tenants_TenantId",
                schema: "ModuleBrowTricks",
                table: "Disclosures",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Disclosures_Tenants_TenantId",
                schema: "ModuleBrowTricks",
                table: "Disclosures");

            migrationBuilder.DropIndex(
                name: "IX_Disclosures_TenantId",
                schema: "ModuleBrowTricks",
                table: "Disclosures");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "ModuleBrowTricks",
                table: "Disclosures");

            migrationBuilder.DropColumn(
                name: "SignedAt",
                schema: "ModuleBrowTricks",
                table: "Clients");

            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                schema: "ModuleBrowTricks",
                table: "Disclosures",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SignatureRequestId",
                schema: "ModuleBrowTricks",
                table: "Clients",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Disclosures_ClientId",
                schema: "ModuleBrowTricks",
                table: "Disclosures",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Disclosures_Clients_ClientId",
                schema: "ModuleBrowTricks",
                table: "Disclosures",
                column: "ClientId",
                principalSchema: "ModuleBrowTricks",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
