using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EduApoyosBackend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EstadosSolicitud",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosSolicitud", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposApoyo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposApoyo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposDocumento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposDocumento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NombreCompleto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Estudiantes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoDocumentoId = table.Column<int>(type: "int", nullable: false),
                    NumeroDocumento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProgramaAcademico = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Semestre = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estudiantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estudiantes_TiposDocumento_TipoDocumentoId",
                        column: x => x.TipoDocumentoId,
                        principalTable: "TiposDocumento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Estudiantes_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesApoyo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EstudianteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoApoyoId = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstadoSolicitudId = table.Column<int>(type: "int", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesApoyo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesApoyo_EstadosSolicitud_EstadoSolicitudId",
                        column: x => x.EstadoSolicitudId,
                        principalTable: "EstadosSolicitud",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesApoyo_Estudiantes_EstudianteId",
                        column: x => x.EstudianteId,
                        principalTable: "Estudiantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolicitudesApoyo_TiposApoyo_TipoApoyoId",
                        column: x => x.TipoApoyoId,
                        principalTable: "TiposApoyo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistorialesEstados",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SolicitudId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EstadoAnteriorId = table.Column<int>(type: "int", nullable: true),
                    EstadoNuevoId = table.Column<int>(type: "int", nullable: false),
                    FechaCambio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialesEstados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialesEstados_EstadosSolicitud_EstadoAnteriorId",
                        column: x => x.EstadoAnteriorId,
                        principalTable: "EstadosSolicitud",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialesEstados_EstadosSolicitud_EstadoNuevoId",
                        column: x => x.EstadoNuevoId,
                        principalTable: "EstadosSolicitud",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialesEstados_SolicitudesApoyo_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "SolicitudesApoyo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistorialesEstados_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "EstadosSolicitud",
                columns: new[] { "Id", "Descripcion", "Nombre" },
                values: new object[,]
                {
                    { 1, "Radicada", "Pendiente" },
                    { 2, "Validando", "En Revisión" },
                    { 3, "Aceptada", "Aprobada" },
                    { 4, "No cumple requisitos", "Rechazada" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Asesor" },
                    { 2, "Estudiante" }
                });

            migrationBuilder.InsertData(
                table: "TiposApoyo",
                columns: new[] { "Id", "Descripcion", "Nombre" },
                values: new object[,]
                {
                    { 1, "Apoyo financiero mensual", "Económico" },
                    { 2, "Acceso a comedores universitarios", "Alimentario" },
                    { 3, "Subsidio de transporte urbano", "Transporte" }
                });

            migrationBuilder.InsertData(
                table: "TiposDocumento",
                columns: new[] { "Id", "Codigo", "Descripcion" },
                values: new object[,]
                {
                    { 1, "CC", "Cédula de Ciudadanía" },
                    { 2, "TI", "Tarjeta de Identidad" },
                    { 3, "PS", "Pasaporte" },
                    { 4, "CE", "Cédula de Extranjería" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Estudiantes_TipoDocumentoId",
                table: "Estudiantes",
                column: "TipoDocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Estudiantes_UsuarioId",
                table: "Estudiantes",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesEstados_EstadoAnteriorId",
                table: "HistorialesEstados",
                column: "EstadoAnteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesEstados_EstadoNuevoId",
                table: "HistorialesEstados",
                column: "EstadoNuevoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesEstados_SolicitudId",
                table: "HistorialesEstados",
                column: "SolicitudId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesEstados_UsuarioId",
                table: "HistorialesEstados",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesApoyo_EstadoSolicitudId",
                table: "SolicitudesApoyo",
                column: "EstadoSolicitudId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesApoyo_EstudianteId",
                table: "SolicitudesApoyo",
                column: "EstudianteId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesApoyo_TipoApoyoId",
                table: "SolicitudesApoyo",
                column: "TipoApoyoId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RolId",
                table: "Usuarios",
                column: "RolId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorialesEstados");

            migrationBuilder.DropTable(
                name: "SolicitudesApoyo");

            migrationBuilder.DropTable(
                name: "EstadosSolicitud");

            migrationBuilder.DropTable(
                name: "Estudiantes");

            migrationBuilder.DropTable(
                name: "TiposApoyo");

            migrationBuilder.DropTable(
                name: "TiposDocumento");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
