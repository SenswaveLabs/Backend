using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senswave.Homes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SharingExpiration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationTimeUtc",
                schema: "Homes",
                table: "HomeSharingInvitation",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationTimeUtc",
                schema: "Homes",
                table: "HomeSharingInvitation");
        }
    }
}
