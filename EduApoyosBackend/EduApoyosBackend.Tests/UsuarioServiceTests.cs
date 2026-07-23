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
    public class UsuarioServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IPasswordHasher> _hasherMock;
        private readonly Mock<ITokenService> _jwtProviderMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
        private readonly Mock<IEstudianteRepository> _estudiantesRepoMock;

        private readonly UsuarioService _usuarioService;

        public UsuarioServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _hasherMock = new Mock<IPasswordHasher>();
            _jwtProviderMock = new Mock<ITokenService>();
            _usuarioRepoMock = new Mock<IUsuarioRepository>();
            _estudiantesRepoMock = new Mock<IEstudianteRepository>();

            // Configuramos el Unit of Work para que devuelva nuestros repositorios mockeados
            _uowMock.Setup(u => u.Usuarios).Returns(_usuarioRepoMock.Object);
            _uowMock.Setup(u => u.Estudiantes).Returns(_estudiantesRepoMock.Object);

            // Por defecto, los métodos asíncronos de repositorios devuelven tareas completadas
            _estudiantesRepoMock.Setup(r => r.AgregarAsync(It.IsAny<Estudiante>())).Returns(Task.CompletedTask);

            _usuarioService = new UsuarioService(_uowMock.Object, _hasherMock.Object, _jwtProviderMock.Object);
        }
        [Fact]
        public async Task RegisterUsuarioAsync_DeberiaRegistrarExitosamente_CuandoCorreoNoExiste()
        {
            // Arrange
            var dto = new RegistroUsuarioDto
            {
                NombreCompleto = "Ana Gómez",
                Email = "ana.gomez@edu.co",
                Password = "Password123*",
                RolId = 2
            };

            // Simulamos que el correo NO existe previamente
            _usuarioRepoMock.Setup(r => r.ObtenerPorCorreoAsync(dto.Email)).ReturnsAsync((Usuario?)null);
            _hasherMock.Setup(h => h.Hash(dto.Password)).Returns("hashed_password");

            // Act
            var resultado = await _usuarioService.RegistrarUsuarioAsync(dto);

            // Assert
            resultado.Should().Be("Usuario registrado con éxito de manera segura.");

            // Verificamos que se guardaron los cambios una vez
            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _usuarioRepoMock.Verify(r => r.AgregarAsync(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public async Task RegisterUsuarioAsync_DeberiaLanzarExcepcion_CuandoCorreoYaExiste()
        {
            // Arrange
            var dto = new RegistroUsuarioDto { Email = "existente@edu.co" };
            var usuarioExistente = new Usuario(Guid.NewGuid(), "Usuario Existente", dto.Email, "hashed_password", 1, DateTime.Now);

            // Simulamos que el correo YA existe
            _usuarioRepoMock.Setup(r => r.ObtenerPorCorreoAsync(dto.Email)).ReturnsAsync(usuarioExistente);

            // Act & Assert
            var accion = async () => await _usuarioService.RegistrarUsuarioAsync(dto);

            await accion.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("El correo electrónico ya está en uso.");

            // Verificamos que NUNCA se intentó guardar nada
            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
