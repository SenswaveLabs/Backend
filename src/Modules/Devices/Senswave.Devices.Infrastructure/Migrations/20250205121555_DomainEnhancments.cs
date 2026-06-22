using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senswave.Devices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DomainEnhancments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Widgets_OperationId",
                schema: "Devices",
                table: "Widgets");

            migrationBuilder.DropIndex(
                name: "IX_Dashboards_DeviceId",
                schema: "Devices",
                table: "Dashboards");

            migrationBuilder.CreateIndex(
                name: "IX_Widgets_OperationId_Name",
                schema: "Devices",
                table: "Widgets",
                columns: new[] { "OperationId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dashboards_DeviceId_Name",
                schema: "Devices",
                table: "Dashboards",
                columns: new[] { "DeviceId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Widgets_OperationId_Name",
                schema: "Devices",
                table: "Widgets");

            migrationBuilder.DropIndex(
                name: "IX_Dashboards_DeviceId_Name",
                schema: "Devices",
                table: "Dashboards");

            migrationBuilder.CreateIndex(
                name: "IX_Widgets_OperationId",
                schema: "Devices",
                table: "Widgets",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_Dashboards_DeviceId",
                schema: "Devices",
                table: "Dashboards",
                column: "DeviceId");
        }
    }
}
