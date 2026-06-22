using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senswave.Homes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LinkHomeWithInvitation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_HomeSharingInvitation_Homes_HomeId",
                schema: "Homes",
                table: "HomeSharingInvitation",
                column: "HomeId",
                principalSchema: "Homes",
                principalTable: "Homes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HomeSharingInvitation_Homes_HomeId",
                schema: "Homes",
                table: "HomeSharingInvitation");
        }
    }
}
