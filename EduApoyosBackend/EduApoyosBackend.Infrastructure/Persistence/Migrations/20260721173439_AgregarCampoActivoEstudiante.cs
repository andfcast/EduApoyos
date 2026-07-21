using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduApoyosBackend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCampoActivoEstudiante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Estudiantes",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Estudiantes");
        }
    }
}
