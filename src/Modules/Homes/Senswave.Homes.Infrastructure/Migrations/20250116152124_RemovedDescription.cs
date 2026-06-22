using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senswave.Homes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                schema: "Homes",
                table: "Homes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "Homes",
                table: "Homes",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");
        }
    }
}
