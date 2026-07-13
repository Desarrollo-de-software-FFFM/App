using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExploraYa1.Migrations
{
    /// <inheritdoc />
    public partial class AddGeoDbId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GeoDbId",
                table: "AppDestinos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeoDbId",
                table: "AppDestinos");
        }
    }
}
