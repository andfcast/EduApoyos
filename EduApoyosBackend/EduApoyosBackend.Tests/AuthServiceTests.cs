using EduApoyosBackend.Application.DTOs;
using EduApoyosBackend.Application.Interfaces;
using EduApoyosBackend.Application.Services;
using EduApoyosBackend.Domain.Repositories;
using System.Timers;
using FluentAssertions;
using Moq;
using Xunit;
using EduApoyosBackend.Domain.Entities;

namespace EduApoyosBackend.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IPasswordHasher> _hasherMock;
        private readonly Mock<ITokenService> _jwtProviderMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
        private readonly Mock<IEstudianteRepository> _estudianteRepoMock;

        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _hasherMock = new Mock<IPasswordHasher>();
            _jwtProviderMock = new Mock<ITokenService>();
            _usuarioRepoMock = new Mock<IUsuarioRepository>();
            _estudianteRepoMock = new Mock<IEstudianteRepository>();

            // Configuramos el Unit of Work para que devuelva nuestros repositorios mockeados
            _uowMock.Setup(u => u.Usuarios).Returns(_usuarioRepoMock.Object);
            _uowMock.Setup(u => u.Estudiantes).Returns(_estudianteRepoMock.Object);

            _authService = new AuthService(_uowMock.Object, _hasherMock.Object, _jwtProviderMock.Object);
        }

        [Fact]
        public async Task RegisterEstudianteAsync_DeberiaRegistrarExitosamente_CuandoCorreoNoExiste()
        {
            // Arrange
            var dto = new RegistroUsuarioDto
            {
                NombreCompleto = "Ana Gómez",
                Email = "ana.gomez@edu.co",
                Password = "Password123*",
                TipoDocumentoId = 1,
                NumeroDocumento = "123456789",
                ProgramaAcademico = "Ingeniería",
                Semestre = 3
            };

            // Simulamos que el correo NO existe previamente
            _usuarioRepoMock.Setup(r => r.ObtenerPorCorreoAsync(dto.Email)).ReturnsAsync((Usuario?)null);
            _hasherMock.Setup(h => h.Hash(dto.Password)).Returns("hashed_password");

            // Act
            var resultado = await _authService.RegistrarEstudianteAsync(dto);

            // Assert
            resultado.Should().Be("Estudiante registrado con éxito de manera segura.");

            // Verificamos que se guardaron los cambios dos veces (usuario y estudiante)
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
            _usuarioRepoMock.Verify(r => r.AgregarAsync(It.IsAny<Usuario>()), Times.Once);
            _estudianteRepoMock.Verify(r => r.AgregarAsync(It.IsAny<Estudiante>()), Times.Once);
        }

        [Fact]
        public async Task RegisterEstudianteAsync_DeberiaLanzarExcepcion_CuandoCorreoYaExiste()
        {
            // Arrange
            var dto = new RegistroUsuarioDto { Email = "existente@edu.co" };
            var usuarioExistente = new Usuario(Guid.NewGuid(), "Usuario Existente", dto.Email, "hashed_password", 1, DateTime.Now);

            // Simulamos que el correo YA existe
            _usuarioRepoMock.Setup(r => r.ObtenerPorCorreoAsync(dto.Email)).ReturnsAsync(usuarioExistente);

            // Act & Assert
            var accion = async () => await _authService.RegistrarEstudianteAsync(dto);

            await accion.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("El correo electrónico ya está en uso.");

            // Verificamos que NUNCA se intentó guardar nada
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task RegisterEstudianteAsync_DeberiaLanzarExcepcion_CuandoEstudianteYaExiste()
        {
            // Arrange
            var dto = new RegistroUsuarioDto { Email = "existente@edu.co", TipoDocumentoId = 1, NumeroDocumento = "987654321" };
            var estudianteExistente = new Estudiante(Guid.NewGuid(),Guid.NewGuid(), dto.TipoDocumentoId, dto.NumeroDocumento, "Ingeniería", 3);

            // Simulamos que el correo YA existe
            _estudianteRepoMock.Setup(r => r.ExisteUsuarioPorNumDocumentoAsync(dto.TipoDocumentoId, dto.NumeroDocumento)).ReturnsAsync(true);

            // Act & Assert
            var accion = async () => await _authService.RegistrarEstudianteAsync(dto);

            await accion.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("El número de documento ya está en uso.");

            // Verificamos que NUNCA se intentó guardar nada
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_DeberiaRetornarToken_CuandoCredencialesSonCorrectas()
        {
            // Arrange
            var dto = new LoginDto { Email = "carlos@edu.co", Password = "Password123*" };
            var guidTest = Guid.NewGuid();
            var usuarioEnDb = new Usuario(guidTest, "Carlos Pérez", dto.Email, "hashed_password",1,DateTime.Now);
                
            _usuarioRepoMock.Setup(r => r.ObtenerPorCorreoAsync(dto.Email)).ReturnsAsync(usuarioEnDb);
            _hasherMock.Setup(h => h.Verify(dto.Password, usuarioEnDb.PasswordHash)).Returns(true);
            _jwtProviderMock.Setup(j => j.GenerateToken(usuarioEnDb)).Returns("token_jwt_mocked");

            // Act
            var resultado = await _authService.LoginAsync(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Token.Should().Be("token_jwt_mocked");
            resultado.UsuarioId.Should().Be(guidTest);
            resultado.Nombre.Should().Be("Carlos Pérez");
            resultado.Mensaje.Should().Be("Autenticación exitosa.");
        }

        [Fact]
        public async Task LoginAsync_DeberiaLanzarExcepcion_CuandoUsuarioNoExiste()
        {
            // Arrange
            var dto = new LoginDto { Email = "noexiste@edu.co", Password = "123" };

            _usuarioRepoMock.Setup(r => r.ObtenerPorCorreoAsync(dto.Email)).ReturnsAsync((Usuario?)null);

            // Act & Assert
            var accion = async () => await _authService.LoginAsync(dto);

            await accion.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Credenciales inválidas.");
        }

        [Fact]
        public async Task LoginAsync_DeberiaLanzarExcepcion_CuandoContrasenaEsIncorrecta()
        {
            // Arrange
            var dto = new LoginDto { Email = "carlos@edu.co", Password = "WrongPassword" };
            var usuarioEnDb = new Usuario(Guid.NewGuid(), "Carlos Pérez", dto.Email, "hashed_password", 1, DateTime.Now);

            _usuarioRepoMock.Setup(r => r.ObtenerPorCorreoAsync(dto.Email)).ReturnsAsync(usuarioEnDb);
            _hasherMock.Setup(h => h.Verify(dto.Password, usuarioEnDb.PasswordHash)).Returns(false);

            // Act & Assert
            var accion = async () => await _authService.LoginAsync(dto);

            await accion.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Credenciales inválidas.");
        }
    }
}