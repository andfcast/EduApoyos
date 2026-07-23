using EduApoyosBackend.Application.DTOs;
using EduApoyosBackend.Application.Interfaces;
using EduApoyosBackend.Application.Services;
using EduApoyosBackend.Domain.Entities;
using EduApoyosBackend.Domain.Repositories;
using EduApoyosBackend.Infrastructure.Persistence;
using EduApoyosBackend.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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

        private DbContextOptions<AppDbContext> CrearOpcionesBaseDatosMemoria()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task ObtenerEstudiantesPaginadosAsync_DeberiaFiltrarYPaginarCorrectamente()
        {
            var options = CrearOpcionesBaseDatosMemoria();

            using (var contextArrange = new AppDbContext(options))
            {
                // 1. Crear e insertar programas académicos (Catálogos)
                var programa1 = new ProgramaAcademico(1, "Ingeniería de Sistemas");
                var programa2 = new ProgramaAcademico(2, "Medicina");
                await contextArrange.ProgramasAcademicos.AddRangeAsync(programa1, programa2);

                // 2. Crear e insertar usuarios
                var usuario1 = new Usuario(Guid.NewGuid(), "Carlos Pérez", "carlos@edu.co", "hash", 2, DateTime.UtcNow);
                var usuario2 = new Usuario(Guid.NewGuid(), "Ana María Gómez", "ana@edu.co", "hash", 2, DateTime.UtcNow);
                var usuario3 = new Usuario(Guid.NewGuid(), "Carlos Rodríguez", "crodriguez@edu.co", "hash", 2, DateTime.UtcNow);
                await contextArrange.Usuarios.AddRangeAsync(usuario1, usuario2, usuario3);

                // 3. Crear estudiantes usando las FKs (1 para Sistemas, 2 para Medicina) sin reasignar la propiedad de navegación
                var estudiante1 = new Estudiante(Guid.NewGuid(), usuario1.Id, tipoDocumentoId: 1, "101010", programaAcademicoId: 1, semestre: 3);
                var estudiante2 = new Estudiante(Guid.NewGuid(), usuario2.Id, tipoDocumentoId: 2, "202020", programaAcademicoId: 1, semestre: 5);
                var estudiante3 = new Estudiante(Guid.NewGuid(), usuario3.Id, tipoDocumentoId: 1, "303030", programaAcademicoId: 1, semestre: 2);

                await contextArrange.Estudiantes.AddRangeAsync(estudiante1, estudiante2, estudiante3);

                // Guardar todos los cambios de una sola vez
                await contextArrange.SaveChangesAsync();
            }

            // 2. Act & Assert en un contexto limpio (Act)
            using (var contextAct = new AppDbContext(options))
            {
                var unitOfWork = new UnitOfWork(contextAct);
                var servicio = new EstudianteService(unitOfWork, _hasherMock.Object, _jwtProviderMock.Object);

                // Búsqueda por "Carlos" (debería encontrar a Carlos Pérez y Carlos Rodríguez -> 2 registros)
                // Página 1, tamaño 1 -> Debe retornar sólo 1 registro pero con TotalRegistros = 2
                var resultado = await servicio.ObtenerEstudiantesPaginadosAsync(
                    busqueda: "carlos",
                    pagina: 1,
                    tamanoPagina: 1);

                // Assert
                resultado.Should().NotBeNull();
                resultado.TotalRegistros.Should().Be(2); // Total filtrado por "Carlos"
                resultado.PaginaActual.Should().Be(1);
                resultado.Elementos.Should().HaveCount(1);

                // Como los ordena por NombreCompleto: "Carlos Pérez" va antes que "Carlos Rodríguez"
                resultado.Elementos.First().NombreCompleto.Should().Be("Carlos Pérez");
            }
        }

        [Fact]
        public async Task ObtenerEstudiantesPaginadosAsync_SinFiltro_DeberiaRetornarTodosPaginados()
        {
            // Arrange
            var options = CrearOpcionesBaseDatosMemoria();

            using (var contextArrange = new AppDbContext(options))
            {
                var usuario1 = new Usuario(Guid.NewGuid(), "Estudiante A", "a@edu.co", "hash", 2, DateTime.UtcNow);
                var usuario2 = new Usuario(Guid.NewGuid(), "Estudiante B", "b@edu.co", "hash", 2, DateTime.UtcNow);
                await contextArrange.Usuarios.AddRangeAsync(usuario1, usuario2);

                var programa = new ProgramaAcademico(3, "Derecho");
                await contextArrange.ProgramasAcademicos.AddAsync(programa);

                var estudiante1 = new Estudiante(Guid.NewGuid(), usuario1.Id, 1, "1111", 3, 1);
                var estudiante2 = new Estudiante(Guid.NewGuid(), usuario2.Id, 1, "2222", 3, 1);

                await contextArrange.Estudiantes.AddRangeAsync(estudiante1, estudiante2);
                await contextArrange.SaveChangesAsync();
            }

            // Act & Assert
            using (var contextAct = new AppDbContext(options))
            {
                var unitOfWork = new UnitOfWork(contextAct);
                var servicio = new EstudianteService(unitOfWork, _hasherMock.Object, _jwtProviderMock.Object);
                var resultado = await servicio.ObtenerEstudiantesPaginadosAsync(
                    busqueda: null,
                    pagina: 1,
                    tamanoPagina: 10);

                // Assert
                resultado.TotalRegistros.Should().Be(2);
                resultado.Elementos.Should().HaveCount(2);
            }
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
