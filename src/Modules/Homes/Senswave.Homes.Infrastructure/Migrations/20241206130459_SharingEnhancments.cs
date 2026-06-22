using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senswave.Homes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SharingEnhancments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rooms_HomeId",
                schema: "Homes",
                table: "Rooms");

            migrationBuilder.CreateIndex(
                name: "Room_HomeId_Name",
                schema: "Homes",
                table: "Rooms",
                columns: new[] { "HomeId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "Home_DataSourceId_Unique",
                schema: "Homes",
                table: "Homes",
                column: "DataSourceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "Home_OwnerId_Name",
                schema: "Homes",
                table: "Homes",
                columns: new[] { "OwnerId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "HomeSharingInvitation_HomeId_FriendId",
                schema: "Homes",
                table: "HomeSharingInvitation",
                columns: new[] { "HomeId", "FriendId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "HomeSharing_UserId_HomeId",
                schema: "Homes",
                table: "HomeSharing",
                columns: new[] { "UserId", "HomeId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "Room_HomeId_Name",
                schema: "Homes",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "Home_DataSourceId_Unique",
                schema: "Homes",
                table: "Homes");

            migrationBuilder.DropIndex(
                name: "Home_OwnerId_Name",
                schema: "Homes",
                table: "Homes");

            migrationBuilder.DropIndex(
                name: "HomeSharingInvitation_HomeId_FriendId",
                schema: "Homes",
                table: "HomeSharingInvitation");

            migrationBuilder.DropIndex(
                name: "HomeSharing_UserId_HomeId",
                schema: "Homes",
                table: "HomeSharing");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_HomeId",
                schema: "Homes",
                table: "Rooms",
                column: "HomeId");
        }
    }
}
