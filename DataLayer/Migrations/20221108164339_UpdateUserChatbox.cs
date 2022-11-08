using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class UpdateUserChatbox : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastMessageCreated",
                table: "PatientInChatBoxOfReceptions");

            migrationBuilder.CreateIndex(
                name: "IX_PatientInChatBoxOfReceptions_LastMessageId",
                table: "PatientInChatBoxOfReceptions",
                column: "LastMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientInChatBoxOfReceptions_ChatMessages_LastMessageId",
                table: "PatientInChatBoxOfReceptions",
                column: "LastMessageId",
                principalTable: "ChatMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientInChatBoxOfReceptions_ChatMessages_LastMessageId",
                table: "PatientInChatBoxOfReceptions");

            migrationBuilder.DropIndex(
                name: "IX_PatientInChatBoxOfReceptions_LastMessageId",
                table: "PatientInChatBoxOfReceptions");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastMessageCreated",
                table: "PatientInChatBoxOfReceptions",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
