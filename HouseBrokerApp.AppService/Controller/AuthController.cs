using HouseBrokerApp.Application.DTO;
using HouseBrokerApp.Application.Service;
using HouseBrokerApp.AppService.Controller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HouseBrokerApp.AppService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IControllerHelper _controllerHelper;
        public AuthController(IAuthService authService,IControllerHelper controllerHelper)
        {
            _authService = authService;
            _controllerHelper = controllerHelper;
           
        }

        [HttpPost("RegisterUser")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto registerDto)
        {
            try
            {
               
                await _authService.RegisterAsync(registerDto).ConfigureAwait(false);
                return Ok("User Created Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("LoginUser")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            try
            {
                var userDto = await _authService.LoginAsync(loginDto).ConfigureAwait(false);
                userDto.Token = _controllerHelper.GenerateJwtToken(userDto.Id,userDto.Email, userDto.FullName);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

       
    }
}
