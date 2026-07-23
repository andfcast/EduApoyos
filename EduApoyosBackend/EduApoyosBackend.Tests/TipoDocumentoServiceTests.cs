using EduApoyosBackend.Application.DTOs;
using EduApoyosBackend.Application.Services;
using EduApoyosBackend.Domain.Entities;
using EduApoyosBackend.Domain.Repositories;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EduApoyosBackend.Tests
{
    public class TipoDocumentoServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ITipoDocumentoRepository> _tipoRepoMock;
        private readonly TipoDocumentoService _service;

        public TipoDocumentoServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _tipoRepoMock = new Mock<ITipoDocumentoRepository>();
            _uowMock.Setup(u => u.TiposDocumento).Returns(_tipoRepoMock.Object);

            _service = new TipoDocumentoService(_uowMock.Object);
        }

        [Fact]
        public async Task ObtenerTiposDocumentoActivosAsync_DeberiaMapearCorrectamente_LosTiposRetornadosPorRepositorio()
        {
            // Arrange
            var tipos = new List<TipoDocumento>
            {
                new TipoDocumento(1, "CC", "Cédula de ciudadanía"),
                new TipoDocumento(2, "TI", "Tarjeta de identidad")
            };
            _tipoRepoMock.Setup(r => r.ListarAsync()).ReturnsAsync(tipos);

            // Act
            var resultado = (await _service.ObtenerTiposDocumentoActivosAsync()).ToList();

            // Assert
            resultado.Should().HaveCount(2);
            resultado.Should().BeEquivalentTo(new[]
            {
                new TipoDocumentoDto { Id = 1, Codigo = "CC", Nombre = "Cédula de ciudadanía" },
                new TipoDocumentoDto { Id = 2, Codigo = "TI", Nombre = "Tarjeta de identidad" }
            }, options => options.ComparingByMembers<TipoDocumentoDto>());

            _tipoRepoMock.Verify(r => r.ListarAsync(), Times.Once);
        }

        [Fact]
        public async Task CrearServicio_SiUnitOfWorkEsNull_LlamarMetodoProvocaNullReferenceException()
        {
            // Arrange
            var servicioConNull = new TipoDocumentoService(null!);

            // Act
            var action = async () => await servicioConNull.ObtenerTiposDocumentoActivosAsync();

            // Assert
            await action.Should().ThrowAsync<NullReferenceException>();
        }
    }
}