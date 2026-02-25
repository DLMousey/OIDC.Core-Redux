using System;
using System.Net;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OIDC.Core_Minimal.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthenticationEventTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthenticationEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Success = table.Column<bool>(type: "boolean", nullable: false),
                    IpAddress = table.Column<IPAddress>(type: "inet", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthenticationEvents_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });
            
            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationEvents_UserId",
                table: "AuthenticationEvents",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthenticationEvents");
        }
    }
}
