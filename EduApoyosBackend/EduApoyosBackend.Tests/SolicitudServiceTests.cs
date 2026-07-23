using EduApoyosBackend.Application.DTOs;
using EduApoyosBackend.Application.Services;
using EduApoyosBackend.Domain.Entities;
using EduApoyosBackend.Domain.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EduApoyosBackend.Tests
{
    public class SolicitudServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ISolicitudApoyoRepository> _solRepoMock;
        private readonly SolicitudService _service;

        public SolicitudServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _solRepoMock = new Mock<ISolicitudApoyoRepository>();

            _uowMock.Setup(u => u.Solicitudes).Returns(_solRepoMock.Object);

            _service = new SolicitudService(_uowMock.Object);
        }

        [Fact]
        public async Task ObtenerSolicitudesAsync_DeberiaMapearCorrectamente()
        {
            // Arrange
            var asesorUser = new Usuario(Guid.NewGuid(), "Asesor Uno", "asesor@x.co", "hash", 1, DateTime.UtcNow);
            var estudianteUser = new Usuario(Guid.NewGuid(), "Estudiante Uno", "est@x.co", "hash", 1, DateTime.UtcNow);
            var estudiante = new Estudiante(Guid.NewGuid(), estudianteUser.Id, 1, "123", 1, 3)
            {
                Usuario = estudianteUser
            };

            var estado = new EstadoSolicitud(1, "Pendiente", "");
            var tipo = new TipoApoyo(1, "Beca", "");

            var solicitud = new SolicitudApoyo(Guid.NewGuid(), estudiante.Id, tipo.Id, 150.0, "Necesito apoyo", estado.Id, DateTime.UtcNow, DateTime.UtcNow, asesorUser.Id)
            {
                Estudiante = estudiante,
                TipoApoyo = tipo,
                EstadoSolicitud = estado,
                Asesor = asesorUser
            };

            var historial = new HistorialEstado(Guid.NewGuid(), solicitud.Id, null, estado.Id, DateTime.UtcNow, estudianteUser.Id, "Creada");
            historial.EstadoNuevo = estado;
            solicitud.HistorialEstados.Add(historial);

            _solRepoMock.Setup(r => r.ListarAsync()).ReturnsAsync(new List<SolicitudApoyo> { solicitud });

            // Act
            var resultado = (await _service.ObtenerSolicitudesAsync()).ToList();

            // Assert
            resultado.Should().HaveCount(1);
            var dto = resultado[0];
            dto.Id.Should().Be(solicitud.Id);
            dto.Descripcion.Should().Be(solicitud.Descripcion);
            dto.MontoSolicitado.Should().Be(solicitud.MontoSolicitado);
            dto.TipoApoyo.Should().Be(tipo.Nombre);
            dto.Estado.Should().Be(estado.Nombre);
            dto.NombreEstudiante.Should().Be(estudianteUser.NombreCompleto);
            dto.NombreAsesor.Should().Be(asesorUser.NombreCompleto);
            dto.HistorialEstados.Should().HaveCount(1);
            dto.HistorialEstados[0].Observacion.Should().Be("Creada");
        }

        [Fact]
        public async Task ObtenerSolicitudAsync_DeberiaLanzarExcepcion_CuandoNoExiste()
        {
            // Arrange
            var id = Guid.NewGuid();
            _solRepoMock.Setup(r => r.ObtenerPorGuidAsync(id)).ReturnsAsync((SolicitudApoyo?)null);

            // Act
            Func<Task> act = async () => await _service.ObtenerSolicitudAsync(id);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("La solicitud no existe.");
        }

        [Fact]
        public async Task ObtenerSolicitudAsync_DeberiaRetornarDto_CuandoExiste()
        {
            // Arrange
            var asesorUser = new Usuario(Guid.NewGuid(), "Asesor Dos", "asesor2@x.co", "hash", 1, DateTime.UtcNow);
            var estudianteUser = new Usuario(Guid.NewGuid(), "Estudiante Dos", "est2@x.co", "hash", 1, DateTime.UtcNow);
            var estudiante = new Estudiante(Guid.NewGuid(), estudianteUser.Id, 1, "456", 1, 4)
            {
                Usuario = estudianteUser
            };

            var estado = new EstadoSolicitud(2, "Aprobado", "");
            var tipo = new TipoApoyo(2, "Crédito", "");

            var solicitud = new SolicitudApoyo(Guid.NewGuid(), estudiante.Id, tipo.Id, 200.0, "Apoyo económico", estado.Id, DateTime.UtcNow, DateTime.UtcNow, asesorUser.Id)
            {
                Estudiante = estudiante,
                TipoApoyo = tipo,
                EstadoSolicitud = estado,
                Asesor = asesorUser
            };

            var historial = new HistorialEstado(Guid.NewGuid(), solicitud.Id, null, estado.Id, DateTime.UtcNow, estudianteUser.Id, "Aprobada");
            historial.EstadoNuevo = estado;
            solicitud.HistorialEstados.Add(historial);

            _solRepoMock.Setup(r => r.ObtenerPorGuidAsync(solicitud.Id)).ReturnsAsync(solicitud);

            // Act
            var dto = await _service.ObtenerSolicitudAsync(solicitud.Id);

            // Assert
            dto.Should().NotBeNull();
            dto.Id.Should().Be(solicitud.Id);
            dto.TipoApoyo.Should().Be(tipo.Nombre);
            dto.Estado.Should().Be(estado.Nombre);
            dto.NombreEstudiante.Should().Be(estudianteUser.NombreCompleto);
            dto.NombreAsesor.Should().Be(asesorUser.NombreCompleto);
            dto.HistorialEstados.Should().HaveCount(1);
            dto.HistorialEstados[0].Observacion.Should().Be("Aprobada");
        }        
    }
}