using DadJokeAPI.Models;
using DadJokeConsoleApi.Services;
using DadJokeConsoleApi.Utils;
using Microsoft.AspNetCore.Mvc;

namespace DadJokeConsoleApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly JwtTokenGenerator _tokenGenerator;
        private readonly UserService _userService;

        public AuthController(JwtTokenGenerator tokenGenerator, UserService userService)
        {
            _tokenGenerator = tokenGenerator;
            _userService = userService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            var success = _userService.Register(user);
            if (!success)
                return BadRequest("Username already exists.");

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            var valid = _userService.ValidateUser(user.Username, user.Password);
            if (!valid)
                return Unauthorized("Invalid credentials");

            var token = _tokenGenerator.GenerateToken(user.Username);
            return Ok(new { token });
        }
    }
}
