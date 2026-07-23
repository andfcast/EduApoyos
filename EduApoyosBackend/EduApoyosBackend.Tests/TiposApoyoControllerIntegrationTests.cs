using System;
using System.Collections.Generic;
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
    public class TiposApoyoControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly System.Net.Http.HttpClient _client;

        public TiposApoyoControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_DeberiaRetornarUnauthorized_CuandoNoSeProveeToken()
        {
            // Act
            var response = await _client.GetAsync("/api/TiposApoyo");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Get_DeberiaRetornarOkYLista_DeCuandoSeConsultaConTokenValido()
        {
            // 1) Asegurar existencia de un usuario de prueba en la BD del servidor de pruebas
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var existing = dbContext.Usuarios.FirstOrDefault(u => u.Email == "integration.tiposapoyo@edu.co");
                if (existing == null)
                {
                    var usuarioPrueba = new Usuario(Guid.NewGuid(), "Integration TiposApoyo", "integration.tiposapoyo@edu.co", BCrypt.Net.BCrypt.HashPassword("Password123*"), 2, DateTime.UtcNow);
                    dbContext.Usuarios.Add(usuarioPrueba);
                    await dbContext.SaveChangesAsync();
                }
            }

            // 2) Login para obtener token
            var loginDto = new LoginDto
            {
                Email = "integration.tiposapoyo@edu.co",
                Password = "Password123*"
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
            loginResult.Should().NotBeNull();
            loginResult!.Token.Should().NotBeNullOrEmpty();

            // 3) Llamar al endpoint con el token en la cabecera Authorization
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Token);

            var response = await _client.GetAsync("/api/TiposApoyo");

            // 4) Aserciones sobre la respuesta
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var tipos = await response.Content.ReadFromJsonAsync<IEnumerable<TipoApoyoDto>>();
            tipos.Should().NotBeNull();
            tipos!.Select(t => t.Nombre).Should().Contain(new[] { "Económico", "Alimentario", "Transporte" });
        }
    }
}