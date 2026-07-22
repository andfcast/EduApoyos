using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduApoyosBackend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarSolicitud : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AsesorId",
                table: "SolicitudesApoyo",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesApoyo_AsesorId",
                table: "SolicitudesApoyo",
                column: "AsesorId");

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudesApoyo_Usuarios_AsesorId",
                table: "SolicitudesApoyo",
                column: "AsesorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudesApoyo_Usuarios_AsesorId",
                table: "SolicitudesApoyo");

            migrationBuilder.DropIndex(
                name: "IX_SolicitudesApoyo_AsesorId",
                table: "SolicitudesApoyo");

            migrationBuilder.DropColumn(
                name: "AsesorId",
                table: "SolicitudesApoyo");
        }
    }
}
