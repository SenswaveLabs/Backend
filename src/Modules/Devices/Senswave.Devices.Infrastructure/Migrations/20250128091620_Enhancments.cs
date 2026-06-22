using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senswave.Devices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Enhancments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Widgets_Dashboards_DashboardId",
                schema: "Devices",
                table: "Widgets");

            migrationBuilder.DropTable(
                name: "DashboardBarLayout",
                schema: "Devices");

            migrationBuilder.DropTable(
                name: "DashboardLayout",
                schema: "Devices");

            migrationBuilder.DropTable(
                name: "WidgetLayout",
                schema: "Devices");

            migrationBuilder.DropTable(
                name: "WidgetPosition",
                schema: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Widgets_DashboardId",
                schema: "Devices",
                table: "Widgets");

            migrationBuilder.DropColumn(
                name: "DashboardId",
                schema: "Devices",
                table: "Widgets");

            migrationBuilder.DropColumn(
                name: "WidgetConfiguration",
                schema: "Devices",
                table: "Widgets");

            migrationBuilder.RenameColumn(
                name: "WidgetEnabled",
                schema: "Devices",
                table: "Widgets",
                newName: "Enabled");

            migrationBuilder.AddColumn<string>(
                name: "Configuration",
                schema: "Devices",
                table: "Widgets",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "Devices",
                table: "Widgets",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Configuration",
                schema: "Devices",
                table: "Dashboards",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                schema: "Devices",
                table: "Dashboards",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "Devices",
                table: "Dashboards",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "Devices",
                table: "Dashboards",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Configuration",
                schema: "Devices",
                table: "Widgets");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "Devices",
                table: "Widgets");

            migrationBuilder.DropColumn(
                name: "Configuration",
                schema: "Devices",
                table: "Dashboards");

            migrationBuilder.DropColumn(
                name: "Icon",
                schema: "Devices",
                table: "Dashboards");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "Devices",
                table: "Dashboards");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "Devices",
                table: "Dashboards");

            migrationBuilder.RenameColumn(
                name: "Enabled",
                schema: "Devices",
                table: "Widgets",
                newName: "WidgetEnabled");

            migrationBuilder.AddColumn<Guid>(
                name: "DashboardId",
                schema: "Devices",
                table: "Widgets",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "WidgetConfiguration",
                schema: "Devices",
                table: "Widgets",
                type: "character varying(4096)",
                maxLength: 4096,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DashboardBarLayout",
                schema: "Devices",
                columns: table => new
                {
                    DashboardId = table.Column<Guid>(type: "uuid", nullable: false),
                    Icon = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardBarLayout", x => x.DashboardId);
                    table.ForeignKey(
                        name: "FK_DashboardBarLayout_Dashboards_DashboardId",
                        column: x => x.DashboardId,
                        principalSchema: "Devices",
                        principalTable: "Dashboards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DashboardLayout",
                schema: "Devices",
                columns: table => new
                {
                    DashboardId = table.Column<Guid>(type: "uuid", nullable: false),
                    Columns = table.Column<int>(type: "integer", nullable: false),
                    Rows = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardLayout", x => x.DashboardId);
                    table.ForeignKey(
                        name: "FK_DashboardLayout_Dashboards_DashboardId",
                        column: x => x.DashboardId,
                        principalSchema: "Devices",
                        principalTable: "Dashboards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WidgetLayout",
                schema: "Devices",
                columns: table => new
                {
                    WidgetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Icon = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WidgetLayout", x => x.WidgetId);
                    table.ForeignKey(
                        name: "FK_WidgetLayout_Widgets_WidgetId",
                        column: x => x.WidgetId,
                        principalSchema: "Devices",
                        principalTable: "Widgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WidgetPosition",
                schema: "Devices",
                columns: table => new
                {
                    WidgetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    X = table.Column<int>(type: "integer", nullable: false),
                    Y = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WidgetPosition", x => x.WidgetId);
                    table.ForeignKey(
                        name: "FK_WidgetPosition_Widgets_WidgetId",
                        column: x => x.WidgetId,
                        principalSchema: "Devices",
                        principalTable: "Widgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Widgets_DashboardId",
                schema: "Devices",
                table: "Widgets",
                column: "DashboardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Widgets_Dashboards_DashboardId",
                schema: "Devices",
                table: "Widgets",
                column: "DashboardId",
                principalSchema: "Devices",
                principalTable: "Dashboards",
                principalColumn: "Id");
        }
    }
}
