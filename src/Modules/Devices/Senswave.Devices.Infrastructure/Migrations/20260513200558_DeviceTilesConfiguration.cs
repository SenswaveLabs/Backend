using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senswave.Devices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeviceTilesConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Configuration",
                schema: "Devices",
                table: "DeviceTiles",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Configuration",
                schema: "Devices",
                table: "DeviceTiles");
        }
    }
}
