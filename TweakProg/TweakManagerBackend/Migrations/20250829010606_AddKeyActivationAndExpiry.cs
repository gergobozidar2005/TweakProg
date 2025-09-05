using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TweakManagerBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddKeyActivationAndExpiry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "LicenseKeys",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "LicenseKeys",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "LicenseKeys");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "LicenseKeys");
        }
    }
}
