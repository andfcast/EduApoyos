using System.Collections.Generic;
using System.Threading.Tasks;
using EduApoyosBackend.Application.DTOs;
using EduApoyosBackend.Application.Services;
using EduApoyosBackend.Domain.Entities;
using EduApoyosBackend.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace EduApoyosBackend.Tests
{
    public class RolServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IRolRepository> _rolRepoMock;
        private readonly RolService _rolService;

        public RolServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _rolRepoMock = new Mock<IRolRepository>();

            _uowMock.Setup(u => u.Roles).Returns(_rolRepoMock.Object);

            _rolService = new RolService(_uowMock.Object);
        }

        [Fact]
        public async Task ObtenerRolesActivosAsync_DeberiaRetornarRolesMapeados()
        {
            // Arrange
            var rolesEnRepo = new List<Rol>
            {
                new Rol(1, "Administrador"),
                new Rol(2, "Usuario")
            };

            _rolRepoMock.Setup(r => r.ListarAsync()).ReturnsAsync(rolesEnRepo);

            // Act
            var resultado = await _rolService.ObtenerRolesActivosAsync();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.Should().BeEquivalentTo(new[]
            {
                new RolDto { Id = 1, Nombre = "Administrador" },
                new RolDto { Id = 2, Nombre = "Usuario" }
            });
        }

        [Fact]
        public async Task ObtenerRolesActivosAsync_DeberiaRetornarColeccionVacia_CuandoNoHayRoles()
        {
            // Arrange
            _rolRepoMock.Setup(r => r.ListarAsync()).ReturnsAsync(new List<Rol>());

            // Act
            var resultado = await _rolService.ObtenerRolesActivosAsync();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
        }
    }
}