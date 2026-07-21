using EduApoyosBackend.Domain.Entities;
using EduApoyosBackend.Domain.Repositories;
using EduApoyosBackend.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Infrastructure.Persistence.Repositories
{
    public class TipoApoyoRepository : GenericRepository<TipoApoyo>, ITipoApoyoRepository
    {
        public TipoApoyoRepository(AppDbContext context) : base(context) { }
    }
}
