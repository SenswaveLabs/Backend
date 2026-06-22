using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senswave.Homes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HomesReferenceForDataSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "Home_DataSourceId_Unique",
                schema: "Homes",
                table: "Homes");

            migrationBuilder.DropColumn(
                name: "DataSourceId",
                schema: "Homes",
                table: "Homes");

            migrationBuilder.CreateTable(
                name: "DataSourceReference",
                schema: "Homes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HomeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataSourceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSourceReference", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataSourceReference_Homes_HomeId",
                        column: x => x.HomeId,
                        principalSchema: "Homes",
                        principalTable: "Homes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataSourceReference_DataSourceId",
                schema: "Homes",
                table: "DataSourceReference",
                column: "DataSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_DataSourceReference_HomeId",
                schema: "Homes",
                table: "DataSourceReference",
                column: "HomeId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataSourceReference",
                schema: "Homes");

            migrationBuilder.AddColumn<Guid>(
                name: "DataSourceId",
                schema: "Homes",
                table: "Homes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "Home_DataSourceId_Unique",
                schema: "Homes",
                table: "Homes",
                column: "DataSourceId",
                unique: true);
        }
    }
}
