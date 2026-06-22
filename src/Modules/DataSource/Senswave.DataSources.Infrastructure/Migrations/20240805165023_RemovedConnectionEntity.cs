using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senswave.DataSources.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedConnectionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Connections_ConnectionId",
                schema: "DataSources",
                table: "Sessions");

            migrationBuilder.DropTable(
                name: "Connections",
                schema: "DataSources");

            migrationBuilder.RenameColumn(
                name: "ConnectionId",
                schema: "DataSources",
                table: "Sessions",
                newName: "BrokerId");

            migrationBuilder.RenameIndex(
                name: "IX_Sessions_ConnectionId",
                schema: "DataSources",
                table: "Sessions",
                newName: "IX_Sessions_BrokerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Brokers_BrokerId",
                schema: "DataSources",
                table: "Sessions",
                column: "BrokerId",
                principalSchema: "DataSources",
                principalTable: "Brokers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Brokers_BrokerId",
                schema: "DataSources",
                table: "Sessions");

            migrationBuilder.RenameColumn(
                name: "BrokerId",
                schema: "DataSources",
                table: "Sessions",
                newName: "ConnectionId");

            migrationBuilder.RenameIndex(
                name: "IX_Sessions_BrokerId",
                schema: "DataSources",
                table: "Sessions",
                newName: "IX_Sessions_ConnectionId");

            migrationBuilder.CreateTable(
                name: "Connections",
                schema: "DataSources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BrokerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Connections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Connections_Brokers_BrokerId",
                        column: x => x.BrokerId,
                        principalSchema: "DataSources",
                        principalTable: "Brokers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Connections_BrokerId",
                schema: "DataSources",
                table: "Connections",
                column: "BrokerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Connections_ConnectionId",
                schema: "DataSources",
                table: "Sessions",
                column: "ConnectionId",
                principalSchema: "DataSources",
                principalTable: "Connections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
