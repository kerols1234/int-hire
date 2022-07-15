using Microsoft.EntityFrameworkCore.Migrations;

namespace GB_Backend.Migrations
{
    public partial class renameTypesCloumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StrengthsAndWeakness",
                table: "MBTITypes",
                newName: "StrengthsandWeaknesses");

            migrationBuilder.RenameColumn(
                name: "RomanticRelationship",
                table: "MBTITypes",
                newName: "RomanticRelationships");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StrengthsandWeaknesses",
                table: "MBTITypes",
                newName: "StrengthsAndWeakness");

            migrationBuilder.RenameColumn(
                name: "RomanticRelationships",
                table: "MBTITypes",
                newName: "RomanticRelationship");
        }
    }
}
