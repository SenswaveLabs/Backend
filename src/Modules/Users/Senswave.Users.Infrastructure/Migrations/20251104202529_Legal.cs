using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senswave.Users.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Legal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrivacyPolicies",
                schema: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivacyPolicies", x => x.Id);
                    table.CheckConstraint("chk_pp_version_format", "\"Version\" ~ '^[0-9]+\\.[0-9]+\\.[0-9]+$'");
                });

            migrationBuilder.CreateTable(
                name: "TermsAndConditions",
                schema: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermsAndConditions", x => x.Id);
                    table.CheckConstraint("chk_tc_version_format", "\"Version\" ~ '^[0-9]+\\.[0-9]+\\.[0-9]+$'");
                });

            migrationBuilder.CreateTable(
                name: "UserConsents",
                schema: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TermsAndConditionsId = table.Column<Guid>(type: "uuid", nullable: false),
                    PrivacyPolicyId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserConsents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserConsents_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Users",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserConsents_PrivacyPolicies_PrivacyPolicyId",
                        column: x => x.PrivacyPolicyId,
                        principalSchema: "Users",
                        principalTable: "PrivacyPolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserConsents_TermsAndConditions_TermsAndConditionsId",
                        column: x => x.TermsAndConditionsId,
                        principalSchema: "Users",
                        principalTable: "TermsAndConditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "PrivacyPolicies_Version",
                schema: "Users",
                table: "PrivacyPolicies",
                column: "Version",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "TermsAndConditions_Version",
                schema: "Users",
                table: "TermsAndConditions",
                column: "Version",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserConsents_PrivacyPolicyId",
                schema: "Users",
                table: "UserConsents",
                column: "PrivacyPolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_UserConsents_TermsAndConditionsId",
                schema: "Users",
                table: "UserConsents",
                column: "TermsAndConditionsId");

            migrationBuilder.CreateIndex(
                name: "UserConsentTable_UserId_PrivacyPolicyId_TermsAndConditions",
                schema: "Users",
                table: "UserConsents",
                columns: new[] { "UserId", "PrivacyPolicyId", "TermsAndConditionsId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserConsents",
                schema: "Users");

            migrationBuilder.DropTable(
                name: "PrivacyPolicies",
                schema: "Users");

            migrationBuilder.DropTable(
                name: "TermsAndConditions",
                schema: "Users");
        }
    }
}
