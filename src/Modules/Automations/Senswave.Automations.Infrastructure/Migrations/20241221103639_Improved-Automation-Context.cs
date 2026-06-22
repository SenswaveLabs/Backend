using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senswave.Automations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImprovedAutomationContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "View_Description",
                schema: "Automations",
                table: "Automations");

            migrationBuilder.RenameColumn(
                name: "View_Name",
                schema: "Automations",
                table: "Automations",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "View_Icon",
                schema: "Automations",
                table: "Automations",
                newName: "Icon");

            migrationBuilder.CreateIndex(
                name: "UX_Home_AutomationName",
                schema: "Automations",
                table: "Automations",
                columns: new[] { "Name", "HomesReferenceId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Home_AutomationName",
                schema: "Automations",
                table: "Automations");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "Automations",
                table: "Automations",
                newName: "View_Name");

            migrationBuilder.RenameColumn(
                name: "Icon",
                schema: "Automations",
                table: "Automations",
                newName: "View_Icon");

            migrationBuilder.AddColumn<string>(
                name: "View_Description",
                schema: "Automations",
                table: "Automations",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");
        }
    }
}
