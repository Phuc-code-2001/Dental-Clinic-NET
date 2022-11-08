using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DataLayer.Migrations
{
    public partial class FixChatBoxUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientInChatBoxOfReceptions");

            migrationBuilder.CreateTable(
                name: "UsersInChatBoxOfReception",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    HasMessageUnRead = table.Column<bool>(type: "boolean", nullable: false),
                    LastMessageId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersInChatBoxOfReception", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersInChatBoxOfReception_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsersInChatBoxOfReception_ChatMessages_LastMessageId",
                        column: x => x.LastMessageId,
                        principalTable: "ChatMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersInChatBoxOfReception_LastMessageId",
                table: "UsersInChatBoxOfReception",
                column: "LastMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersInChatBoxOfReception_UserId",
                table: "UsersInChatBoxOfReception",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsersInChatBoxOfReception");

            migrationBuilder.CreateTable(
                name: "PatientInChatBoxOfReceptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HasMessageUnRead = table.Column<bool>(type: "boolean", nullable: false),
                    LastMessageId = table.Column<int>(type: "integer", nullable: false),
                    PatientId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientInChatBoxOfReceptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientInChatBoxOfReceptions_ChatMessages_LastMessageId",
                        column: x => x.LastMessageId,
                        principalTable: "ChatMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientInChatBoxOfReceptions_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientInChatBoxOfReceptions_LastMessageId",
                table: "PatientInChatBoxOfReceptions",
                column: "LastMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientInChatBoxOfReceptions_PatientId",
                table: "PatientInChatBoxOfReceptions",
                column: "PatientId");
        }
    }
}
