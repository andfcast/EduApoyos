using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EduApoyosBackend.Application.Services;
using EduApoyosBackend.Domain.Entities;
using EduApoyosBackend.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace EduApoyosBackend.Tests
{
    public class EstadoSolicitudServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IEstadoSolicitudRepository> _estadoRepoMock;
        private readonly EstadoSolicitudService _service;

        public EstadoSolicitudServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _estadoRepoMock = new Mock<IEstadoSolicitudRepository>();

            _uowMock.Setup(u => u.EstadosSolicitud).Returns(_estadoRepoMock.Object);

            _service = new EstadoSolicitudService(_uowMock.Object);
        }

        [Fact]
        public async Task ObtenerEstadosSolicitudAsync_DeberiaRetornarDtos_MapeadosCorrectamente()
        {
            // Arrange
            var estados = new List<EstadoSolicitud>
            {
                new EstadoSolicitud(1, "Pendiente", "Pendiente de revisión"),
                new EstadoSolicitud(2, "Aprobado", "Solicitud aprobada")
            };

            _estadoRepoMock.Setup(r => r.ListarAsync()).ReturnsAsync(estados);

            // Act
            var resultado = await _service.ObtenerEstadosSolicitudAsync();
            var lista = resultado.ToList();

            // Assert
            lista.Should().HaveCount(2);
            lista[0].Id.Should().Be(1);
            lista[0].Nombre.Should().Be("Pendiente");
            lista[1].Id.Should().Be(2);
            lista[1].Nombre.Should().Be("Aprobado");
        }

        [Fact]
        public async Task ObtenerEstadosSolicitudAsync_DeberiaRetornarColeccionVacia_CuandoNoHayEstados()
        {
            // Arrange
            var estados = new List<EstadoSolicitud>();
            _estadoRepoMock.Setup(r => r.ListarAsync()).ReturnsAsync(estados);

            // Act
            var resultado = await _service.ObtenerEstadosSolicitudAsync();

            // Assert
            resultado.Should().BeEmpty();
        }
    }
}
