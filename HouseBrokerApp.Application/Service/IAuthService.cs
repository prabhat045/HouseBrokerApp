using HouseBrokerApp.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseBrokerApp.Application.Service
{
    public interface IAuthService
    {
        Task RegisterAsync(UserRegisterDto registerDto);
        Task<UserDto> LoginAsync(UserLoginDto loginDto);
    }
}
