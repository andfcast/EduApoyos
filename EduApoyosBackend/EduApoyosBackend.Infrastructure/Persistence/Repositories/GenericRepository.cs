using EduApoyosBackend.Domain.Repositories;
using EduApoyosBackend.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using EduApoyosBackend.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;  

        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<T?> ObtenerPorIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> ListarAsync()
        {
            // Si el repositorio se instancia para SolicitudApoyo, cargamos explícitamente
            // las relaciones necesarias para evitar NullReference al proyectar DTOs.
            if (typeof(T) == typeof(SolicitudApoyo))
            {
                var lista = await _context.Set<SolicitudApoyo>()
                    .Include(s => s.EstadoSolicitud)
                    .Include(s => s.TipoApoyo)
                    .Include(s => s.Estudiante)
                        .ThenInclude(e => e.Usuario)
                    .Include(s => s.Asesor)
                    .ToListAsync();

                return lista.Cast<T>();
            }

            return await _context.Set<T>().ToListAsync();
        }

        public IEnumerable<T> Buscar(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Where(expression);
        }

        public async Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> predicado)
        {
            return await _context.Set<T>().AsNoTracking()
                               .Where(predicado)
                               .ToListAsync();
        }

        public async Task AgregarAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public void Actualizar(T entity)
        {
            // EF Core rastrea los cambios de la entidad automáticamente en memoria.
            // Solo marcamos el estado como Modificado.
            _context.Set<T>().Update(entity);
        }

        public void Borrar(T entity)
        {
            // Remueve la entidad del tracking y la marca para eliminación física en BD.
            _context.Set<T>().Remove(entity);
        }

        public async Task<T?> ObtenerPorGuidAsync(Guid guid)
        {
            return await _context.Set<T>().FindAsync(guid);
        }        
    }
}
