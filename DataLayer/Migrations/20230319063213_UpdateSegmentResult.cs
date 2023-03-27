using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class UpdateSegmentResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TechnicanId",
                table: "SegmentationResults",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SegmentationResults_TechnicanId",
                table: "SegmentationResults",
                column: "TechnicanId");

            migrationBuilder.AddForeignKey(
                name: "FK_SegmentationResults_AspNetUsers_TechnicanId",
                table: "SegmentationResults",
                column: "TechnicanId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SegmentationResults_AspNetUsers_TechnicanId",
                table: "SegmentationResults");

            migrationBuilder.DropIndex(
                name: "IX_SegmentationResults_TechnicanId",
                table: "SegmentationResults");

            migrationBuilder.DropColumn(
                name: "TechnicanId",
                table: "SegmentationResults");
        }
    }
}
