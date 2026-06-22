using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senswave.Users.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Language",
                schema: "Users",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Theme",
                schema: "Users",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                schema: "Users",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Theme",
                schema: "Users",
                table: "Users");
        }
    }
}
