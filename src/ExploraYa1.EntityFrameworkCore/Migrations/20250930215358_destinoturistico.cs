using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExploraYa1.Migrations
{
    /// <inheritdoc />
    public partial class Destinoturistico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppPaises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppPaises", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppRegiones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    PaisId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdPais = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRegiones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppRegiones_AppPaises_PaisId",
                        column: x => x.PaisId,
                        principalTable: "AppPaises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppDestinos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Poblacion = table.Column<int>(type: "int", nullable: false),
                    Latitud = table.Column<float>(type: "real", nullable: false),
                    Longuitud = table.Column<float>(type: "real", nullable: false),
                    ImagenUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CalificacionGeneral = table.Column<int>(type: "int", nullable: false),
                    IdRegion = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppDestinos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppDestinos_AppRegiones_RegionId",
                        column: x => x.RegionId,
                        principalTable: "AppRegiones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppDestinos_RegionId",
                table: "AppDestinos",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_AppRegiones_PaisId",
                table: "AppRegiones",
                column: "PaisId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppDestinos");

            migrationBuilder.DropTable(
                name: "AppRegiones");

            migrationBuilder.DropTable(
                name: "AppPaises");
        }
    }
}
