using EduApoyosBackend.Application.DTOs;
using EduApoyosBackend.Application.Interfaces;
using EduApoyosBackend.Application.Services;
using EduApoyosBackend.Domain.Entities;
using EduApoyosBackend.Domain.Repositories;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Tests
{
    public class EstudianteServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IPasswordHasher> _hasherMock;
        private readonly Mock<ITokenService> _jwtProviderMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
        private readonly Mock<IEstudianteRepository> _estudianteRepoMock;

        private readonly EstudianteService _estudianteService;

        public EstudianteServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _hasherMock = new Mock<IPasswordHasher>();
            _jwtProviderMock = new Mock<ITokenService>();
            _usuarioRepoMock = new Mock<IUsuarioRepository>();
            _estudianteRepoMock = new Mock<IEstudianteRepository>();

            // Configuramos el Unit of Work para que devuelva nuestros repositorios mockeados
            _uowMock.Setup(u => u.Usuarios).Returns(_usuarioRepoMock.Object);
            _uowMock.Setup(u => u.Estudiantes).Returns(_estudianteRepoMock.Object);

            _estudianteService = new EstudianteService(_uowMock.Object, _hasherMock.Object, _jwtProviderMock.Object);
        }

        [Fact]
        public async Task RegisterEstudianteAsync_DeberiaRegistrarExitosamente_CuandoCorreoNoExiste()
        {
            // Arrange
            var dto = new RegistroEstudianteDto
            {
                NombreCompleto = "Ana Gómez",
                Email = "ana.gomez@edu.co",
                Password = "Password123*",
                TipoDocumentoId = 1,
                NumeroDocumento = "123456789",
                ProgramaAcademicoId = 1,
                Semestre = 3
            };

            // Simulamos que el correo NO existe previamente
            _usuarioRepoMock.Setup(r => r.ObtenerPorCorreoAsync(dto.Email)).ReturnsAsync((Usuario?)null);
            _hasherMock.Setup(h => h.Hash(dto.Password)).Returns("hashed_password");

            // Act
            var resultado = await _estudianteService.RegistrarEstudianteAsync(dto);

            // Assert
            resultado.Should().Be("Estudiante registrado con éxito de manera segura.");

            // Verificamos que se guardaron los cambios dos veces (usuario y estudiante)
            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
            _usuarioRepoMock.Verify(r => r.AgregarAsync(It.IsAny<Usuario>()), Times.Once);
            _estudianteRepoMock.Verify(r => r.AgregarAsync(It.IsAny<Estudiante>()), Times.Once);
        }

        [Fact]
        public async Task RegisterEstudianteAsync_DeberiaLanzarExcepcion_CuandoCorreoYaExiste()
        {
            // Arrange
            var dto = new RegistroEstudianteDto { Email = "existente@edu.co" };
            var usuarioExistente = new Usuario(Guid.NewGuid(), "Usuario Existente", dto.Email, "hashed_password", 1, DateTime.Now);

            // Simulamos que el correo YA existe
            _usuarioRepoMock.Setup(r => r.ObtenerPorCorreoAsync(dto.Email)).ReturnsAsync(usuarioExistente);

            // Act & Assert
            var accion = async () => await _estudianteService.RegistrarEstudianteAsync(dto);

            await accion.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("El correo electrónico ya está en uso.");

            // Verificamos que NUNCA se intentó guardar nada
            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task RegisterEstudianteAsync_DeberiaLanzarExcepcion_CuandoEstudianteYaExiste()
        {
            // Arrange
            var dto = new RegistroEstudianteDto { Email = "existente@edu.co", TipoDocumentoId = 1, NumeroDocumento = "987654321" };
            var estudianteExistente = new Estudiante(Guid.NewGuid(), Guid.NewGuid(), dto.TipoDocumentoId, dto.NumeroDocumento, 1, 3);

            // Simulamos que el correo YA existe
            _estudianteRepoMock.Setup(r => r.ExisteUsuarioPorNumDocumentoAsync(dto.TipoDocumentoId, dto.NumeroDocumento)).ReturnsAsync(true);

            // Act & Assert
            var accion = async () => await _estudianteService.RegistrarEstudianteAsync(dto);

            await accion.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("El número de documento ya está en uso.");

            // Verificamos que NUNCA se intentó guardar nada
            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
