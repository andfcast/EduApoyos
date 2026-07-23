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
    public class TipoApoyoServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ITipoApoyoRepository> _tipoRepoMock;
        private readonly TipoApoyoService _service;

        public TipoApoyoServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _tipoRepoMock = new Mock<ITipoApoyoRepository>();

            _uowMock.Setup(u => u.TiposApoyo).Returns(_tipoRepoMock.Object);

            _service = new TipoApoyoService(_uowMock.Object);
        }

        [Fact]
        public async Task ObtenerTiposApoyoAsync_DeberiaMapearCorrectamente_LosTiposDevueltosPorRepositorio()
        {
            // Arrange
            var tiposEntidad = new List<TipoApoyo>
            {
                new TipoApoyo(1, "Tipo A", "Descripcion A"),
                new TipoApoyo(2, "Tipo B", "Descripcion B")
            };

            _tipoRepoMock.Setup(r => r.ListarAsync()).ReturnsAsync(tiposEntidad);

            var tiposEsperados = new[]
            {
                new TipoApoyoDto { Id = 1, Nombre = "Tipo A" },
                new TipoApoyoDto { Id = 2, Nombre = "Tipo B" }
            };

            // Act
            var resultado = await _service.ObtenerTiposApoyoAsync();
            var resultadoList = resultado.ToList();

            // Assert
            resultadoList.Should().HaveCount(2);
            resultadoList.Should().BeEquivalentTo(tiposEsperados, options => options.ComparingByMembers<TipoApoyoDto>());
            _tipoRepoMock.Verify(r => r.ListarAsync(), Times.Once);
        }

        [Fact]
        public async Task ObtenerTiposApoyoAsync_DeberiaRetornarColeccionVacia_CuandoRepositorioNoTieneTipos()
        {
            // Arrange
            _tipoRepoMock.Setup(r => r.ListarAsync()).ReturnsAsync(new List<TipoApoyo>());

            // Act
            var resultado = await _service.ObtenerTiposApoyoAsync();

            // Assert
            resultado.Should().BeEmpty();
            _tipoRepoMock.Verify(r => r.ListarAsync(), Times.Once);
        }
    }
}
