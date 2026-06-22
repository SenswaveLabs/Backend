using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senswave.DataSources.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DroppedDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BrokerInfo_Url_Port_BrokerId",
                schema: "DataSources",
                table: "BrokerInfo");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "DataSources",
                table: "Brokers");

            migrationBuilder.CreateIndex(
                name: "IX_BrokerInfo_Url_Port",
                schema: "DataSources",
                table: "BrokerInfo",
                columns: new[] { "Url", "Port" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BrokerInfo_Url_Port",
                schema: "DataSources",
                table: "BrokerInfo");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "DataSources",
                table: "Brokers",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_BrokerInfo_Url_Port_BrokerId",
                schema: "DataSources",
                table: "BrokerInfo",
                columns: new[] { "Url", "Port", "BrokerId" },
                unique: true);
        }
    }
}
