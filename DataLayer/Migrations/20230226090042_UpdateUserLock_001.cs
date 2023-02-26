using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class UpdateUserLock_001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserLocks_UserLockUserId",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLocks",
                table: "UserLocks");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserLockUserId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BaseUser",
                table: "UserLocks");

            migrationBuilder.DropColumn(
                name: "UserLockUserId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserLocks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserLocks",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "BaseUserId",
                table: "UserLocks",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLocks",
                table: "UserLocks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserLocks_BaseUserId",
                table: "UserLocks",
                column: "BaseUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLocks_AspNetUsers_BaseUserId",
                table: "UserLocks",
                column: "BaseUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLocks_AspNetUsers_BaseUserId",
                table: "UserLocks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLocks",
                table: "UserLocks");

            migrationBuilder.DropIndex(
                name: "IX_UserLocks_BaseUserId",
                table: "UserLocks");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserLocks");

            migrationBuilder.DropColumn(
                name: "BaseUserId",
                table: "UserLocks");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserLocks",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BaseUser",
                table: "UserLocks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserLockUserId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLocks",
                table: "UserLocks",
                column: "UserId");

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
    }
}
