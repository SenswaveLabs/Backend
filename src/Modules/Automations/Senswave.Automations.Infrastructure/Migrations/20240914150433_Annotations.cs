using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Senswave.Automations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Annotations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.RenameColumn(
                name: "Description",
                schema: "Automations",
                table: "Automations",
                newName: "View_Description");

            migrationBuilder.RenameColumn(
                name: "HomeReferenceId",
                schema: "Automations",
                table: "Automations",
                newName: "HomesReferenceId");

            migrationBuilder.AlterColumn<string>(
                name: "View_Name",
                schema: "Automations",
                table: "Automations",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "View_Icon",
                schema: "Automations",
                table: "Automations",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "View_Description",
                schema: "Automations",
                table: "Automations",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.RenameColumn(
                name: "View_Description",
                schema: "Automations",
                table: "Automations",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "HomesReferenceId",
                schema: "Automations",
                table: "Automations",
                newName: "HomeReferenceId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Automations",
                table: "Automations",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                schema: "Automations",
                table: "Automations",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "Automations",
                table: "Automations",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(512)",
                oldMaxLength: 512);
        }
    }
}
