using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExploraYa1.Migrations
{
    /// <inheritdoc />
    public partial class AgregadasEntidadesFeature25 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppNotificaciones_FechaHora",
                table: "AppNotificaciones");

            migrationBuilder.DropIndex(
                name: "IX_AppNotificaciones_NombreApi",
                table: "AppNotificaciones");

            migrationBuilder.DropColumn(
                name: "CodigoHttp",
                table: "AppNotificaciones");

            migrationBuilder.DropColumn(
                name: "Endpoint",
                table: "AppNotificaciones");

            migrationBuilder.DropColumn(
                name: "Exitosa",
                table: "AppNotificaciones");

            migrationBuilder.DropColumn(
                name: "FechaHora",
                table: "AppNotificaciones");

            migrationBuilder.DropColumn(
                name: "MensajeError",
                table: "AppNotificaciones");

            migrationBuilder.DropColumn(
                name: "NombreApi",
                table: "AppNotificaciones");

            migrationBuilder.DropColumn(
                name: "TiempoMs",
                table: "AppNotificaciones");

            migrationBuilder.RenameTable(
                name: "ApiExternaLog",
                newName: "AppApiExternaLogs");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "AppApiExternaLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "CodigoHttp",
                table: "AppApiExternaLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Endpoint",
                table: "AppApiExternaLogs",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Exitosa",
                table: "AppApiExternaLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaHora",
                table: "AppApiExternaLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "MensajeError",
                table: "AppApiExternaLogs",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NombreApi",
                table: "AppApiExternaLogs",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "TiempoMs",
                table: "AppApiExternaLogs",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppApiExternaLogs",
                table: "AppApiExternaLogs",
                column: "Id");

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
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppApiExternaLogs",
                table: "AppApiExternaLogs");

            migrationBuilder.DropIndex(
                name: "IX_AppApiExternaLogs_FechaHora",
                table: "AppApiExternaLogs");

            migrationBuilder.DropIndex(
                name: "IX_AppApiExternaLogs_NombreApi",
                table: "AppApiExternaLogs");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AppApiExternaLogs");

            migrationBuilder.DropColumn(
                name: "CodigoHttp",
                table: "AppApiExternaLogs");

            migrationBuilder.DropColumn(
                name: "Endpoint",
                table: "AppApiExternaLogs");

            migrationBuilder.DropColumn(
                name: "Exitosa",
                table: "AppApiExternaLogs");

            migrationBuilder.DropColumn(
                name: "FechaHora",
                table: "AppApiExternaLogs");

            migrationBuilder.DropColumn(
                name: "MensajeError",
                table: "AppApiExternaLogs");

            migrationBuilder.DropColumn(
                name: "NombreApi",
                table: "AppApiExternaLogs");

            migrationBuilder.DropColumn(
                name: "TiempoMs",
                table: "AppApiExternaLogs");

            migrationBuilder.RenameTable(
                name: "AppApiExternaLogs",
                newName: "ApiExternaLog");

            migrationBuilder.AddColumn<int>(
                name: "CodigoHttp",
                table: "AppNotificaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Endpoint",
                table: "AppNotificaciones",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Exitosa",
                table: "AppNotificaciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaHora",
                table: "AppNotificaciones",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "MensajeError",
                table: "AppNotificaciones",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NombreApi",
                table: "AppNotificaciones",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "TiempoMs",
                table: "AppNotificaciones",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_AppNotificaciones_FechaHora",
                table: "AppNotificaciones",
                column: "FechaHora");

            migrationBuilder.CreateIndex(
                name: "IX_AppNotificaciones_NombreApi",
                table: "AppNotificaciones",
                column: "NombreApi");
        }
    }
}
