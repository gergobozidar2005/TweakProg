using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TweakManagerBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddHardwareIdToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HardwareId",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HardwareId",
                table: "AspNetUsers");
        }
    }
}
