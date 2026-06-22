using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senswave.DataSources.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImprovedDataSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Protocol",
                schema: "DataSources",
                table: "BrokerInfo");

            migrationBuilder.AddColumn<int>(
                name: "ProtocolVersion",
                schema: "DataSources",
                table: "BrokerInfo",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProtocolVersion",
                schema: "DataSources",
                table: "BrokerInfo");

            migrationBuilder.AddColumn<string>(
                name: "Protocol",
                schema: "DataSources",
                table: "BrokerInfo",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
