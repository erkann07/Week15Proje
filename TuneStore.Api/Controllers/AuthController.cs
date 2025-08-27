using Microsoft.AspNetCore.Mvc;
using TuneStore.Application.Abstractions;
using TuneStore.Application.DTOs;

namespace TuneStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) => _auth = auth;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req) => Ok(await _auth.RegisterAsync(req));

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req) => Ok(await _auth.LoginAsync(req));
    }
}
