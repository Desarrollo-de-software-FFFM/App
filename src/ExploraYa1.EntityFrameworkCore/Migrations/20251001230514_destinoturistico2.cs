using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExploraYa1.Migrations
{
    /// <inheritdoc />
    public partial class destinoturistico2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppRegiones_AppPaises_PaisId",
                table: "AppRegiones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppPaises",
                table: "AppPaises");

            migrationBuilder.RenameTable(
                name: "AppPaises",
                newName: "Pais");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pais",
                table: "Pais",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppRegiones_Pais_PaisId",
                table: "AppRegiones",
                column: "PaisId",
                principalTable: "Pais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppRegiones_Pais_PaisId",
                table: "AppRegiones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pais",
                table: "Pais");

            migrationBuilder.RenameTable(
                name: "Pais",
                newName: "AppPaises");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppPaises",
                table: "AppPaises",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppRegiones_AppPaises_PaisId",
                table: "AppRegiones",
                column: "PaisId",
                principalTable: "AppPaises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
