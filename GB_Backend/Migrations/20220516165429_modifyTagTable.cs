using Microsoft.EntityFrameworkCore.Migrations;

namespace GB_Backend.Migrations
{
    public partial class modifyTagTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_AspNetUsers_ApplicantUserId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Jobs_JobId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_ApplicantUserId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_JobId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "ApplicantUserId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "JobId",
                table: "Tags");

            migrationBuilder.CreateTable(
                name: "ApplicantUserTag",
                columns: table => new
                {
                    ApplicantUsersId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TagsName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantUserTag", x => new { x.ApplicantUsersId, x.TagsName });
                    table.ForeignKey(
                        name: "FK_ApplicantUserTag_AspNetUsers_ApplicantUsersId",
                        column: x => x.ApplicantUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicantUserTag_Tags_TagsName",
                        column: x => x.TagsName,
                        principalTable: "Tags",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobTag",
                columns: table => new
                {
                    JobsId = table.Column<int>(type: "int", nullable: false),
                    TagsName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTag", x => new { x.JobsId, x.TagsName });
                    table.ForeignKey(
                        name: "FK_JobTag_Jobs_JobsId",
                        column: x => x.JobsId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobTag_Tags_TagsName",
                        column: x => x.TagsName,
                        principalTable: "Tags",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantUserTag_TagsName",
                table: "ApplicantUserTag",
                column: "TagsName");

            migrationBuilder.CreateIndex(
                name: "IX_JobTag_TagsName",
                table: "JobTag",
                column: "TagsName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicantUserTag");

            migrationBuilder.DropTable(
                name: "JobTag");

            migrationBuilder.AddColumn<string>(
                name: "ApplicantUserId",
                table: "Tags",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JobId",
                table: "Tags",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ApplicantUserId",
                table: "Tags",
                column: "ApplicantUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_JobId",
                table: "Tags",
                column: "JobId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_AspNetUsers_ApplicantUserId",
                table: "Tags",
                column: "ApplicantUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Jobs_JobId",
                table: "Tags",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
