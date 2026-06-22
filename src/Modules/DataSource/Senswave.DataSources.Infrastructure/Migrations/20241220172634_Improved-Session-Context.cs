using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senswave.DataSources.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImprovedSessionContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BrokerInfo_Url_Port",
                schema: "DataSources",
                table: "BrokerInfo");

            migrationBuilder.CreateIndex(
                name: "IX_Brokers_OwnerId_Name",
                schema: "DataSources",
                table: "Brokers",
                columns: new[] { "OwnerId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BrokerInfo_Url_Port_BrokerId",
                schema: "DataSources",
                table: "BrokerInfo",
                columns: new[] { "Url", "Port", "BrokerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BrokerInfo_Url_Port_ClientName",
                schema: "DataSources",
                table: "BrokerInfo",
                columns: new[] { "Url", "Port", "ClientName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Brokers_OwnerId_Name",
                schema: "DataSources",
                table: "Brokers");

            migrationBuilder.DropIndex(
                name: "IX_BrokerInfo_Url_Port_BrokerId",
                schema: "DataSources",
                table: "BrokerInfo");

            migrationBuilder.DropIndex(
                name: "IX_BrokerInfo_Url_Port_ClientName",
                schema: "DataSources",
                table: "BrokerInfo");

            migrationBuilder.CreateIndex(
                name: "IX_BrokerInfo_Url_Port",
                schema: "DataSources",
                table: "BrokerInfo",
                columns: new[] { "Url", "Port" },
                unique: true);
        }
    }
}
