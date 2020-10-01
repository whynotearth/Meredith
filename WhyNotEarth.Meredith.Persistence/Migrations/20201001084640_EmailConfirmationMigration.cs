using Microsoft.EntityFrameworkCore.Migrations;

namespace WhyNotEarth.Meredith.Persistence.Migrations
{
    public partial class EmailConfirmationMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE ""public"".""Users"" SET ""EmailConfirmed"" = true 
                WHERE ""Users"".""Id"" IN (SELECT DISTINCT ""u"".""Id"" FROM ""public"".""Users"" AS u JOIN ""public"".""AspNetUserLogins""
                    ON ""UserId"" = ""u"".""Id"")");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
