using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YogaApp.API;
using YogaApp.API.Entities;

namespace YogaApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly YogaDbContext _context;

        public AuthController(YogaDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            // 0. Verificamos si el usuario ya existe (Validación básica)
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("Este correo ya está registrado.");
            }
            // 1. Encriptar la contraseña usando BCrypt
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 2. Crear el objeto Usuario
            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = "Alumno" // Por defecto, todos los nuevos usuarios son "Alumno"
            };

            // 3. Guardar en la base de datos
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }
    }

    public class UserDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
