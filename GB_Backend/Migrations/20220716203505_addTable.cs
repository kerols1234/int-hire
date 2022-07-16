using Microsoft.EntityFrameworkCore.Migrations;

namespace GB_Backend.Migrations
{
    public partial class addTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MPTIModels",
                columns: table => new
                {
                    personality = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Overview = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValuesandMotivations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Strengths = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Weaknesses = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GrowthandDevelopment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MainTriat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AtWork = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KindredSpirits = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IntriguingDifferences = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PotentialComplements = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChallengingOpposites = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Topcareers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CareerstoAvoid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Characteristics = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MPTIModels", x => x.personality);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MPTIModels");
        }
    }
}
