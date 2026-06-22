using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senswave.Homes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HomesFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataSourceReference_Homes_HomeId",
                schema: "Homes",
                table: "DataSourceReference");

            migrationBuilder.AddForeignKey(
                name: "FK_DataSourceReference_Homes_HomeId",
                schema: "Homes",
                table: "DataSourceReference",
                column: "HomeId",
                principalSchema: "Homes",
                principalTable: "Homes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataSourceReference_Homes_HomeId",
                schema: "Homes",
                table: "DataSourceReference");

            migrationBuilder.AddForeignKey(
                name: "FK_DataSourceReference_Homes_HomeId",
                schema: "Homes",
                table: "DataSourceReference",
                column: "HomeId",
                principalSchema: "Homes",
                principalTable: "Homes",
                principalColumn: "Id");
        }
    }
}
