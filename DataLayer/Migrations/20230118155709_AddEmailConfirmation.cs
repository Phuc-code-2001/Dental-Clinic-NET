using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class AddEmailConfirmation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailConfirmationUserId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EmailConfirmations",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LastRequiredCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastTimeModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailConfirmations", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EmailConfirmationUserId",
                table: "AspNetUsers",
                column: "EmailConfirmationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_EmailConfirmations_EmailConfirmationUserId",
                table: "AspNetUsers",
                column: "EmailConfirmationUserId",
                principalTable: "EmailConfirmations",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_EmailConfirmations_EmailConfirmationUserId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "EmailConfirmations");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EmailConfirmationUserId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmailConfirmationUserId",
                table: "AspNetUsers");
        }
    }
}
