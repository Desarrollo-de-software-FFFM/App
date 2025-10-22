using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExploraYa1.Migrations
{
    /// <inheritdoc />
    public partial class Arreglando_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PaisId1",
                table: "AppRegiones",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RegionId1",
                table: "AppDestinos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppRegiones_PaisId1",
                table: "AppRegiones",
                column: "PaisId1");

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

            migrationBuilder.AddForeignKey(
                name: "FK_AppRegiones_AppPaises_PaisId1",
                table: "AppRegiones",
                column: "PaisId1",
                principalTable: "AppPaises",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppDestinos_AppRegiones_RegionId1",
                table: "AppDestinos");

            migrationBuilder.DropForeignKey(
                name: "FK_AppRegiones_AppPaises_PaisId1",
                table: "AppRegiones");

            migrationBuilder.DropIndex(
                name: "IX_AppRegiones_PaisId1",
                table: "AppRegiones");

            migrationBuilder.DropIndex(
                name: "IX_AppDestinos_RegionId1",
                table: "AppDestinos");

            migrationBuilder.DropColumn(
                name: "PaisId1",
                table: "AppRegiones");

            migrationBuilder.DropColumn(
                name: "RegionId1",
                table: "AppDestinos");
        }
    }
}
