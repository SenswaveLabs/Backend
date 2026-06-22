using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senswave.Devices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DevicePresence2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PresenceId",
                schema: "Devices",
                table: "Devices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PresenceId",
                schema: "Devices",
                table: "Devices",
                type: "uuid",
                nullable: true);
        }
    }
}
