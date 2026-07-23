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
    public class EstudiantesControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly System.Net.Http.HttpClient _client;

        public EstudiantesControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_DeberiaRetornarUnauthorized_CuandoNoSeProveeToken()
        {
            var response = await _client.GetAsync("/api/Estudiantes");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetSolicitudes_DeberiaRetornarLista_CuandoEstudianteTieneSolicitudes()
        {
            // Arrange: sembrar datos necesarios
            Guid usuarioId;
            Guid estudianteId;
            var passwordPlain = "Password123*";
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Asegurar que exista el rol "Estudiante" (no duplicar si ya está)
                if (!db.Roles.Any(r => r.Id == 1))
                {
                    db.Roles.Add(new Rol(1, "Asesor"));
                }
                if (!db.Roles.Any(r => r.Id == 2))
                {
                    db.Roles.Add(new Rol(2, "Estudiante"));
                }

                // Asegurar que exista un tipo de apoyo con id 1
                if (!db.TiposApoyo.Any(t => t.Id == 1))
                {
                    db.TiposApoyo.Add(new TipoApoyo(1, "Económico", "Apoyo financiero mensual"));
                }

                // Asegurar estado 1 (Pendiente) exista (OnModelCreating normalmente lo inserta)
                if (!db.EstadosSolicitud.Any(e => e.Id == 1))
                {
                    db.EstadosSolicitud.Add(new EstadoSolicitud(1, "Pendiente", "Radicada"));
                }

                // Crear usuario de prueba si no existe
                var email = "estudiante.integration@edu.co";
                var existing = db.Usuarios.FirstOrDefault(u => u.Email == email);
                Usuario usuario;
                if (existing == null)
                {
                    usuario = new Usuario(Guid.NewGuid(), "Estudiante Test", email, BCrypt.Net.BCrypt.HashPassword(passwordPlain), 2, DateTime.UtcNow);
                    db.Usuarios.Add(usuario);
                }
                else
                {
                    usuario = existing;
                }
                usuarioId = usuario.Id;
                var emailAsesor = "asesor.integration@edu.co";
                Usuario asesor = new Usuario(Guid.NewGuid(), "Asesor Test", emailAsesor, BCrypt.Net.BCrypt.HashPassword(passwordPlain), 1, DateTime.UtcNow);
                db.Usuarios.Add(asesor);

                // Crear estudiante si no existe
                var existingEst = db.Estudiantes.FirstOrDefault(e => e.UsuarioId == usuarioId);
                Estudiante estudiante;
                if (existingEst == null)
                {
                    estudiante = new Estudiante(Guid.NewGuid(), usuarioId, 1, "00000000", 1, 1);
                    db.Estudiantes.Add(estudiante);
                }
                else
                {
                    estudiante = existingEst;
                }
                estudianteId = estudiante.Id;

                // Crear una solicitud ligada al estudiante (si ya existe una similar evitar duplicar)
                if (!db.SolicitudesApoyo.Any(s => s.EstudianteId == estudianteId && s.Descripcion.Contains("Solicitud integración")))
                {
                    var solicitud = new SolicitudApoyo(Guid.NewGuid(), estudianteId, 1, 500.0, "Solicitud integración - prueba", 1, DateTime.UtcNow, DateTime.UtcNow, asesor.Id);
                    db.SolicitudesApoyo.Add(solicitud);

                    // Crear historial asociado
                    var historial = new HistorialEstado(Guid.NewGuid(), solicitud.Id, null, solicitud.EstadoSolicitudId, DateTime.UtcNow, usuarioId, "Registro inicial - prueba integración");
                    db.HistorialesEstados.Add(historial);
                }

                await db.SaveChangesAsync();
            }

            // 1) Hacer login para obtener token (debe incluir rol "Estudiante")
            var loginDto = new LoginDto { Email = "estudiante.integration@edu.co", Password = passwordPlain };
            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
            loginResult!.Token.Should().NotBeNullOrEmpty();

            // 2) Llamar al endpoint protegido usando el token
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Token);

            // Nota: el servicio acepta tanto Estudiante.Id como Usuario.Id; se prueba con Usuario.Id
            var response = await _client.GetAsync($"/api/estudiantes/{usuarioId}/solicitudes");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var solicitudes = await response.Content.ReadFromJsonAsync<SolicitudApoyoDto[]>();
            solicitudes.Should().NotBeNull();
            solicitudes!.Length.Should().BeGreaterThanOrEqualTo(1);
            solicitudes.Any(s => s.Descripcion.Contains("Solicitud integración")).Should().BeTrue();
        }

        [Fact]
        public async Task Post_DeberiaRegistrarEstudiante_CuandoUsuarioEsAsesor()
        {
            // Asegurar asesor
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                if (!db.Usuarios.Any(u => u.Email == "integration.asesor@edu.co"))
                {
                    var asesor = new Usuario(Guid.NewGuid(), "Asesor Integration", "integration.asesor@edu.co", BCrypt.Net.BCrypt.HashPassword("Password123*"), 1, DateTime.UtcNow);
                    db.Usuarios.Add(asesor);
                    await db.SaveChangesAsync();
                }
            }

            // Login as asesor
            var loginDto = new LoginDto { Email = "integration.asesor@edu.co", Password = "Password123*" };
            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
            loginResult.Should().NotBeNull();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult!.Token);

            // Prepare registro
            var newEmail = $"new.student.{Guid.NewGuid():N}@edu.co";
            var docNum = DateTime.UtcNow.Ticks.ToString().Substring(0,12);
            var registro = new RegistroEstudianteDto
            {
                Id = Guid.NewGuid(),
                NombreCompleto = "Nuevo Estudiante",
                Email = newEmail,
                Password = "Password123*",
                TipoDocumentoId = 1,
                NumeroDocumento = docNum,
                ProgramaAcademicoId = 1,
                Semestre = 2,
                Activo = true
            };

            var response = await _client.PostAsJsonAsync("/api/Estudiantes", registro);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Verificar en BD que el usuario/estudiante fue creado
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var usuario = db.Usuarios.FirstOrDefault(u => u.Email == newEmail);
                usuario.Should().NotBeNull();
                var estudiante = db.Estudiantes.FirstOrDefault(e => e.UsuarioId == usuario!.Id);
                estudiante.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task GetSolicitudes_DeberiaRetornarOkYColeccion_CuandoUsuarioEsEstudiante()
        {
            // Seed estudiante con solicitud
            Guid estudianteId;
            var estudianteEmail = "integration.estudiante.solicitudes@edu.co";
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Crear asesor para asignar en solicitud
                if (!db.Usuarios.Any(u => u.Email == "integration.asesor@edu.co"))
                {
                    db.Usuarios.Add(new Usuario(Guid.NewGuid(), "Asesor Integration", "integration.asesor@edu.co", BCrypt.Net.BCrypt.HashPassword("Password123*"), 1, DateTime.UtcNow));
                    await db.SaveChangesAsync();
                }
                var asesorId = db.Usuarios.First(u => u.Email == "integration.asesor@edu.co").Id;

                // Crear usuario estudiante y entidad Estudiante si no existe
                Usuario usuarioEst;
                if (!db.Usuarios.Any(u => u.Email == estudianteEmail))
                {
                    usuarioEst = new Usuario(Guid.NewGuid(), "Estudiante Solicitudes", estudianteEmail, BCrypt.Net.BCrypt.HashPassword("Password123*"), 2, DateTime.UtcNow);
                    db.Usuarios.Add(usuarioEst);
                    await db.SaveChangesAsync();
                }
                else
                {
                    usuarioEst = db.Usuarios.First(u => u.Email == estudianteEmail);
                }

                var tipoDocId = db.TiposDocumento.Select(t => t.Id).First();
                var progId = db.ProgramasAcademicos.Select(p => p.Id).First();

                Estudiante estudiante = db.Estudiantes.FirstOrDefault(e => e.UsuarioId == usuarioEst.Id)
                    ?? new Estudiante(Guid.NewGuid(), usuarioEst.Id, tipoDocId, "555444333", progId, 1);

                if (!db.Estudiantes.Any(e => e.UsuarioId == usuarioEst.Id))
                {
                    db.Estudiantes.Add(estudiante);
                    await db.SaveChangesAsync();
                }

                estudianteId = estudiante.Id;

                // Crear solicitud
                if (!db.SolicitudesApoyo.Any(s => s.EstudianteId == estudianteId && s.Descripcion == "Solicitud integración"))
                {
                    var solicitud = new SolicitudApoyo(Guid.NewGuid(), estudianteId, db.TiposApoyo.Select(t => t.Id).First(), 150000, "Solicitud integración", 1, DateTime.UtcNow, DateTime.UtcNow, asesorId);
                    db.SolicitudesApoyo.Add(solicitud);
                    await db.SaveChangesAsync();
                }
            }

            // Login as estudiante
            var loginDto = new LoginDto { Email = estudianteEmail, Password = "Password123*" };
            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
            loginResult.Should().NotBeNull();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult!.Token);

            // Act
            var response = await _client.GetAsync($"/api/Estudiantes/{estudianteId}/solicitudes");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var solicitudes = await response.Content.ReadFromJsonAsync<SolicitudApoyoDto[]>();
            solicitudes.Should().NotBeNull();
            solicitudes!.Select(s => s.Descripcion).Should().Contain(d => d.Contains("Solicitud integración"));
        }
    }
}