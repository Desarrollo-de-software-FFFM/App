using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExploraYa1.Migrations
{
    /// <inheritdoc />
    public partial class AddApiExternaLogsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppApiExternaLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NombreApi = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Endpoint = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Exitosa = table.Column<bool>(type: "bit", nullable: false),
                    CodigoHttp = table.Column<int>(type: "int", nullable: false),
                    TiempoMs = table.Column<double>(type: "float", nullable: false),
                    MensajeError = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppApiExternaLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppApiExternaLogs_FechaHora",
                table: "AppApiExternaLogs",
                column: "FechaHora");

            migrationBuilder.CreateIndex(
                name: "IX_AppApiExternaLogs_NombreApi",
                table: "AppApiExternaLogs",
                column: "NombreApi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppApiExternaLogs");
        }
    }
}
