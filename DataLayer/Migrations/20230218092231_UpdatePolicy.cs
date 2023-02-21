using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class UpdatePolicy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserLockUserId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserLockUserId",
                table: "AspNetUsers",
                column: "UserLockUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserLocks_UserLockUserId",
                table: "AspNetUsers",
                column: "UserLockUserId",
                principalTable: "UserLocks",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserLocks_UserLockUserId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserLockUserId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserLockUserId",
                table: "AspNetUsers");
        }
    }
}
