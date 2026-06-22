using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System.Text.Json;

#nullable disable

namespace Senswave.Automations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SplitAutomationsToThree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutomationCondition",
                schema: "Automations");

            migrationBuilder.DropTable(
                name: "AutomationResult",
                schema: "Automations");

            migrationBuilder.CreateTable(
                name: "AutomationConditions",
                schema: "Automations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AutomationId = table.Column<Guid>(type: "uuid", nullable: false),
                    OperationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConditionType = table.Column<int>(type: "integer", nullable: false),
                    ConditionConfiguration = table.Column<string>(type: "text", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutomationConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AutomationConditions_Automations_AutomationId",
                        column: x => x.AutomationId,
                        principalSchema: "Automations",
                        principalTable: "Automations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AutomationResults",
                schema: "Automations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AutomationId = table.Column<Guid>(type: "uuid", nullable: false),
                    OperationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ValueToSend = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutomationResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AutomationResults_Automations_AutomationId",
                        column: x => x.AutomationId,
                        principalSchema: "Automations",
                        principalTable: "Automations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AutomationConditions_AutomationId",
                schema: "Automations",
                table: "AutomationConditions",
                column: "AutomationId");

            migrationBuilder.CreateIndex(
                name: "IX_AutomationResults_AutomationId",
                schema: "Automations",
                table: "AutomationResults",
                column: "AutomationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutomationConditions",
                schema: "Automations");

            migrationBuilder.DropTable(
                name: "AutomationResults",
                schema: "Automations");

            migrationBuilder.CreateTable(
                name: "AutomationCondition",
                schema: "Automations",
                columns: table => new
                {
                    AutomationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConditionConfiguration = table.Column<JsonDocument>(type: "jsonb", nullable: false),
                    ConditionType = table.Column<int>(type: "integer", nullable: false),
                    OperationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutomationCondition", x => new { x.AutomationId, x.Id });
                    table.ForeignKey(
                        name: "FK_AutomationCondition_Automations_AutomationId",
                        column: x => x.AutomationId,
                        principalSchema: "Automations",
                        principalTable: "Automations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AutomationResult",
                schema: "Automations",
                columns: table => new
                {
                    AutomationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OperationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResultType = table.Column<int>(type: "integer", nullable: false),
                    ValueToSend = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutomationResult", x => new { x.AutomationId, x.Id });
                    table.ForeignKey(
                        name: "FK_AutomationResult_Automations_AutomationId",
                        column: x => x.AutomationId,
                        principalSchema: "Automations",
                        principalTable: "Automations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
