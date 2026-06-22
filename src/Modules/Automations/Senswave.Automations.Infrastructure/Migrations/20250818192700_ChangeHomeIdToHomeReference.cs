using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senswave.Automations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeHomeIdToHomeReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HomesReferenceId",
                schema: "Automations",
                table: "Automations",
                newName: "HomeReferenceId");

            migrationBuilder.CreateTable(
                name: "HomeReferences",
                schema: "Automations",
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
                name: "IX_Automations_HomeReferenceId",
                schema: "Automations",
                table: "Automations",
                column: "HomeReferenceId");

            migrationBuilder.CreateIndex(
                name: "UX_HomeReference_OwnerId_HomeId",
                schema: "Automations",
                table: "HomeReferences",
                columns: new[] { "OwnerId", "HomeId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Automations_HomeReferences_HomeReferenceId",
                schema: "Automations",
                table: "Automations",
                column: "HomeReferenceId",
                principalSchema: "Automations",
                principalTable: "HomeReferences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Automations_HomeReferences_HomeReferenceId",
                schema: "Automations",
                table: "Automations");

            migrationBuilder.DropTable(
                name: "HomeReferences",
                schema: "Automations");

            migrationBuilder.DropIndex(
                name: "IX_Automations_HomeReferenceId",
                schema: "Automations",
                table: "Automations");

            migrationBuilder.RenameColumn(
                name: "HomeReferenceId",
                schema: "Automations",
                table: "Automations",
                newName: "HomesReferenceId");
        }
    }
}
