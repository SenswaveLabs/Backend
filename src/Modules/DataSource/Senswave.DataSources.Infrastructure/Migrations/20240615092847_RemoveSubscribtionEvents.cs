using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Senswave.DataSources.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSubscribtionEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrokerCredentials",
                schema: "DataSources");

            migrationBuilder.DropTable(
                name: "SubscribtionEvents",
                schema: "DataSources");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BrokerCredentials",
                schema: "DataSources",
                columns: table => new
                {
                    BrokerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrokerCredentials", x => x.BrokerId);
                    table.ForeignKey(
                        name: "FK_BrokerCredentials_Brokers_BrokerId",
                        column: x => x.BrokerId,
                        principalSchema: "DataSources",
                        principalTable: "Brokers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscribtionEvents",
                schema: "DataSources",
                columns: table => new
                {
                    SubscribtionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Payload = table.Column<string>(type: "text", nullable: false),
                    ReceivedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscribtionEvents", x => new { x.SubscribtionId, x.Id });
                    table.ForeignKey(
                        name: "FK_SubscribtionEvents_Subscribtions_SubscribtionId",
                        column: x => x.SubscribtionId,
                        principalSchema: "DataSources",
                        principalTable: "Subscribtions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
