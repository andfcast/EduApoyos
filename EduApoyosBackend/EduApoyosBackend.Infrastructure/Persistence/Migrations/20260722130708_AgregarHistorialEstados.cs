using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduApoyosBackend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarHistorialEstados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "MontoSolicitado",
                table: "SolicitudesApoyo",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MontoSolicitado",
                table: "SolicitudesApoyo");
        }
    }
}
