using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OIDC.Core_Minimal.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationEntityCancelUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancelUrl",
                table: "Applications",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "AccessTokens",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "AccessTokens",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelUrl",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "AccessTokens");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "AccessTokens");
        }
    }
}
