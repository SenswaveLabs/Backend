using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senswave.DataSources.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImprovedContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subscribtions_BrokerId",
                schema: "DataSources",
                table: "Subscribtions");

            migrationBuilder.CreateIndex(
                name: "IX_Subscribtions_BrokerId_Topic",
                schema: "DataSources",
                table: "Subscribtions",
                columns: new[] { "BrokerId", "Topic" },
                unique: true);

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
                name: "IX_Subscribtions_BrokerId_Topic",
                schema: "DataSources",
                table: "Subscribtions");

            migrationBuilder.DropIndex(
                name: "IX_BrokerInfo_Url_Port",
                schema: "DataSources",
                table: "BrokerInfo");

            migrationBuilder.CreateIndex(
                name: "IX_Subscribtions_BrokerId",
                schema: "DataSources",
                table: "Subscribtions",
                column: "BrokerId");
        }
    }
}
