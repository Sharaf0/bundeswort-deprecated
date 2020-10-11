using Microsoft.EntityFrameworkCore.Migrations;

namespace Bundeswort.Migrations
{
    public partial class UnaccentExtension : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"CREATE EXTENSION unaccent");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DROP EXTENSION unaccent");
        }
    }
}
