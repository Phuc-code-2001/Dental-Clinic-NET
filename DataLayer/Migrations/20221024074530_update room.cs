using Microsoft.EntityFrameworkCore.Migrations;

namespace DataLayer.Migrations
{
    public partial class updateroom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Devices_device_id",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_device_id",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "device_id",
                table: "Rooms");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_room_id",
                table: "Devices",
                column: "room_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Rooms_room_id",
                table: "Devices",
                column: "room_id",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Rooms_room_id",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_room_id",
                table: "Devices");

            migrationBuilder.AddColumn<int>(
                name: "device_id",
                table: "Rooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_device_id",
                table: "Rooms",
                column: "device_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Devices_device_id",
                table: "Rooms",
                column: "device_id",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
