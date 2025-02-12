using Xunit;
using Moq;
using HouseBrokerApp.Application.Services;
using HouseBrokerApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using HouseBrokerApp.Application.DTO;
using HouseBrokerApp.Application.Service;

namespace HouseBrokerApp.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly IAuthService _authService;
        private List<ApplicationUser> _users;

        public AuthServiceTests()
        {
            _users = new List<ApplicationUser>();
            _userManagerMock = GetMockUserManager(_users);
            _roleManagerMock = GetMockRoleManager();
            _authService = new AuthService(_userManagerMock.Object, _roleManagerMock.Object);
        }

        private static Mock<UserManager<ApplicationUser>> GetMockUserManager(List<ApplicationUser> users)
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mgr = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);
            mgr.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
               .ReturnsAsync(IdentityResult.Success)
               .Callback<ApplicationUser, string>((user, password) =>
               {
                   user.Id = Guid.NewGuid().ToString();
                   users.Add(user);
               });
            mgr.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
               .ReturnsAsync((string email) => users.FirstOrDefault(u => u.Email == email));
            mgr.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
               .ReturnsAsync(true);
            mgr.Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>()))
               .ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
               .ReturnsAsync(IdentityResult.Success);
            return mgr;
        }

        private static Mock<RoleManager<IdentityRole>> GetMockRoleManager()
        {
            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            var roleMgr = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);
            roleMgr.Setup(x => x.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            roleMgr.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);
            return roleMgr;
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowException_WhenUserAlreadyExists()
        {
            var existingUser = new ApplicationUser
            {
                Id = "existingUser",
                Email = "duplicate@example.com",
                FullName = "Existing User"
            };
            _users.Add(existingUser);

            var registerDto = new UserRegisterDto
            {
                Email = "duplicate@example.com",
                FullName = "New User",
                Password = "Password123!",
                IsBroker = true,
                PhoneNumber = "1234567890"
            };

            await Assert.ThrowsAsync<Exception>(() => _authService.RegisterAsync(registerDto));
        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateUserSuccessfully()
        {
            var registerDto = new UserRegisterDto
            {
                Email = "newuser@example.com",
                FullName = "New User",
                Password = "Password123!",
                IsBroker = true,
                PhoneNumber = "1234567890"
            };

            
            await _authService.RegisterAsync(registerDto).ConfigureAwait(false);

            
            var createdUser = _users.FirstOrDefault(u => u.Email == "newuser@example.com");
            Assert.NotNull(createdUser);
            Assert.False(string.IsNullOrEmpty(createdUser.Id));
            Assert.Equal("newuser@example.com", createdUser.Email);
            Assert.Equal("New User", createdUser.FullName);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowException_WhenUserNotFound()
        {
            
            var loginDto = new UserLoginDto
            {
                Email = "nonexistent@example.com",
                Password = "Password123!"
            };

            
            await Assert.ThrowsAsync<Exception>(() => _authService.LoginAsync(loginDto));
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnUserDto_WhenCredentialsAreValid()
        {
           
            var user = new ApplicationUser
            {
                Id = "loginUser",
                Email = "login@example.com",
                FullName = "Login User"
            };
            _users.Add(user);

            _userManagerMock.Setup(x => x.FindByEmailAsync("login@example.com"))
                            .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, "Password123!"))
                            .ReturnsAsync(true);

            var loginDto = new UserLoginDto
            {
                Email = "login@example.com",
                Password = "Password123!"
            };

            var result = await _authService.LoginAsync(loginDto);

            
            Assert.NotNull(result);
            Assert.Equal("loginUser", result.Id);
            Assert.Equal("login@example.com", result.Email);
            Assert.Equal("Login User", result.FullName);
        }
    }
}
