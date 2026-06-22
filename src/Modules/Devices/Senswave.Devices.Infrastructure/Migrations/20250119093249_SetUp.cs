using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Senswave.Devices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SetUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Devices");

            migrationBuilder.CreateTable(
                name: "Devices",
                schema: "Devices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HomeOwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    HomeReferenceId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomReferenceId = table.Column<Guid>(type: "uuid", nullable: true),
                    Icon = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dashboards",
                schema: "Devices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dashboards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dashboards_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalSchema: "Devices",
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataReferences",
                schema: "Devices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataSourceDataReferenceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataReferences_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalSchema: "Devices",
                        principalTable: "Devices",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DeviceSharing",
                schema: "Devices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    SharingType = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceSharing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceSharing_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalSchema: "Devices",
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                    Rows = table.Column<int>(type: "integer", nullable: false),
                    Columns = table.Column<int>(type: "integer", nullable: false)
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
                name: "Operations",
                schema: "Devices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataReferenceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Configuration = table.Column<string>(type: "jsonb", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Operations_DataReferences_DataReferenceId",
                        column: x => x.DataReferenceId,
                        principalSchema: "Devices",
                        principalTable: "DataReferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Operations_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalSchema: "Devices",
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeviceTiles",
                schema: "Devices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    OperationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Configuration = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceTiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceTiles_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalSchema: "Devices",
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeviceTiles_Operations_OperationId",
                        column: x => x.OperationId,
                        principalSchema: "Devices",
                        principalTable: "Operations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperationValues",
                schema: "Devices",
                columns: table => new
                {
                    OperationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<string>(type: "jsonb", nullable: false),
                    ProcessedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationValues", x => new { x.OperationId, x.Id });
                    table.ForeignKey(
                        name: "FK_OperationValues_Operations_OperationId",
                        column: x => x.OperationId,
                        principalSchema: "Devices",
                        principalTable: "Operations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Widgets",
                schema: "Devices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DashboardId = table.Column<Guid>(type: "uuid", nullable: false),
                    WidgetEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    WidgetConfiguration = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    OperationId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Widgets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Widgets_Dashboards_DashboardId",
                        column: x => x.DashboardId,
                        principalSchema: "Devices",
                        principalTable: "Dashboards",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Widgets_Operations_OperationId",
                        column: x => x.OperationId,
                        principalSchema: "Devices",
                        principalTable: "Operations",
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
                    X = table.Column<int>(type: "integer", nullable: false),
                    Y = table.Column<int>(type: "integer", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false)
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
                name: "IX_Dashboards_DeviceId",
                schema: "Devices",
                table: "Dashboards",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_DataReferences_DataSourceDataReferenceId",
                schema: "Devices",
                table: "DataReferences",
                column: "DataSourceDataReferenceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DataReferences_DeviceId",
                schema: "Devices",
                table: "DataReferences",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceSharing_DeviceId",
                schema: "Devices",
                table: "DeviceSharing",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceSharing_UserId_DeviceId",
                schema: "Devices",
                table: "DeviceSharing",
                columns: new[] { "UserId", "DeviceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceTiles_DeviceId",
                schema: "Devices",
                table: "DeviceTiles",
                column: "DeviceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceTiles_OperationId",
                schema: "Devices",
                table: "DeviceTiles",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_DataReferenceId",
                schema: "Devices",
                table: "Operations",
                column: "DataReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_DeviceId",
                schema: "Devices",
                table: "Operations",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Widgets_DashboardId",
                schema: "Devices",
                table: "Widgets",
                column: "DashboardId");

            migrationBuilder.CreateIndex(
                name: "IX_Widgets_OperationId",
                schema: "Devices",
                table: "Widgets",
                column: "OperationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DashboardBarLayout",
                schema: "Devices");

            migrationBuilder.DropTable(
                name: "DashboardLayout",
                schema: "Devices");

            migrationBuilder.DropTable(
                name: "DeviceSharing",
                schema: "Devices");

            migrationBuilder.DropTable(
                name: "DeviceTiles",
                schema: "Devices");

            migrationBuilder.DropTable(
                name: "OperationValues",
                schema: "Devices");

            migrationBuilder.DropTable(
                name: "WidgetLayout",
                schema: "Devices");

            migrationBuilder.DropTable(
                name: "WidgetPosition",
                schema: "Devices");

            migrationBuilder.DropTable(
                name: "Widgets",
                schema: "Devices");

            migrationBuilder.DropTable(
                name: "Dashboards",
                schema: "Devices");

            migrationBuilder.DropTable(
                name: "Operations",
                schema: "Devices");

            migrationBuilder.DropTable(
                name: "DataReferences",
                schema: "Devices");

            migrationBuilder.DropTable(
                name: "Devices",
                schema: "Devices");
        }
    }
}
