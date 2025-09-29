using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExploraYa1.Migrations
{
    public partial class Created_destinoTuristico_Entity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppPaises",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppPaises", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AppRegiones",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Paisid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRegiones", x => x.id);
                    table.ForeignKey(
                        name: "FK_AppRegiones_AppPaises_Paisid",
                        column: x => x.Paisid,
                        principalTable: "AppPaises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppDestinos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    poblacion = table.Column<int>(type: "int", nullable: false),
                    latitud = table.Column<float>(type: "real", nullable: false),
                    longuitud = table.Column<float>(type: "real", nullable: false),
                    imagenUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    calificacionGeneral = table.Column<int>(type: "int", nullable: false),
                    Regionid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                        name: "FK_AppDestinos_AppRegiones_Regionid",
                        column: x => x.Regionid,
                        principalTable: "AppRegiones",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppDestinos_Regionid",
                table: "AppDestinos",
                column: "Regionid");

            migrationBuilder.CreateIndex(
                name: "IX_AppRegiones_Paisid",
                table: "AppRegiones",
                column: "Paisid");
        }

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

