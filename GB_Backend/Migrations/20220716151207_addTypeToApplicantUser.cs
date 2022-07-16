using Microsoft.EntityFrameworkCore.Migrations;

namespace GB_Backend.Migrations
{
    public partial class addTypeToApplicantUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplicants_MBTITypes_MBTITypeId",
                table: "JobApplicants");

            migrationBuilder.DropIndex(
                name: "IX_JobApplicants_MBTITypeId",
                table: "JobApplicants");

            migrationBuilder.DropColumn(
                name: "MBTITypeId",
                table: "JobApplicants");

            migrationBuilder.AddColumn<string>(
                name: "TestPersonality",
                table: "JobApplicants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TweeterPersonality",
                table: "JobApplicants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TestPersonality",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TwitterPersonality",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestPersonality",
                table: "JobApplicants");

            migrationBuilder.DropColumn(
                name: "TweeterPersonality",
                table: "JobApplicants");

            migrationBuilder.DropColumn(
                name: "TestPersonality",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TwitterPersonality",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "MBTITypeId",
                table: "JobApplicants",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobApplicants_MBTITypeId",
                table: "JobApplicants",
                column: "MBTITypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplicants_MBTITypes_MBTITypeId",
                table: "JobApplicants",
                column: "MBTITypeId",
                principalTable: "MBTITypes",
                principalColumn: "Type",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
