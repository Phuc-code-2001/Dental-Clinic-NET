using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DataLayer.Migrations
{
    public partial class updateDeviceServiceDeleteServiceDevice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_ServiceDevices_serviceDevice_id",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_ServiceDevices_serviceDevice_id",
                table: "Services");

            migrationBuilder.DropTable(
                name: "ServiceDevices");

            migrationBuilder.DropIndex(
                name: "IX_Services_serviceDevice_id",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Devices_serviceDevice_id",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "serviceDevice_id",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "serviceDevice_id",
                table: "Devices");

            migrationBuilder.CreateTable(
                name: "DeviceService",
                columns: table => new
                {
                    DevicesId = table.Column<int>(type: "integer", nullable: false),
                    ServicesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceService", x => new { x.DevicesId, x.ServicesId });
                    table.ForeignKey(
                        name: "FK_DeviceService_Devices_DevicesId",
                        column: x => x.DevicesId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeviceService_Services_ServicesId",
                        column: x => x.ServicesId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceService_ServicesId",
                table: "DeviceService",
                column: "ServicesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceService");

            migrationBuilder.AddColumn<int>(
                name: "serviceDevice_id",
                table: "Services",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "serviceDevice_id",
                table: "Devices",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ServiceDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    device_id = table.Column<int>(type: "integer", nullable: false),
                    LastTimeModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    service_id = table.Column<int>(type: "integer", nullable: false),
                    TimeCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceDevices", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Services_serviceDevice_id",
                table: "Services",
                column: "serviceDevice_id");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_serviceDevice_id",
                table: "Devices",
                column: "serviceDevice_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_ServiceDevices_serviceDevice_id",
                table: "Devices",
                column: "serviceDevice_id",
                principalTable: "ServiceDevices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_ServiceDevices_serviceDevice_id",
                table: "Services",
                column: "serviceDevice_id",
                principalTable: "ServiceDevices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
