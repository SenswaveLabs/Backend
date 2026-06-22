using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senswave.Devices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenamingInDeviceTile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeviceTiles_Operations_OperationId",
                schema: "Devices",
                table: "DeviceTiles");

            migrationBuilder.RenameColumn(
                name: "OperationId",
                schema: "Devices",
                table: "DeviceTiles",
                newName: "SwitchOperationId");

            migrationBuilder.RenameIndex(
                name: "IX_DeviceTiles_OperationId",
                schema: "Devices",
                table: "DeviceTiles",
                newName: "IX_DeviceTiles_SwitchOperationId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceTiles_Operations_SwitchOperationId",
                schema: "Devices",
                table: "DeviceTiles",
                column: "SwitchOperationId",
                principalSchema: "Devices",
                principalTable: "Operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeviceTiles_Operations_SwitchOperationId",
                schema: "Devices",
                table: "DeviceTiles");

            migrationBuilder.RenameColumn(
                name: "SwitchOperationId",
                schema: "Devices",
                table: "DeviceTiles",
                newName: "OperationId");

            migrationBuilder.RenameIndex(
                name: "IX_DeviceTiles_SwitchOperationId",
                schema: "Devices",
                table: "DeviceTiles",
                newName: "IX_DeviceTiles_OperationId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceTiles_Operations_OperationId",
                schema: "Devices",
                table: "DeviceTiles",
                column: "OperationId",
                principalSchema: "Devices",
                principalTable: "Operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
