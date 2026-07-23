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
        public async Task Get_DeberiaRetornarOkYLista_CuandoUsuarioEsAsesor()
        {
            // Seed asesor y estudiante
            Guid estudianteId;
            var estudianteEmail = "integration.estudiante.get@edu.co";
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Crear asesor
                if (!db.Usuarios.Any(u => u.Email == "integration.asesor@edu.co"))
                {
                    var asesor = new Usuario(Guid.NewGuid(), "Asesor Integration", "integration.asesor@edu.co", BCrypt.Net.BCrypt.HashPassword("Password123*"), 1, DateTime.UtcNow);
                    db.Usuarios.Add(asesor);
                }

                // Crear usuario estudiante y entidad Estudiante
                if (!db.Usuarios.Any(u => u.Email == estudianteEmail))
                {
                    var usuarioEst = new Usuario(Guid.NewGuid(), "Estudiante Integration", estudianteEmail, BCrypt.Net.BCrypt.HashPassword("Password123*"), 2, DateTime.UtcNow);
                    db.Usuarios.Add(usuarioEst);
                    await db.SaveChangesAsync();

                    var progId = db.ProgramasAcademicos.Select(p => p.Id).First();
                    var tipoDocId = db.TiposDocumento.Select(t => t.Id).First();

                    var estudiante = new Estudiante(Guid.NewGuid(), usuarioEst.Id, tipoDocId, "123456789", progId, 1);
                    estudianteId = estudiante.Id;
                    db.Estudiantes.Add(estudiante);
                }
                else
                {
                    var usuarioEst = db.Usuarios.First(u => u.Email == estudianteEmail);
                    var est = db.Estudiantes.FirstOrDefault(e => e.UsuarioId == usuarioEst.Id);
                    if (est == null)
                    {
                        var progId = db.ProgramasAcademicos.Select(p => p.Id).First();
                        var tipoDocId = db.TiposDocumento.Select(t => t.Id).First();
                        var estudiante = new Estudiante(Guid.NewGuid(), usuarioEst.Id, tipoDocId, "123456789", progId, 1);
                        estudianteId = estudiante.Id;
                        db.Estudiantes.Add(estudiante);
                    }
                    else
                    {
                        estudianteId = est.Id;
                    }
                }

                await db.SaveChangesAsync();
            }

            // Login as asesor
            var loginDto = new LoginDto { Email = "integration.asesor@edu.co", Password = "Password123*" };
            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
            loginResult.Should().NotBeNull();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult!.Token);

            // Act
            var response = await _client.GetAsync("/api/Estudiantes");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var estudiantes = await response.Content.ReadFromJsonAsync<EstudianteDto[]>();
            estudiantes.Should().NotBeNull();
            estudiantes!.Select(e => e.Email).Should().Contain(est => est == estudianteEmail);
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