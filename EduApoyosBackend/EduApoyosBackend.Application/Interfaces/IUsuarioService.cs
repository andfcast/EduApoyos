using EduApoyosBackend.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<string> RegistrarUsuarioAsync(RegistroUsuarioDto dto);
    }
}
