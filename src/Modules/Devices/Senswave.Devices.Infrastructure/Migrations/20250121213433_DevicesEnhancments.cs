using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senswave.Devices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DevicesEnhancments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Configuration",
                schema: "Devices",
                table: "DeviceTiles");

            migrationBuilder.RenameColumn(
                name: "Value",
                schema: "Devices",
                table: "OperationValues",
                newName: "InternalValue");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InternalValue",
                schema: "Devices",
                table: "OperationValues",
                newName: "Value");

            migrationBuilder.AddColumn<string>(
                name: "Configuration",
                schema: "Devices",
                table: "DeviceTiles",
                type: "character varying(4096)",
                maxLength: 4096,
                nullable: false,
                defaultValue: "");
        }
    }
}
