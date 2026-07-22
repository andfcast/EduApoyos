using EduApoyosBackend.Domain.Entities;
using EduApoyosBackend.Domain.Repositories;
using EduApoyosBackend.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Infrastructure.Persistence.Repositories
{
    public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(AppDbContext context) : base(context) { }
        public async Task<Usuario?> ObtenerPorCorreoAsync(string email) {
            return await _context.Usuarios.Include(s =>s.Rol)
            .FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<bool> ExisteUsuarioConCorreoAsync(string email)
        {
            return await _context.Usuarios.AnyAsync(u => u.Email == email);
        }        
    }
}
