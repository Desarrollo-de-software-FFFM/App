using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExploraYa1.Migrations
{
    /// <inheritdoc />
    public partial class Destinosconpaises24 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RegionId1",
                table: "AppDestinos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppDestinos_RegionId1",
                table: "AppDestinos",
                column: "RegionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AppDestinos_AppRegiones_RegionId1",
                table: "AppDestinos",
                column: "RegionId1",
                principalTable: "AppRegiones",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppDestinos_AppRegiones_RegionId1",
                table: "AppDestinos");

            migrationBuilder.DropIndex(
                name: "IX_AppDestinos_RegionId1",
                table: "AppDestinos");

            migrationBuilder.DropColumn(
                name: "RegionId1",
                table: "AppDestinos");
        }
    }
}
