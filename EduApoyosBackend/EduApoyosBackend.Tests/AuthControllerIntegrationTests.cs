using EduApoyosBackend.API;
using EduApoyosBackend.Application.DTOs;
using EduApoyosBackend.Domain.Entities;
using EduApoyosBackend.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
namespace EduApoyosBackend.Tests
{
    public class AuthControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<EduApoyosBackend.API.Program> _factory;
        private readonly HttpClient _client;

        // Al heredar o pasar el WebApplicationFactory, aseguramos que levante el contexto de la API
        public AuthControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }
        [Fact]
        public async Task Login_DeberiaRetornarUnauthorized_CuandoUsuarioNoExiste()
        {
            // Arrange
            var dto = new LoginDto
            {
                Email = "no.existo@edu.co",
                Password = "Password123*"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", dto);

            // Assert
            // El servicio lanzará UnauthorizedAccessException y el Exception Handler lo transformará en Unauthorized (401)
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            var contenido = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            contenido.Should().ContainKey("mensaje");
            contenido!["mensaje"].ToString().Should().Be("Credenciales inválidas.");
        }

        [Fact]
        public async Task Login_DeberiaRetornarUnauthorized_CuandoContrasenaEsIncorrecta()
        {
            // Arrange: Supongamos que este usuario sí existe, pero mandamos clave errónea
            var dto = new LoginDto
            {
                Email = "carlos@edu.co",
                Password = "ClaveFalsaIncorrecta"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            var contenido = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            contenido.Should().ContainKey("mensaje");
            contenido!["mensaje"].ToString().Should().Be("Credenciales inválidas.");
        }

        [Fact]
        public async Task Login_DeberiaRetornarOkYToken_CuandoCredencialesSonValidas()
        {
            // 1. Arrange: Asegurarnos de que el usuario existe en la base de datos de pruebas
            // Usamos el contenedor de servicios de la API para sembrar un usuario válido de prueba
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Verificamos si ya existe para evitar duplicados si se corre varias veces
                var existingUser = dbContext.Usuarios.FirstOrDefault(u => u.Email == "test.usuario@edu.co");
                if (existingUser == null)
                {
                    var usuarioPrueba = new Usuario(Guid.NewGuid(), "Usuario Test", "test.usuario@edu.co", BCrypt.Net.BCrypt.HashPassword("Password123*"), 2, DateTime.UtcNow);                        
                    dbContext.Usuarios.Add(usuarioPrueba);
                    await dbContext.SaveChangesAsync();
                }
            }

            var loginDto = new LoginDto
            {
                Email = "test.usuario@edu.co",
                Password = "Password123*"
            };

            // 2. Act: Realizamos la petición POST al endpoint de login
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

            // 3. Assert: Validamos que la respuesta sea exitosa (200 OK) y retorne el token
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        
            var resultado = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            resultado.Token.Should().NotBeNullOrEmpty();
        }
    }
}
