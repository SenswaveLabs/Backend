using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senswave.Devices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HomeReferenceEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HomeOwnerId",
                schema: "Devices",
                table: "Devices");

            migrationBuilder.CreateTable(
                name: "HomeReferences",
                schema: "Devices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    HomeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeReferences", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_HomeReferenceId",
                schema: "Devices",
                table: "Devices",
                column: "HomeReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeReferences_HomeId_OwnerId",
                schema: "Devices",
                table: "HomeReferences",
                columns: new[] { "HomeId", "OwnerId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_HomeReferences_HomeReferenceId",
                schema: "Devices",
                table: "Devices",
                column: "HomeReferenceId",
                principalSchema: "Devices",
                principalTable: "HomeReferences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_HomeReferences_HomeReferenceId",
                schema: "Devices",
                table: "Devices");

            migrationBuilder.DropTable(
                name: "HomeReferences",
                schema: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_HomeReferenceId",
                schema: "Devices",
                table: "Devices");

            migrationBuilder.AddColumn<Guid>(
                name: "HomeOwnerId",
                schema: "Devices",
                table: "Devices",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
