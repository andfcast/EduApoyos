using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EduApoyosBackend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarProgramaAcademico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProgramaAcademico",
                table: "Estudiantes");

            migrationBuilder.AddColumn<int>(
                name: "ProgramaAcademicoId",
                table: "Estudiantes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ProgramasAcademicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramasAcademicos", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ProgramasAcademicos",
                columns: new[] { "Id", "Descripcion" },
                values: new object[,]
                {
                    { 1, "Ingeniería de Sistemas" },
                    { 2, "Medicina" },
                    { 3, "Derecho" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Estudiantes_ProgramaAcademicoId",
                table: "Estudiantes",
                column: "ProgramaAcademicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Estudiantes_ProgramasAcademicos_ProgramaAcademicoId",
                table: "Estudiantes",
                column: "ProgramaAcademicoId",
                principalTable: "ProgramasAcademicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Estudiantes_ProgramasAcademicos_ProgramaAcademicoId",
                table: "Estudiantes");

            migrationBuilder.DropTable(
                name: "ProgramasAcademicos");

            migrationBuilder.DropIndex(
                name: "IX_Estudiantes_ProgramaAcademicoId",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "ProgramaAcademicoId",
                table: "Estudiantes");

            migrationBuilder.AddColumn<string>(
                name: "ProgramaAcademico",
                table: "Estudiantes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
