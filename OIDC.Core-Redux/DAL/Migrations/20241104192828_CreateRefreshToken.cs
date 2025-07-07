using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OIDC.Core_Minimal.DAL.Migrations
{
    /// <inheritdoc />
    public partial class CreateRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessToken_Application_ApplicationId",
                table: "AccessToken");

            migrationBuilder.DropForeignKey(
                name: "FK_AccessToken_Users_UserId",
                table: "AccessToken");

            migrationBuilder.DropForeignKey(
                name: "FK_AccessTokenScope_AccessToken_AccessTokensId",
                table: "AccessTokenScope");

            migrationBuilder.DropForeignKey(
                name: "FK_AccessTokenScope_Scope_ScopesName",
                table: "AccessTokenScope");

            migrationBuilder.DropForeignKey(
                name: "FK_Application_Users_UserId",
                table: "Application");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUser_Application_AuthorisedApplicationsId",
                table: "ApplicationUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Scope",
                table: "Scope");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Application",
                table: "Application");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccessToken",
                table: "AccessToken");

            migrationBuilder.RenameTable(
                name: "Scope",
                newName: "Scopes");

            migrationBuilder.RenameTable(
                name: "Application",
                newName: "Applications");

            migrationBuilder.RenameTable(
                name: "AccessToken",
                newName: "AccessTokens");

            migrationBuilder.RenameIndex(
                name: "IX_Application_UserId",
                table: "Applications",
                newName: "IX_Applications_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AccessToken_UserId",
                table: "AccessTokens",
                newName: "IX_AccessTokens_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AccessToken_ApplicationId",
                table: "AccessTokens",
                newName: "IX_AccessTokens_ApplicationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Scopes",
                table: "Scopes",
                column: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Applications",
                table: "Applications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccessTokens",
                table: "AccessTokens",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Uses = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessTokens_Applications_ApplicationId",
                table: "AccessTokens",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccessTokens_Users_UserId",
                table: "AccessTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccessTokenScope_AccessTokens_AccessTokensId",
                table: "AccessTokenScope",
                column: "AccessTokensId",
                principalTable: "AccessTokens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccessTokenScope_Scopes_ScopesName",
                table: "AccessTokenScope",
                column: "ScopesName",
                principalTable: "Scopes",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Users_UserId",
                table: "Applications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUser_Applications_AuthorisedApplicationsId",
                table: "ApplicationUser",
                column: "AuthorisedApplicationsId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessTokens_Applications_ApplicationId",
                table: "AccessTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_AccessTokens_Users_UserId",
                table: "AccessTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_AccessTokenScope_AccessTokens_AccessTokensId",
                table: "AccessTokenScope");

            migrationBuilder.DropForeignKey(
                name: "FK_AccessTokenScope_Scopes_ScopesName",
                table: "AccessTokenScope");

            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Users_UserId",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUser_Applications_AuthorisedApplicationsId",
                table: "ApplicationUser");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Scopes",
                table: "Scopes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Applications",
                table: "Applications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccessTokens",
                table: "AccessTokens");

            migrationBuilder.RenameTable(
                name: "Scopes",
                newName: "Scope");

            migrationBuilder.RenameTable(
                name: "Applications",
                newName: "Application");

            migrationBuilder.RenameTable(
                name: "AccessTokens",
                newName: "AccessToken");

            migrationBuilder.RenameIndex(
                name: "IX_Applications_UserId",
                table: "Application",
                newName: "IX_Application_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AccessTokens_UserId",
                table: "AccessToken",
                newName: "IX_AccessToken_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AccessTokens_ApplicationId",
                table: "AccessToken",
                newName: "IX_AccessToken_ApplicationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Scope",
                table: "Scope",
                column: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Application",
                table: "Application",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccessToken",
                table: "AccessToken",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessToken_Application_ApplicationId",
                table: "AccessToken",
                column: "ApplicationId",
                principalTable: "Application",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccessToken_Users_UserId",
                table: "AccessToken",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccessTokenScope_AccessToken_AccessTokensId",
                table: "AccessTokenScope",
                column: "AccessTokensId",
                principalTable: "AccessToken",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccessTokenScope_Scope_ScopesName",
                table: "AccessTokenScope",
                column: "ScopesName",
                principalTable: "Scope",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Application_Users_UserId",
                table: "Application",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUser_Application_AuthorisedApplicationsId",
                table: "ApplicationUser",
                column: "AuthorisedApplicationsId",
                principalTable: "Application",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
