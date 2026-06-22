using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senswave.Homes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HomesReferenceForDataSource2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DataSourceReference_DataSourceId",
                schema: "Homes",
                table: "DataSourceReference");

            migrationBuilder.CreateIndex(
                name: "IX_DataSourceReference_DataSourceId",
                schema: "Homes",
                table: "DataSourceReference",
                column: "DataSourceId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DataSourceReference_DataSourceId",
                schema: "Homes",
                table: "DataSourceReference");

            migrationBuilder.CreateIndex(
                name: "IX_DataSourceReference_DataSourceId",
                schema: "Homes",
                table: "DataSourceReference",
                column: "DataSourceId");
        }
    }
}
