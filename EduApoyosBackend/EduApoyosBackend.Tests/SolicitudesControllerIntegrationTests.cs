using System;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EduApoyosBackend.API;
using EduApoyosBackend.Application.DTOs;
using EduApoyosBackend.Domain.Entities;
using EduApoyosBackend.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
namespace EduApoyosBackend.Tests
{
    public class SolicitudesControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory; private readonly System.Net.Http.HttpClient _client;


        public SolicitudesControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetSolicitudes_DeberiaRetornarOkYLista_CuandoUsuarioEsAsesor()
        {
            // Preparar datos: asesor, estudiante y solicitud
            Guid solicitudId;
            var estudianteEmail = "integration.estudiante.solicitudes@edu.co";
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                if (!db.Usuarios.Any(u => u.Email == "integration.asesor@edu.co"))
                {
                    db.Usuarios.Add(new Usuario(Guid.NewGuid(), "Asesor Integration", "integration.asesor@edu.co", BCrypt.Net.BCrypt.HashPassword("Password123*"), 1, DateTime.UtcNow));
                }

                Usuario usuarioEst;
                if (!db.Usuarios.Any(u => u.Email == estudianteEmail))
                {
                    usuarioEst = new Usuario(Guid.NewGuid(), "Estudiante Integration", estudianteEmail, BCrypt.Net.BCrypt.HashPassword("Password123*"), 2, DateTime.UtcNow);
                    db.Usuarios.Add(usuarioEst);
                    await db.SaveChangesAsync();
                }
                else
                {
                    usuarioEst = db.Usuarios.First(u => u.Email == estudianteEmail);
                }

                var tipoApoyoId = db.TiposApoyo.Select(t => t.Id).First();
                var tipoDocId = db.TiposDocumento.Select(t => t.Id).First();
                var progId = db.ProgramasAcademicos.Select(p => p.Id).First();
                var asesorId = db.Usuarios.First(u => u.Email == "integration.asesor@edu.co").Id;

                var estudiante = db.Estudiantes.FirstOrDefault(e => e.UsuarioId == usuarioEst.Id)
                                ?? new Estudiante(Guid.NewGuid(), usuarioEst.Id, tipoDocId, "999888777", progId, 1);
                if (!db.Estudiantes.Any(e => e.UsuarioId == usuarioEst.Id))
                {
                    db.Estudiantes.Add(estudiante);
                    await db.SaveChangesAsync();
                }

                // Crear solicitud de prueba
                if (!db.SolicitudesApoyo.Any(s => s.EstudianteId == estudiante.Id && s.Descripcion.Contains("Solicitud integración GET")))
                {
                    var solicitud = new SolicitudApoyo(Guid.NewGuid(), estudiante.Id, tipoApoyoId, 12345, "Solicitud integración GET", 1, DateTime.UtcNow, DateTime.UtcNow, asesorId);
                    db.SolicitudesApoyo.Add(solicitud);
                }

                await db.SaveChangesAsync();
            }

            // Login como asesor
            var loginDto = new LoginDto { Email = "integration.asesor@edu.co", Password = "Password123*" };
            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
            loginResult.Should().NotBeNull();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult!.Token);

            // Act
            var response = await _client.GetAsync("/api/Solicitudes?pagina=1&tamanoPagina=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var pag = await response.Content.ReadFromJsonAsync<RespuestaPaginadaDto<SolicitudApoyoDto>>();
            pag.Should().NotBeNull();
            pag!.Elementos.Select(e => e.Descripcion).Should().Contain(d => d.Contains("Solicitud integración GET"));
        }

        [Fact]
        public async Task Post_DeberiaRegistrarSolicitud_CuandoUsuarioEsEstudiante()
        {
            // Preparar estudiante y asesor (asesor para asignar)
            var estudianteEmail = "integration.estudiante.post@edu.co";
            Guid estudianteId;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                if (!db.Usuarios.Any(u => u.Email == "integration.asesor@edu.co"))
                {
                    db.Usuarios.Add(new Usuario(Guid.NewGuid(), "Asesor Integration", "integration.asesor@edu.co", BCrypt.Net.BCrypt.HashPassword("Password123*"), 1, DateTime.UtcNow));
                }

                Usuario usuarioEst;
                if (!db.Usuarios.Any(u => u.Email == estudianteEmail))
                {
                    usuarioEst = new Usuario(Guid.NewGuid(), "Estudiante Post", estudianteEmail, BCrypt.Net.BCrypt.HashPassword("Password123*"), 2, DateTime.UtcNow);
                    db.Usuarios.Add(usuarioEst);
                    await db.SaveChangesAsync();
                }
                else
                {
                    usuarioEst = db.Usuarios.First(u => u.Email == estudianteEmail);
                }

                var tipoDocId = db.TiposDocumento.Select(t => t.Id).First();
                var progId = db.ProgramasAcademicos.Select(p => p.Id).First();

                var estudiante = db.Estudiantes.FirstOrDefault(e => e.UsuarioId == usuarioEst.Id)
                                ?? new Estudiante(Guid.NewGuid(), usuarioEst.Id, tipoDocId, "222333444", progId, 1);
                if (!db.Estudiantes.Any(e => e.UsuarioId == usuarioEst.Id))
                {
                    db.Estudiantes.Add(estudiante);
                    await db.SaveChangesAsync();
                }

                estudianteId = estudiante.Id;
            }

            // Login como estudiante
            var loginDto = new LoginDto { Email = estudianteEmail, Password = "Password123*" };
            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
            loginResult.Should().NotBeNull();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult!.Token);

            // Preparar registro
            int tipoApoyoId;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                tipoApoyoId = db.TiposApoyo.Select(t => t.Id).First();
            }

            var registro = new RegistroSolicitudDto
            {
                EstudianteId = estudianteId,
                TipoApoyoId = tipoApoyoId,
                MontoSolicitado = 50000,
                Descripcion = "Solicitud integración POST válida",
                EstadoId = 1
            };

            var response = await _client.PostAsJsonAsync("/api/Solicitudes", registro);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verificar en BD
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var created = db.SolicitudesApoyo.FirstOrDefault(s => s.EstudianteId == estudianteId && s.Descripcion.Contains("Solicitud integración POST válida"));
                created.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task Patch_DeberiaActualizarEstado_CuandoUsuarioEsAsesor()
        {
            // Preparar datos: asesor, estudiante y solicitud
            Guid solicitudId;
            Guid asesorId;
            var estudianteEmail = "integration.estudiante.patch@edu.co";
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                if (!db.Usuarios.Any(u => u.Email == "integration.asesor@edu.co"))
                {
                    db.Usuarios.Add(new Usuario(Guid.NewGuid(), "Asesor Integration", "integration.asesor@edu.co", BCrypt.Net.BCrypt.HashPassword("Password123*"), 1, DateTime.UtcNow));
                }

                Usuario usuarioEst;
                if (!db.Usuarios.Any(u => u.Email == estudianteEmail))
                {
                    usuarioEst = new Usuario(Guid.NewGuid(), "Estudiante Patch", estudianteEmail, BCrypt.Net.BCrypt.HashPassword("Password123*"), 2, DateTime.UtcNow);
                    db.Usuarios.Add(usuarioEst);
                    await db.SaveChangesAsync();
                }
                else
                {
                    usuarioEst = db.Usuarios.First(u => u.Email == estudianteEmail);
                }

                var tipoApoyoId = db.TiposApoyo.Select(t => t.Id).First();
                var tipoDocId = db.TiposDocumento.Select(t => t.Id).First();
                var progId = db.ProgramasAcademicos.Select(p => p.Id).First();
                asesorId = db.Usuarios.First(u => u.Email == "integration.asesor@edu.co").Id;

                var estudiante = db.Estudiantes.FirstOrDefault(e => e.UsuarioId == usuarioEst.Id)
                                ?? new Estudiante(Guid.NewGuid(), usuarioEst.Id, tipoDocId, "777666555", progId, 1);
                if (!db.Estudiantes.Any(e => e.UsuarioId == usuarioEst.Id))
                {
                    db.Estudiantes.Add(estudiante);
                    await db.SaveChangesAsync();
                }

                var solicitud = new SolicitudApoyo(Guid.NewGuid(), estudiante.Id, tipoApoyoId, 10000, "Solicitud integración PATCH", 1, DateTime.UtcNow, DateTime.UtcNow, asesorId);
                db.SolicitudesApoyo.Add(solicitud);
                await db.SaveChangesAsync();
                solicitudId = solicitud.Id;
            }

            // Login como asesor
            var loginDto = new LoginDto { Email = "integration.asesor@edu.co", Password = "Password123*" };
            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
            loginResult.Should().NotBeNull();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult!.Token);

            // Preparar patch
            var patchDto = new ActualizarEstadoSolicitudDto
            {
                EstadoId = 2,
                UsuarioId = asesorId
            };

            var response = await _client.PatchAsJsonAsync($"/api/Solicitudes/{solicitudId}/estado", patchDto);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verificar en BD
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var updated = db.SolicitudesApoyo.FirstOrDefault(s => s.Id == solicitudId);
                updated.Should().NotBeNull();
                updated!.EstadoSolicitudId.Should().Be(2);
            }
        }
    }
}