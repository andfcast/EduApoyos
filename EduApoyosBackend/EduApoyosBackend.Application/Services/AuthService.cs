using EduApoyosBackend.Application.DTOs;
using EduApoyosBackend.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyosBackend.Application.Services
{
    public class AuthService : IAuthService
    {
        public async Task<LoginResponseDto> Login(LoginRequestDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
