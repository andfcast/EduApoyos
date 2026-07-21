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

        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _hasherMock = new Mock<IPasswordHasher>();
            _jwtProviderMock = new Mock<ITokenService>();
            _usuarioRepoMock = new Mock<IUsuarioRepository>();

            // Configuramos el Unit of Work para que devuelva nuestros repositorios mockeados
            _uowMock.Setup(u => u.Usuarios).Returns(_usuarioRepoMock.Object);

            _authService = new AuthService(_uowMock.Object, _hasherMock.Object, _jwtProviderMock.Object);
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