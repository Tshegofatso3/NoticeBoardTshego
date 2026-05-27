using Microsoft.AspNetCore.Mvc;
using NoticeBoardApi.DTOs;
using NoticeBoardApi.Services;

namespace NoticeBoardApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService auth) : ControllerBase
    {
        private readonly IAuthService _auth = auth;

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var token = _auth.Login(request.Username, request.Password);
            if (token is null)
                return Unauthorized(new { message = "Invalid username or password." });
            return Ok(new { token });
        }
    }
}