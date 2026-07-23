using EduApoyosBackend.API.Middlewares;
using EduApoyosBackend.Application.Interfaces;
using EduApoyosBackend.Application.Services;
using EduApoyosBackend.Domain.Repositories;
using EduApoyosBackend.Infrastructure.Persistence;
using EduApoyosBackend.Infrastructure.Persistence.Context;
using EduApoyosBackend.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

namespace EduApoyosBackend.API
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Configurar el motor de Logging nativo para escribir en Consola y flujos de Debug
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            var myAllowSpecificOrigins = "_myAllowSpecificOrigins";

            // 2. Registrar el servicio CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: myAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.WithOrigins(
                                                "http://localhost:4200",
                                                "http://localhost:8080")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                                  });
            });
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("EduApoyosBackend.Infrastructure"))); // Las migraciones se registran desde la API
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
            builder.Services.AddScoped<ITokenService, JwtTokenService>();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IRolService, RolService>();
            builder.Services.AddScoped<IUsuarioService, UsuarioService>();
            builder.Services.AddScoped<IEstudianteService, EstudianteService>();
            builder.Services.AddScoped<ITipoDocumentoService, TipoDocumentoService>();
            builder.Services.AddScoped<IProgramaAcademicoService, ProgramaAcademicoService>();
            builder.Services.AddScoped<ITipoApoyoService, TipoApoyoService>();
            builder.Services.AddScoped<IEstadoSolicitudService, EstadoSolicitudService>();
            builder.Services.AddScoped<ISolicitudService, SolicitudService>();

            builder.Services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
            });
            builder.Services.AddAuthorization();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EduApoyos API", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                // Habilita el botón "Authorize" con el candado para enviar el Token JWT
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Ingresa tu token JWT en este formato: Bearer {tu_token}"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EduApoyos API v1");
                });
            }

            app.UseHttpsRedirection();
            app.UseCors(myAllowSpecificOrigins);
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
