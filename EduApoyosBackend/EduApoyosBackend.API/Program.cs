
using EduApoyosBackend.Application.Interfaces;
using EduApoyosBackend.Application.Services;
using EduApoyosBackend.Domain.Repositories;
using EduApoyosBackend.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace EduApoyosBackend.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Configurar el motor de Logging nativo para escribir en Consola y flujos de Debug
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
            builder.Services.AddScoped<ITokenService, JwtTokenService>();

            builder.Services.AddScoped<IAuthService, AuthService>();                  
            builder.Services.AddScoped<IRolService, RolService>();
            builder.Services.AddScoped<IUsuarioService, UsuarioService>();
            builder.Services.AddScoped<IEstudianteService, EstudianteService>();
            //builder.Services.AddScoped<ITipoDocumentoService, TipoDocumentoService>();
            //builder.Services.AddScoped<ITipoApoyoService, TipoApoyoService>();
            //builder.Services.AddScoped<IEstadoSolicitudService, EstadoSolicitudService>();
            //builder.Services.AddScoped<ISolicitudApoyoService, SolicitudApoyoService>();
            //builder.Services.AddScoped<IHistorialEstadoService, HistorialEstadoService>();


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
