using EduApoyosBackend.Application.DTOs;
using EduApoyosBackend.Application.Services;
using EduApoyosBackend.Domain.Entities;
using EduApoyosBackend.Domain.Repositories;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EduApoyosBackend.Tests
{
    public class ProgramaAcademicoServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IProgramaAcademicoRepository> _programaRepoMock;
        private readonly ProgramaAcademicoService _service;

        public ProgramaAcademicoServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _programaRepoMock = new Mock<IProgramaAcademicoRepository>();

            _uowMock.Setup(u => u.ProgramasAcademicos).Returns(_programaRepoMock.Object);

            _service = new ProgramaAcademicoService(_uowMock.Object);
        }

        [Fact]
        public async Task ObtenerProgramasAcademicosActivosAsync_DeberiaRetornarDtos_CuandoHayProgramas()
        {
            // Arrange
            var entidades = new List<ProgramaAcademico>
            {
                new ProgramaAcademico(1, "Ingeniería de Sistemas"),
                new ProgramaAcademico(2, "Administración de Empresas")
            };

            _programaRepoMock.Setup(r => r.ListarAsync()).ReturnsAsync(entidades);

            // Act
            var resultado = await _service.ObtenerProgramasAcademicosActivosAsync();
            var lista = resultado.ToList();

            // Assert
            lista.Should().HaveCount(2);
            lista[0].Id.Should().Be(1);
            lista[0].Nombre.Should().Be("Ingeniería de Sistemas");
            lista[1].Id.Should().Be(2);
            lista[1].Nombre.Should().Be("Administración de Empresas");

            _programaRepoMock.Verify(r => r.ListarAsync(), Times.Once);
        }

        [Fact]
        public async Task ObtenerProgramasAcademicosActivosAsync_DeberiaRetornarColeccionVacia_CuandoNoHayProgramas()
        {
            // Arrange
            _programaRepoMock.Setup(r => r.ListarAsync()).ReturnsAsync(new List<ProgramaAcademico>());

            // Act
            var resultado = await _service.ObtenerProgramasAcademicosActivosAsync();

            // Assert
            resultado.Should().BeEmpty();
            _programaRepoMock.Verify(r => r.ListarAsync(), Times.Once);
        }
    }
}
