using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TweakManagerBackend.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceIpWithHardwareId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AssignedToIpAddress",
                table: "LicenseKeys",
                newName: "AssignedToHardwareId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AssignedToHardwareId",
                table: "LicenseKeys",
                newName: "AssignedToIpAddress");
        }
    }
}
