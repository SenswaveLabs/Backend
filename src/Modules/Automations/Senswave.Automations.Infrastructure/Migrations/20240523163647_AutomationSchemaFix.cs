using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senswave.Automations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AutomationSchemaFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Automations");

            migrationBuilder.RenameTable(
                name: "Automations",
                newName: "Automations",
                newSchema: "Automations");

            migrationBuilder.RenameTable(
                name: "AutomationResult",
                newName: "AutomationResult",
                newSchema: "Automations");

            migrationBuilder.RenameTable(
                name: "AutomationCondition",
                newName: "AutomationCondition",
                newSchema: "Automations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Automations",
                schema: "Automations",
                newName: "Automations");

            migrationBuilder.RenameTable(
                name: "AutomationResult",
                schema: "Automations",
                newName: "AutomationResult");

            migrationBuilder.RenameTable(
                name: "AutomationCondition",
                schema: "Automations",
                newName: "AutomationCondition");
        }
    }
}
