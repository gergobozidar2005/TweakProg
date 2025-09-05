using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TweakManagerBackend.Migrations
{
    /// <inheritdoc />
    public partial class LinkKeyToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "LicenseKeys",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseKeys_ApplicationUserId",
                table: "LicenseKeys",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LicenseKeys_AspNetUsers_ApplicationUserId",
                table: "LicenseKeys",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LicenseKeys_AspNetUsers_ApplicationUserId",
                table: "LicenseKeys");

            migrationBuilder.DropIndex(
                name: "IX_LicenseKeys_ApplicationUserId",
                table: "LicenseKeys");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "LicenseKeys");
        }
    }
}
