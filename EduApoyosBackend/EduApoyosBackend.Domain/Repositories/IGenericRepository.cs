using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Domain.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        // Operaciones de Lectura Asíncronas
        Task<T?> ObtenerPorIdAsync(int id);
        Task<T?> ObtenerPorGuidAsync(Guid id);
        Task<IEnumerable<T>> ListarAsync();

        // Consultas personalizadas usando expresiones Lambda (se evalúan en diferido)
        IEnumerable<T> Buscar(Expression<Func<T, bool>> expression);

        // Operaciones de Escritura
        Task AgregarAsync(T entity);
        void Actualizar(T entity);
        void Borrar(T entity);
    }
}
