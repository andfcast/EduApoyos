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
        public async Task GetSolicitudes_DeberiaRetornarPaginado_CuandoHaySolicitudesYUsuarioEsAsesor()
        {
            var asesorEmail = "asesor.integration@edu.co";
            var estudianteEmail = "estudiante.integration2@edu.co";
            var asesorPassword = "AsesorPass1!";
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Asegurar roles y datos base
            if (!db.Roles.Any(r => r.Id == 1)) db.Roles.Add(new Rol(1, "Asesor"));
            if (!db.Roles.Any(r => r.Id == 2)) db.Roles.Add(new Rol(2, "Estudiante"));
            if (!db.TiposApoyo.Any(t => t.Id == 1)) db.TiposApoyo.Add(new TipoApoyo(1, "Económico", "Desc"));
            if (!db.EstadosSolicitud.Any(e => e.Id == 1)) db.EstadosSolicitud.Add(new EstadoSolicitud(1, "Pendiente", "Radicada"));
            await db.SaveChangesAsync();

            // Crear o actualizar usuario asesor garantizando el Hash del password actual
            Usuario asesor;
            var existingAsesor = db.Usuarios.FirstOrDefault(u => u.Email == asesorEmail);
            if (existingAsesor == null)
            {
                asesor = new Usuario(Guid.NewGuid(), "Asesor Integration", asesorEmail, BCrypt.Net.BCrypt.HashPassword(asesorPassword), 1, DateTime.UtcNow);
                db.Usuarios.Add(asesor);
            }
            else
            {
                asesor = existingAsesor;                
                asesor.PasswordHash = BCrypt.Net.BCrypt.HashPassword(asesorPassword);
            }

            // Crear usuario estudiante
            Usuario usuarioEstudiante;
            var existingUserEst = db.Usuarios.FirstOrDefault(u => u.Email == estudianteEmail);
            if (existingUserEst == null)
            {
                usuarioEstudiante = new Usuario(Guid.NewGuid(), "Estudiante Integration 2", estudianteEmail, BCrypt.Net.BCrypt.HashPassword("PassEst1!"), 2, DateTime.UtcNow);
                db.Usuarios.Add(usuarioEstudiante);
            }
            else usuarioEstudiante = existingUserEst;

            await db.SaveChangesAsync();

            // Crear estudiante si no existe
            Estudiante estudiante;
            var existingEst = db.Estudiantes.FirstOrDefault(e => e.UsuarioId == usuarioEstudiante.Id);
            if (existingEst == null)
            {
                estudiante = new Estudiante(Guid.NewGuid(), usuarioEstudiante.Id, 1, "99999999", 1, 1);
                db.Estudiantes.Add(estudiante);
            }
            else estudiante = existingEst;

            // Agregar solicitudes para el estudiante
            if (!db.SolicitudesApoyo.Any(s => s.EstudianteId == estudiante.Id && s.Descripcion.Contains("Integración Asesor")))
            {
                for (int i = 0; i < 3; i++)
                {
                    var sol = new SolicitudApoyo(Guid.NewGuid(), estudiante.Id, 1, 100 + i, $"Solicitud Integración Asesor #{i}", 1, DateTime.UtcNow.AddDays(-i), DateTime.UtcNow.AddDays(-i), asesor.Id);
                    db.SolicitudesApoyo.Add(sol);
                    db.HistorialesEstados.Add(new HistorialEstado(Guid.NewGuid(), sol.Id, null, sol.EstadoSolicitudId, DateTime.UtcNow, asesor.Id, "Registro inicial"));
                }
            }

            await db.SaveChangesAsync();

            // 1) Login como Asesor para obtener JWT
            var loginDto = new LoginDto { Email = asesorEmail, Password = asesorPassword };
            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
            loginResult!.Token.Should().NotBeNullOrEmpty();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Token);

            // 2) Act: llamar GET /api/solicitudes con paginación
            var response = await _client.GetAsync("/api/solicitudes?pagina=1&tamanoPagina=2");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var paginado = await response.Content.ReadFromJsonAsync<RespuestaPaginadaDto<SolicitudApoyoDto>>();
            paginado.Should().NotBeNull();
            paginado!.Elementos.Should().NotBeNull();
            paginado.Elementos.Count.Should().BeLessThanOrEqualTo(2);
            paginado.TotalRegistros.Should().BeGreaterThanOrEqualTo(3);
            paginado.Elementos.Any(e => e.Descripcion.Contains("Integración Asesor")).Should().BeTrue();
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