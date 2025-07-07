using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OIDC.Core_Minimal.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateApplicationEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Applications",
                newName: "HomepageUrl");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Applications",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "CallbackUrl",
                table: "Applications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "Applications",
                type: "character varying(24)",
                maxLength: 24,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClientSecret",
                table: "Applications",
                type: "character varying(88)",
                maxLength: 88,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AccessTokens",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AccessTokens",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CallbackUrl",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "ClientSecret",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AccessTokens");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AccessTokens");

            migrationBuilder.RenameColumn(
                name: "HomepageUrl",
                table: "Applications",
                newName: "Url");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Applications",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);
        }
    }
}
