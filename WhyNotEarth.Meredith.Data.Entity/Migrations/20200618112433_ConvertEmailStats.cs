using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System;

namespace WhyNotEarth.Meredith.Data.Entity.Migrations
{
    public partial class ConvertEmailStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailEvents",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    EmailId = table.Column<int>(nullable: false),
                    Type = table.Column<byte>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailEvents_Emails_EmailId",
                        column: x => x.EmailId,
                        principalSchema: "public",
                        principalTable: "Emails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailEvents_EmailId",
                schema: "public",
                table: "EmailEvents",
                column: "EmailId");

            migrationBuilder.Sql(@"insert into ""public"".""EmailEvents"" (""EmailId"", ""Type"", ""DateTime"") 
	            select ""Id"", 1, ""DeliverDateTime"" from ""public"".""Emails"" where ""DeliverDateTime"" is not null");

            migrationBuilder.Sql(@"insert into ""public"".""EmailEvents"" (""EmailId"", ""Type"", ""DateTime"") 
	            select ""Id"", 2, ""OpenDateTime"" from ""public"".""Emails"" where ""OpenDateTime"" is not null");

            migrationBuilder.Sql(@"insert into ""public"".""EmailEvents"" (""EmailId"", ""Type"", ""DateTime"") 
	            select ""Id"", 3, ""ClickDateTime"" from ""public"".""Emails"" where ""ClickDateTime"" is not null");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailEvents",
                schema: "public");

            migrationBuilder.Sql(@"delete from ""public"".""EmailEvents""");
        }
    }
}
