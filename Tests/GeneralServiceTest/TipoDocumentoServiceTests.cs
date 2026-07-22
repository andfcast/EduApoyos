using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using EduApoyosBackend.Application.Services;
using EduApoyosBackend.Application.Interfaces;
using EduApoyosBackend.Domain.Repositories;
using EduApoyosBackend.Domain.Entities;
using EduApoyosBackend.Application.DTOs;
using System.Linq;

namespace Tests.GeneralServiceTest
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

            _uowMock.SetupGet(u => u.TiposDocumento).Returns(_tipoRepoMock.Object);

            _service = new TipoDocumentoService(_uowMock.Object);
        }

        [Fact]
        public async Task Listar_TiposDocumento_RetornaDtoLista()
        {
            // Arrange
            var tipos = new List<TipoDocumento>
            {
                new TipoDocumento(1, "DNI"),
                new TipoDocumento(2, "Pasaporte")
            };
            _tipoRepoMock.Setup(r => r.ListarAsync()).ReturnsAsync(tipos.Cast<TipoDocumento>());

            // Act
            var result = await _service.ListarTiposDocumentoAsync();

            // Assert
            Assert.NotNull(result);
            var lista = result.ToList();
            Assert.Equal(2, lista.Count);
            Assert.Contains(lista, t => t.Id == 1 && t.Nombre == "DNI");
            Assert.Contains(lista, t => t.Id == 2 && t.Nombre == "Pasaporte");
        }

        [Fact]
        public async Task ObtenerPorId_Existente_RetornaDto()
        {
            // Arrange
            var tipo = new TipoDocumento(1, "DNI");
            _tipoRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(tipo);

            // Act
            var result = await _service.ObtenerTipoDocumentoPorIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("DNI", result.Nombre);
        }

        [Fact]
        public async Task ObtenerPorId_NoExistente_LanzaInvalidOperationException()
        {
            // Arrange
            _tipoRepoMock.Setup(r => r.ObtenerPorIdAsync(99)).ReturnsAsync((TipoDocumento)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ObtenerTipoDocumentoPorIdAsync(99));
        }

        [Fact]
        public async Task Registrar_Nuevo_TipoDocumento_CreaYGuarda()
        {
            // Arrange
            var dto = new TipoDocumentoDto { Id = 0, Nombre = "Cédula" };
            _tipoRepoMock.Setup(r => r.AgregarAsync(It.IsAny<TipoDocumento>())).Returns(Task.CompletedTask).Verifiable();
            _uowMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1).Verifiable();

            // Act
            var result = await _service.RegistrarTipoDocumentoAsync(dto);

            // Assert
            Assert.Equal("Tipo de documento registrado con éxito.", result);
            _tipoRepoMock.Verify(r => r.AgregarAsync(It.Is<TipoDocumento>(t => t.Nombre == "Cédula")), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(default), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Actualizar_TipoDocumento_Existente_ActualizaYGuarda()
        {
            // Arrange
            var existente = new TipoDocumento(1, "DNI");
            _tipoRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(existente);
            _tipoRepoMock.Setup(r => r.Actualizar(It.IsAny<TipoDocumento>()));
            _uowMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1).Verifiable();

            var dto = new TipoDocumentoDto { Id = 1, Nombre = "Documento Nacional" };

            // Act
            var result = await _service.ActualizarTipoDocumentoAsync(dto);

            // Assert
            Assert.Equal("Tipo de documento actualizado con éxito.", result);
            _uowMock.Verify(u => u.SaveChangesAsync(default), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Actualizar_TipoDocumento_NoExistente_LanzaInvalidOperationException()
        {
            // Arrange
            _tipoRepoMock.Setup(r => r.ObtenerPorIdAsync(5)).ReturnsAsync((TipoDocumento)null);
            var dto = new TipoDocumentoDto { Id = 5, Nombre = "X" };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ActualizarTipoDocumentoAsync(dto));
        }

        [Fact]
        public async Task Borrar_TipoDocumento_Existente_EliminaYGuarda()
        {
            // Arrange
            var existente = new TipoDocumento(2, "Pasaporte");
            _tipoRepoMock.Setup(r => r.ObtenerPorIdAsync(2)).ReturnsAsync(existente);
            _tipoRepoMock.Setup(r => r.Borrar(It.IsAny<TipoDocumento>()));
            _uowMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1).Verifiable();

            // Act
            var result = await _service.EliminarTipoDocumentoAsync(2);

            // Assert
            Assert.Equal("Tipo de documento eliminado con éxito.", result);
            _tipoRepoMock.Verify(r => r.Borrar(It.Is<TipoDocumento>(t => t.Id == 2)), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(default), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Borrar_TipoDocumento_NoExistente_LanzaInvalidOperationException()
        {
            // Arrange
            _tipoRepoMock.Setup(r => r.ObtenerPorIdAsync(99)).ReturnsAsync((TipoDocumento)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.EliminarTipoDocumentoAsync(99));
        }
    }
}
