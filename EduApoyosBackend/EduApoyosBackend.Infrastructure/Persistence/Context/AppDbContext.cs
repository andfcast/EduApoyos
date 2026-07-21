using EduApoyosBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;


namespace EduApoyosBackend.Infrastructure.Persistence.Context
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<TipoDocumento> TiposDocumento { get; set; }

        public DbSet<ProgramaAcademico> ProgramasAcademicos { get; set; }
        public DbSet<TipoApoyo> TiposApoyo { get; set; }
        public DbSet<EstadoSolicitud> EstadosSolicitud { get; set; }
        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<SolicitudApoyo> SolicitudesApoyo { get; set; }
        public DbSet<HistorialEstado> HistorialesEstados { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Rol>(b => {
                b.ToTable("Roles").HasKey(r => r.Id); b.Property(r => r.Id).ValueGeneratedNever();
                b.HasData(new Rol(1, "Asesor"), new Rol(2, "Estudiante"));
            });

            modelBuilder.Entity<TipoDocumento>(b => {
                b.ToTable("TiposDocumento").HasKey(t => t.Id); b.Property(t => t.Id).ValueGeneratedNever();
                b.HasData(new TipoDocumento(1, "CC", "Cédula de Ciudadanía"), new TipoDocumento(2, "TI", "Tarjeta de Identidad"), new TipoDocumento(3, "PS", "Pasaporte")
                    , new TipoDocumento(4, "CE", "Cédula de Extranjería"));
            });

            modelBuilder.Entity<TipoApoyo>(b => {
                b.ToTable("TiposApoyo").HasKey(t => t.Id); b.Property(t => t.Id).ValueGeneratedNever();
                b.HasData(
                    new TipoApoyo(1, "Económico", "Apoyo financiero mensual"),
                    new TipoApoyo(2, "Alimentario", "Acceso a comedores universitarios"),
                    new TipoApoyo(3, "Transporte", "Subsidio de transporte urbano")
                );
            });

            modelBuilder.Entity<ProgramaAcademico>(b => {
                b.ToTable("ProgramasAcademicos").HasKey(p => p.Id); b.Property(p => p.Id).ValueGeneratedNever();
                b.HasData(
                    new ProgramaAcademico(1, "Ingeniería de Sistemas"),
                    new ProgramaAcademico(2, "Medicina"),
                    new ProgramaAcademico(3, "Derecho")
                );
            });

            modelBuilder.Entity<EstadoSolicitud>(b => {
                b.ToTable("EstadosSolicitud").HasKey(e => e.Id); b.Property(e => e.Id).ValueGeneratedNever();
                b.HasData(new EstadoSolicitud(1, "Pendiente", "Radicada"), new EstadoSolicitud(2, "En Revisión", "Validando"), new EstadoSolicitud(3, "Aprobada", "Aceptada"), new EstadoSolicitud(4, "Rechazada", "No cumple requisitos"));
            });

            modelBuilder.Entity<Usuario>(b => {
                b.ToTable("Usuarios").HasKey(u => u.Id);
                b.HasOne(u => u.Rol).WithMany().HasForeignKey(u => u.RolId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Estudiante>(b => {
                b.ToTable("Estudiantes").HasKey(e => e.Id);
                b.Property(e => e.Activo).HasDefaultValue(true);
                b.HasOne(e => e.Usuario).WithOne().HasForeignKey<Estudiante>(e => e.UsuarioId).OnDelete(DeleteBehavior.Cascade);
                b.HasOne(e => e.TipoDocumento).WithMany().HasForeignKey(e => e.TipoDocumentoId).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.ProgramaAcademico).WithMany().HasForeignKey(e => e.ProgramaAcademicoId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SolicitudApoyo>(b => {
                b.ToTable("SolicitudesApoyo").HasKey(s => s.Id);
                b.HasOne(s => s.Estudiante).WithMany(e => e.Solicitudes).HasForeignKey(s => s.EstudianteId).OnDelete(DeleteBehavior.Cascade);
                b.HasOne(s => s.EstadoSolicitud).WithMany().HasForeignKey(s => s.EstadoSolicitudId).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(s => s.TipoApoyo).WithMany().HasForeignKey(s => s.TipoApoyoId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<HistorialEstado>(b => {
                b.ToTable("HistorialesEstados").HasKey(h => h.Id);
                b.HasOne(h => h.Solicitud).WithMany(s => s.HistorialEstados).HasForeignKey(h => h.SolicitudId).OnDelete(DeleteBehavior.Cascade);
                b.HasOne(h => h.EstadoAnterior).WithMany().HasForeignKey(h => h.EstadoAnteriorId).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(h => h.EstadoNuevo).WithMany().HasForeignKey(h => h.EstadoNuevoId).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(h => h.Usuario).WithMany().HasForeignKey(h => h.UsuarioId).OnDelete(DeleteBehavior.Restrict);
                b.Property(h => h.Observacion).HasMaxLength(500).IsRequired();
            });
        }
    }
}
