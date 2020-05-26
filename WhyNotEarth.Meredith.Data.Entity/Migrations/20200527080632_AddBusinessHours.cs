using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WhyNotEarth.Meredith.Data.Entity.Migrations
{
    public partial class AddBusinessHours : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_PaymentMethods_PaymentMethodId",
                schema: "ModuleShop",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "PaymentMethods",
                schema: "ModuleShop");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PaymentMethodId",
                schema: "ModuleShop",
                table: "Orders");

            migrationBuilder.AddColumn<byte>(
                name: "PaymentMethodType",
                schema: "ModuleShop",
                table: "Orders",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Tenants",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "NotificationType",
                table: "Tenants",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "PaymentMethodType",
                table: "Tenants",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateTable(
                name: "BusinessHours",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    TenantId = table.Column<int>(nullable: false),
                    DayOfWeek = table.Column<int>(nullable: false),
                    IsClosed = table.Column<bool>(nullable: false),
                    OpeningTime = table.Column<TimeSpan>(nullable: true),
                    ClosingTime = table.Column<TimeSpan>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessHours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessHours_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessHours_TenantId",
                table: "BusinessHours",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessHours");

            migrationBuilder.DropColumn(
                name: "PaymentMethodType",
                schema: "ModuleShop",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "NotificationType",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "PaymentMethodType",
                table: "Tenants");

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                schema: "ModuleShop",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentMethodId",
                schema: "ModuleShop",
                table: "Orders",
                column: "PaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_PaymentMethods_PaymentMethodId",
                schema: "ModuleShop",
                table: "Orders",
                column: "PaymentMethodId",
                principalSchema: "ModuleShop",
                principalTable: "PaymentMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
