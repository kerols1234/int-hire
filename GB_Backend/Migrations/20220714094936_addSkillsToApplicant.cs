using Microsoft.EntityFrameworkCore.Migrations;

namespace GB_Backend.Migrations
{
    public partial class addSkillsToApplicant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "skills",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "skills",
                table: "AspNetUsers");
        }
    }
}
