using Microsoft.EntityFrameworkCore.Migrations;
using System.Text.Json;

#nullable disable

namespace Senswave.Automations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AutomationResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResultConfiguration",
                schema: "Automations",
                table: "AutomationResult");

            migrationBuilder.AddColumn<Guid>(
                name: "OperationId",
                schema: "Automations",
                table: "AutomationResult",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ValueToSend",
                schema: "Automations",
                table: "AutomationResult",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "OperationId",
                schema: "Automations",
                table: "AutomationCondition",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OperationId",
                schema: "Automations",
                table: "AutomationResult");

            migrationBuilder.DropColumn(
                name: "ValueToSend",
                schema: "Automations",
                table: "AutomationResult");

            migrationBuilder.DropColumn(
                name: "OperationId",
                schema: "Automations",
                table: "AutomationCondition");

            migrationBuilder.AddColumn<JsonDocument>(
                name: "ResultConfiguration",
                schema: "Automations",
                table: "AutomationResult",
                type: "jsonb",
                nullable: false);
        }
    }
}
