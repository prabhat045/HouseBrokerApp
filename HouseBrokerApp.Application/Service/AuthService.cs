using HouseBrokerApp.Application.DTO;
using HouseBrokerApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseBrokerApp.Application.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task RegisterAsync(UserRegisterDto registerDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
                throw new Exception("User already exists");

            var user = new ApplicationUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Email,
                FullName = registerDto.FullName,
                IsBroker = registerDto.IsBroker,
                PhoneNumber = registerDto.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password).ConfigureAwait(false);
            if (!result.Succeeded)
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

            await CreateRole().ConfigureAwait(false);

            await InsertRole(user).ConfigureAwait(false);

        }

        private async Task InsertRole(ApplicationUser user)
        {
            if (user.IsBroker)
                await _userManager.AddToRoleAsync(user, "Broker");
            else
                await _userManager.AddToRoleAsync(user, "HouseSeeker");
        }

        private async Task CreateRole()
        {
            if (!await _roleManager.RoleExistsAsync("Broker"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Broker"));
            }
            if (!await _roleManager.RoleExistsAsync("HouseSeeker"))
            {
                await _roleManager.CreateAsync(new IdentityRole("HouseSeeker"));
            }
        }

        public async Task<UserDto> LoginAsync(UserLoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || user.Email == null)
                throw new Exception("Invalid credentials");

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result)
                throw new Exception("Invalid credentials");

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName
            };
        }

    }
}
