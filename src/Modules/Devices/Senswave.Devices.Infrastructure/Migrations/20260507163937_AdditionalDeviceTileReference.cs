using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senswave.Devices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalDeviceTileReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DisplayableOperationId",
                schema: "Devices",
                table: "DeviceTiles",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceTiles_DisplayableOperationId",
                schema: "Devices",
                table: "DeviceTiles",
                column: "DisplayableOperationId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceTiles_Operations_DisplayableOperationId",
                schema: "Devices",
                table: "DeviceTiles",
                column: "DisplayableOperationId",
                principalSchema: "Devices",
                principalTable: "Operations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeviceTiles_Operations_DisplayableOperationId",
                schema: "Devices",
                table: "DeviceTiles");

            migrationBuilder.DropIndex(
                name: "IX_DeviceTiles_DisplayableOperationId",
                schema: "Devices",
                table: "DeviceTiles");

            migrationBuilder.DropColumn(
                name: "DisplayableOperationId",
                schema: "Devices",
                table: "DeviceTiles");
        }
    }
}
