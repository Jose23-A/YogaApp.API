using Microsoft.AspNetCore.Mvc;
using YogaApp.API.Services;

namespace YogaApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        // Le inyectamos el servicio que fabrica las llaves
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UserDto request)
        {
            try
            {
                // El servicio valida la clave y genera el Token JWT
                var tokenJwt = await _authService.LoginAsync(request.Email, request.Password);

                // AHORA SÍ: Devolvemos un JSON que dice {"token": "eyJhbG..."}
                return Ok(new { token = tokenJwt });
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] UserDto request)
        {
            try
            {
                var student = await _authService.RegisterAsync(request.Email, request.Password);
                return Ok(student);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class UserDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}