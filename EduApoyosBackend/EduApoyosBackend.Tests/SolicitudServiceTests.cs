using EduApoyosBackend.Application.DTOs;
using EduApoyosBackend.Application.Services;
using EduApoyosBackend.Domain.Entities;
using EduApoyosBackend.Domain.Repositories;
using EduApoyosBackend.Infrastructure.Persistence;
using EduApoyosBackend.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.EntityFrameworkCore.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace EduApoyosBackend.Tests
{
    public class SolicitudServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ISolicitudApoyoRepository> _solRepoMock;
        private readonly Mock<IEstudianteRepository> _estudiantesRepoMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
        private readonly Mock<IHistorialEstadoRepository> _historialRepoMock;
        private readonly SolicitudService _service;

        public SolicitudServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _solRepoMock = new Mock<ISolicitudApoyoRepository>();
            _estudiantesRepoMock = new Mock<IEstudianteRepository>();
            _usuarioRepoMock = new Mock<IUsuarioRepository>();
            _historialRepoMock = new Mock<IHistorialEstadoRepository>();

            _uowMock.Setup(u => u.Solicitudes).Returns(_solRepoMock.Object);
            _uowMock.Setup(u => u.Estudiantes).Returns(_estudiantesRepoMock.Object);
            _uowMock.Setup(u => u.Usuarios).Returns(_usuarioRepoMock.Object);
            _uowMock.Setup(u => u.HistorialEstados).Returns(_historialRepoMock.Object);

            _solRepoMock.Setup(r => r.AgregarAsync(It.IsAny<SolicitudApoyo>())).Returns(Task.CompletedTask);
            _historialRepoMock.Setup(r => r.AgregarAsync(It.IsAny<HistorialEstado>())).Returns(Task.CompletedTask);
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

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

        [Fact]
        public async Task RegistrarSolicitudAsync_DeberiaLanzar_Error_CuandoEstudianteIdEsInvalido()
        {
            // Arrange
            var dto = new RegistroSolicitudDto
            {
                EstudianteId = Guid.Empty,
                TipoApoyoId = 1,
                MontoSolicitado = 100.0,
                Descripcion = "Descripción válida"
            };

            // Act
            Func<Task> act = async () => await _service.RegistrarSolicitudAsync(dto);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("El Id del estudiante no es válido.");

            // Verificar que no se intentó persistir nada
            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _solRepoMock.Verify(r => r.AgregarAsync(It.IsAny<SolicitudApoyo>()), Times.Never);
        }

        [Fact]
        public async Task RegistrarSolicitudAsync_DeberiaLanzar_Error_CuandoEstudianteNoExiste()
        {
            // Arrange
            var dto = new RegistroSolicitudDto
            {
                EstudianteId = Guid.NewGuid(),
                TipoApoyoId = 1,
                MontoSolicitado = 50.0,
                Descripcion = "Solicitud de prueba"
            };

            // Simular que no existe por Id ni por UsuarioId
            _estudiantesRepoMock.Setup(r => r.ObtenerPorGuidAsync(dto.EstudianteId)).ReturnsAsync((Estudiante?)null);
            _estudiantesRepoMock.Setup(r => r.BuscarAsync(It.IsAny<Expression<Func<Estudiante, bool>>>())).ReturnsAsync(Array.Empty<Estudiante>());

            // Act
            Func<Task> act = async () => await _service.RegistrarSolicitudAsync(dto);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("El estudiante especificado no existe.");

            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _solRepoMock.Verify(r => r.AgregarAsync(It.IsAny<SolicitudApoyo>()), Times.Never);
        }

        [Fact]
        public async Task RegistrarSolicitudAsync_DeberiaRegistrar_Correctamente_CuandoSeEncuentraEstudianteYNoSeProveeAsesor()
        {
            // Arrange
            var estudianteId = Guid.NewGuid();
            var dto = new RegistroSolicitudDto
            {
                EstudianteId = estudianteId,
                TipoApoyoId = 2,
                MontoSolicitado = 1234.5,
                Descripcion = "Necesito apoyo por motivos académicos",
                AsesorId = null // fuerza búsqueda de asesor en repositorio
            };

            // Estudiante existente (encontrado por ObtenerPorGuidAsync)
            var estudianteExistente = new Estudiante(estudianteId, Guid.NewGuid(), 1, "12345678", 1, 1);
            _estudiantesRepoMock.Setup(r => r.ObtenerPorGuidAsync(estudianteId)).ReturnsAsync(estudianteExistente);

            // Usuarios con rol asesor (RolId == 1)
            var asesor = new Usuario(Guid.NewGuid(), "Asesor Prueba", "asesor@edu.co", "hash", 1, DateTime.UtcNow);
            _usuarioRepoMock.Setup(r => r.BuscarAsync(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .ReturnsAsync(new[] { asesor }.AsEnumerable());

            // Capturamos la solicitud y el historial agregados
            SolicitudApoyo? solicitudAgregada = null;
            _solRepoMock.Setup(r => r.AgregarAsync(It.IsAny<SolicitudApoyo>()))
                .Callback<SolicitudApoyo>(s => solicitudAgregada = s)
                .Returns(Task.CompletedTask);

            HistorialEstado? historialAgregado = null;
            _historialRepoMock.Setup(r => r.AgregarAsync(It.IsAny<HistorialEstado>()))
                .Callback<HistorialEstado>(h => historialAgregado = h)
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _service.RegistrarSolicitudAsync(dto);

            // Assert
            resultado.Should().Be("Solicitud registrada con éxito de manera segura.");

            // Verificaciones de persistencia
            _solRepoMock.Verify(r => r.AgregarAsync(It.IsAny<SolicitudApoyo>()), Times.Once);
            _historialRepoMock.Verify(r => r.AgregarAsync(It.IsAny<HistorialEstado>()), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));

            solicitudAgregada.Should().NotBeNull();
            solicitudAgregada!.EstudianteId.Should().Be(estudianteExistente.Id);
            solicitudAgregada.TipoApoyoId.Should().Be(dto.TipoApoyoId);
            solicitudAgregada.MontoSolicitado.Should().Be(dto.MontoSolicitado);
            solicitudAgregada.Descripcion.Should().Be(dto.Descripcion);
            // El asesor elegido debe coincidir con el que devolvió el repo
            solicitudAgregada.AsesorId.Should().Be(asesor.Id);

            historialAgregado.Should().NotBeNull();
            historialAgregado!.SolicitudId.Should().Be(solicitudAgregada.Id);
            historialAgregado.UsuarioId.Should().Be(asesor.Id);
            historialAgregado.Observacion.Should().Contain("Solicitud registrada");
        }

        [Fact]
        public async Task ActualizarEstadoSolicitudAsync_DeberiaActualizarEstadoYCrearHistorial()
        {
            // Arrange
            var solicitudId = Guid.NewGuid();
            var estadoAnterior = 1;
            var nuevoEstado = 3;
            var usuarioId = Guid.NewGuid();

            var solicitud = new SolicitudApoyo(
                solicitudId,
                Guid.NewGuid(),
                tipoApoyoId: 2,
                monto: 100,
                descripcion: "Prueba",
                estadoSolicitudId: estadoAnterior,
                fechaSolicitud: DateTime.UtcNow.AddDays(-1),
                fechaActualizacion: DateTime.UtcNow.AddDays(-1),
                asesorId: null
            );

            _solRepoMock.Setup(r => r.ObtenerPorGuidAsync(solicitudId)).ReturnsAsync(solicitud);

            var dto = new ActualizarEstadoSolicitudDto { EstadoId = nuevoEstado, UsuarioId = usuarioId };

            // Act
            var result = await _service.ActualizarEstadoSolicitudAsync(solicitudId, dto);

            // Assert
            result.Should().Be("Estado de la solicitud actualizado con éxito.");
            solicitud.EstadoSolicitudId.Should().Be(nuevoEstado);
            solicitud.FechaActualizacion.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1)); // se actualizó recientemente

            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
            _historialRepoMock.Verify(r => r.AgregarAsync(It.Is<HistorialEstado>(h =>
                h.SolicitudId == solicitudId &&
                h.EstadoAnteriorId == estadoAnterior &&
                h.EstadoNuevoId == nuevoEstado &&
                h.UsuarioId == usuarioId &&
                h.Observacion == $"Cambio de estado de {estadoAnterior} a {nuevoEstado}"
            )), Times.Once);
        }

        [Fact]
        public async Task ActualizarEstadoSolicitudAsync_DeberiaLanzarExcepcion_CuandoSolicitudNoExiste()
        {
            // Arrange
            var solicitudId = Guid.NewGuid();
            _solRepoMock.Setup(r => r.ObtenerPorGuidAsync(solicitudId)).ReturnsAsync((SolicitudApoyo?)null);

            var dto = new ActualizarEstadoSolicitudDto { EstadoId = 2, UsuarioId = Guid.NewGuid() };

            // Act
            var accion = async () => await _service.ActualizarEstadoSolicitudAsync(solicitudId, dto);

            // Assert
            await accion.Should().ThrowAsync<InvalidOperationException>().WithMessage("La solicitud no existe.");

            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _historialRepoMock.Verify(r => r.AgregarAsync(It.IsAny<HistorialEstado>()), Times.Never);
        }

        private static DbContextOptions<AppDbContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task ObtenerSolicitudesXEstudianteAsync_DeberiaRetornarSolicitudes_EncontrandoEstudiantePorId()
        {
            // Arrange
            var options = CreateNewContextOptions();

            var estudianteId = Guid.NewGuid();
            var usuarioEstudianteId = Guid.NewGuid();
            var asesorId = Guid.NewGuid();

            using (var context = new AppDbContext(options))
            {
                // 1. Insertar TODOS los TiposApoyo requeridos (Id = 1 para que coincida con tipoApoyoId: 1)
                var tipoApoyo1 = new TipoApoyo(1, "Económico", "Económico");
                var tipoApoyo2 = new TipoApoyo(2, "Alimentario", "Alimentario");

                // 2. Insertar TODOS los EstadoSolicitud requeridos (Id = 1 y Id = 2 para el Historial)
                var estadoSolicitud1 = new EstadoSolicitud(1, "Pendiente", "Pendiente");
                var estadoSolicitud2 = new EstadoSolicitud(2, "En Revisión", "En Revisión");

                await context.TiposApoyo.AddRangeAsync(tipoApoyo1, tipoApoyo2);
                await context.EstadosSolicitud.AddRangeAsync(estadoSolicitud1, estadoSolicitud2);

                // Crear usuarios (rol seeded en OnModelCreating: 1 Asesor, 2 Estudiante)
                var usuarioEstudiante = new Usuario(usuarioEstudianteId, "Estudiante Uno", "est@edu.co", "hash", 2, DateTime.UtcNow);
                var usuarioAsesor = new Usuario(asesorId, "Asesor Uno", "asesor@edu.co", "hash", 1, DateTime.UtcNow);

                await context.Usuarios.AddRangeAsync(usuarioEstudiante, usuarioAsesor);

                // Crear estudiante apuntando al usuario creado
                var estudiante = new Estudiante(estudianteId, usuarioEstudianteId, 1, "12345", 1, 2);
                await context.Estudiantes.AddAsync(estudiante);

                var solicitudId = Guid.NewGuid();
                // tipoApoyoId: 1 coincide con tipoApoyo1
                var solicitud = new SolicitudApoyo(solicitudId, estudianteId, tipoApoyoId: 1, monto: 100.0, descripcion: "Necesidad X", estadoSolicitudId: 1,
                                                  fechaSolicitud: DateTime.UtcNow.AddDays(-1), fechaActualizacion: DateTime.UtcNow, asesorId: asesorId);

                var historial = new HistorialEstado(Guid.NewGuid(), solicitudId, estadoAnteriorId: 0, estadoNuevoId: 2, fechaCambio: DateTime.UtcNow, usuarioId: asesorId, observacion: "Observación");
                solicitud.HistorialEstados.Add(historial);

                await context.SolicitudesApoyo.AddAsync(solicitud);
                await context.SaveChangesAsync();
            }

            using (var context = new AppDbContext(options))
            {
                var uow = new UnitOfWork(context);
                var service = new SolicitudService(uow);

                // Act
                var resultados = (await service.ObtenerSolicitudesXEstudianteAsync(estudianteId)).ToList();

                // Assert
                resultados.Should().HaveCount(1);
                var dto = resultados.First();
                dto.NombreEstudiante.Should().Be("Estudiante Uno");
                dto.NombreAsesor.Should().Be("Asesor Uno");
                dto.TipoApoyo.Should().Be("Económico"); // Coincide con Id 1
                dto.Estado.Should().Be("Pendiente"); // Coincide con Id 1
                dto.HistorialEstados.Should().HaveCount(1);
                dto.HistorialEstados.First().EstadoAnterior.Should().BeEmpty();
                dto.HistorialEstados.First().EstadoSiguiente.Should().Be("En Revisión"); // Coincide con Id 2
            }
        }

        [Fact]
        public async Task ObtenerSolicitudesXEstudianteAsync_DeberiaRetornarSolicitudes_EncontrandoEstudiantePorUsuarioId()
        {
            // Arrange
            var options = CreateNewContextOptions();

            var estudianteId = Guid.NewGuid();
            var usuarioEstudianteId = Guid.NewGuid();
            
            using (var contextArrange = new AppDbContext(options))
            {
                var tipoApoyo = new TipoApoyo(2, "Alimentario", "Alimentario");
                var estadoSolicitud = new EstadoSolicitud(1, "Pendiente", "Pendiente");

                await contextArrange.TiposApoyo.AddAsync(tipoApoyo);
                await contextArrange.EstadosSolicitud.AddAsync(estadoSolicitud);
                var usuarioEstudiante = new Usuario(usuarioEstudianteId, "Estudiante Dos", "est2@edu.co", "hash", 2, DateTime.UtcNow);
                await contextArrange.Usuarios.AddAsync(usuarioEstudiante);

                var estudiante = new Estudiante(estudianteId, usuarioEstudianteId, 1, "54321", 1, 3);
                await contextArrange.Estudiantes.AddAsync(estudiante);

                var solicitud = new SolicitudApoyo(
                    Guid.NewGuid(),
                    estudianteId,
                    tipoApoyoId: 2,
                    monto: 50.0,
                    descripcion: "Necesidad Y",
                    estadoSolicitudId: 1,
                    fechaSolicitud: DateTime.UtcNow,
                    fechaActualizacion: DateTime.UtcNow,
                    asesorId: null
                );
                await contextArrange.SolicitudesApoyo.AddAsync(solicitud);
                
                await contextArrange.SaveChangesAsync();
            } 
            using (var contextAct = new AppDbContext(options))
            {
                var uow = new UnitOfWork(contextAct);
                var service = new SolicitudService(uow);

                // Act
                var resultados = (await service.ObtenerSolicitudesXEstudianteAsync(usuarioEstudianteId)).ToList();

                // Assert
                resultados.Should().HaveCount(1);
                var dto = resultados.First();
                dto.NombreEstudiante.Should().Be("Estudiante Dos");
                dto.TipoApoyo.Should().Be("Alimentario"); // seeded tipoApoyo 2
            }
        }

        [Fact]
        public async Task ObtenerSolicitudesXEstudianteAsync_DeberiaRetornarVacio_CuandoNoExisteEstudiante()
        {
            // Arrange
            var options = CreateNewContextOptions();

            using (var context = new AppDbContext(options))
            {
                // Context vacio (solo datos seedable en OnModelCreating)
                await context.SaveChangesAsync();
            }

            using (var context = new AppDbContext(options))
            {
                var uow = new UnitOfWork(context);
                var service = new SolicitudService(uow);

                // Act
                var resultados = await service.ObtenerSolicitudesXEstudianteAsync(Guid.NewGuid());

                // Assert
                resultados.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task ObtenerSolicitudesFiltradasAsync_FiltrarPorTipoApoyo_Y_Paginacion()
        {
            // Arrange
            var options = CreateNewContextOptions();

            using (var context = new AppDbContext(options))
            {
                var tipoA = new TipoApoyo(1, "A", "A");
                var tipoB = new TipoApoyo(2, "B", "B");
                var estado = new EstadoSolicitud(1, "Pendiente", "Pendiente");

                await context.TiposApoyo.AddRangeAsync(tipoA, tipoB);
                await context.EstadosSolicitud.AddAsync(estado);

                var usuario = new Usuario(Guid.NewGuid(), "Est", "est@x.co", "hash", 2, DateTime.UtcNow);
                await context.Usuarios.AddAsync(usuario);

                var estudiante = new Estudiante(Guid.NewGuid(), usuario.Id, 1, "dni", 1, 2);
                await context.Estudiantes.AddAsync(estudiante);

                // 6 solicitudes: 4 de tipo A y 2 de tipo B
                for (int i = 0; i < 6; i++)
                {
                    var tipoId = i < 4 ? 1 : 2;
                    var solicitud = new SolicitudApoyo(Guid.NewGuid(),
                        estudiante.Id,
                        tipoApoyoId: tipoId,
                        monto: 10 + i,
                        descripcion: $"desc-{i}",
                        estadoSolicitudId: 1,
                        fechaSolicitud: DateTime.UtcNow.AddDays(-i),
                        fechaActualizacion: DateTime.UtcNow.AddDays(-i),
                        asesorId: null);
                    await context.SolicitudesApoyo.AddAsync(solicitud);
                }

                await context.SaveChangesAsync();
            }

            using (var context = new AppDbContext(options))
            {
                var uow = new UnitOfWork(context);
                var service = new SolicitudService(uow);

                var filtro = new FiltroSolicitudDto
                {
                    TipoApoyoId = 2,
                    Pagina = 1,
                    TamanoPagina = 5
                };

                // Act
                var respuesta = await service.ObtenerSolicitudesFiltradasAsync(filtro);

                // Assert
                respuesta.TotalRegistros.Should().Be(2);
                respuesta.Elementos.Should().HaveCount(2);
                respuesta.Elementos.All(e => e.TipoApoyo == "B").Should().BeTrue();
                respuesta.PaginaActual.Should().Be(1);
            }
        }

        [Fact]
        public async Task ObtenerSolicitudesFiltradasAsync_FiltrarPorFecha_Y_NombreAsesorVacio_Y_Historial_Mapeado()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var fechaBase = DateTime.UtcNow.Date;

            using (var context = new AppDbContext(options))
            {
                var tipo = new TipoApoyo(1, "TipoX", "TipoX");
                var estadoNuevo = new EstadoSolicitud(2, "Iniciado", "Iniciado");
                var estadoAnterior = new EstadoSolicitud(0, "", "");

                await context.TiposApoyo.AddAsync(tipo);
                await context.EstadosSolicitud.AddRangeAsync(estadoAnterior, estadoNuevo);

                var usuario = new Usuario(Guid.NewGuid(), "Est", "est@x.co", "hash", 2, DateTime.UtcNow);
                await context.Usuarios.AddAsync(usuario);

                var estudiante = new Estudiante(Guid.NewGuid(), usuario.Id, 1, "dni", 1, 2);
                await context.Estudiantes.AddAsync(estudiante);

                // Solicitud con Asesor null y fecha en fechaBase (hora diferente)
                var sol1 = new SolicitudApoyo(Guid.NewGuid(),
                    estudiante.Id,
                    tipoApoyoId: 1,
                    monto: 100,
                    descripcion: "S1",
                    estadoSolicitudId: 2,
                    fechaSolicitud: fechaBase.AddHours(9),
                    fechaActualizacion: fechaBase.AddHours(9),
                    asesorId: null);

                // Agregar historial con EstadoAnterior nulo/0 y EstadoNuevo = 2
                var historial = new HistorialEstado(Guid.NewGuid(), sol1.Id, estadoAnteriorId: 0, estadoNuevoId: 2, fechaCambio: fechaBase.AddHours(9), usuarioId: usuario.Id, observacion: "creada");
                historial.EstadoNuevo = estadoNuevo;
                sol1.HistorialEstados.Add(historial);

                await context.SolicitudesApoyo.AddAsync(sol1);

                // Otra solicitud en diferente fecha (no debe aparecer en filtro)
                var sol2 = new SolicitudApoyo(Guid.NewGuid(),
                    estudiante.Id,
                    tipoApoyoId: 1,
                    monto: 200,
                    descripcion: "S2",
                    estadoSolicitudId: 2,
                    fechaSolicitud: fechaBase.AddDays(-1),
                    fechaActualizacion: fechaBase.AddDays(-1),
                    asesorId: Guid.NewGuid()); // con asesor para comprobar que solo la primera tiene NombreAsesor vacío
                await context.SolicitudesApoyo.AddAsync(sol2);

                await context.SaveChangesAsync();
            }

            using (var context = new AppDbContext(options))
            {
                var uow = new UnitOfWork(context);
                var service = new SolicitudService(uow);

                var filtro = new FiltroSolicitudDto
                {
                    Fecha = fechaBase,
                    Pagina = 1,
                    TamanoPagina = 10
                };

                // Act
                var resultado = await service.ObtenerSolicitudesFiltradasAsync(filtro);

                // Assert
                resultado.TotalRegistros.Should().Be(1);
                resultado.Elementos.Should().HaveCount(1);

                var dto = resultado.Elementos.First();
                dto.NombreAsesor.Should().BeEmpty();
                dto.HistorialEstados.Should().HaveCount(1);
                dto.HistorialEstados.First().EstadoAnterior.Should().BeEmpty();
                dto.HistorialEstados.First().EstadoSiguiente.Should().Be("Iniciado");
            }
        }

        [Fact]
        public async Task ObtenerSolicitudesFiltradasAsync_Paginacion_PaginasDiferentes()
        {
            // Arrange
            var options = CreateNewContextOptions();

            using (var context = new AppDbContext(options))
            {
                var tipo = new TipoApoyo(1, "TipoP", "TipoP");
                var estado = new EstadoSolicitud(1, "Pendiente", "Pendiente");
                await context.TiposApoyo.AddAsync(tipo);
                await context.EstadosSolicitud.AddAsync(estado);

                var usuario = new Usuario(Guid.NewGuid(), "Est", "est@x.co", "hash", 2, DateTime.UtcNow);
                await context.Usuarios.AddAsync(usuario);
                var estudiante = new Estudiante(Guid.NewGuid(), usuario.Id, 1, "dni", 1, 2);
                await context.Estudiantes.AddAsync(estudiante);

                // Crear 7 solicitudes para paginar (tamano pagina 5 -> pagina 1:5, pagina2:2)
                for (int i = 0; i < 7; i++)
                {
                    var solicitud = new SolicitudApoyo(Guid.NewGuid(),
                        estudiante.Id,
                        tipoApoyoId: 1,
                        monto: 10 + i,
                        descripcion: $"desc-{i}",
                        estadoSolicitudId: 1,
                        fechaSolicitud: DateTime.UtcNow.AddMinutes(-i),
                        fechaActualizacion: DateTime.UtcNow.AddMinutes(-i),
                        asesorId: null);
                    await context.SolicitudesApoyo.AddAsync(solicitud);
                }

                await context.SaveChangesAsync();
            }

            using (var context = new AppDbContext(options))
            {
                var uow = new UnitOfWork(context);
                var service = new SolicitudService(uow);

                var filtroPagina2 = new FiltroSolicitudDto
                {
                    Pagina = 2,
                    TamanoPagina = 5
                };

                // Act
                var respuesta = await service.ObtenerSolicitudesFiltradasAsync(filtroPagina2);

                // Assert
                respuesta.TotalRegistros.Should().Be(7);
                respuesta.Elementos.Should().HaveCount(2);
                respuesta.PaginaActual.Should().Be(2);
            }
        }
    }
}