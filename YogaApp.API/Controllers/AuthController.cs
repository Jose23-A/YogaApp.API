using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YogaApp.API.Entities; // Asegúrate de tener los usings correctos

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

        // --- LOGIN: El método que te faltaba ---
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UserDto request)
        {
            // 1. Buscar al Usuario por Email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            // 2. Verificar si existe y si la contraseña coincide (usando BCrypt)
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized("Credenciales incorrectas.");
            }

            // 3. MAGIA: Buscar la ficha de ESTUDIANTE asociada a ese email
            // Necesitamos esto porque las reservas se hacen con el StudentId, no con el UserId.
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == request.Email);

            if (student == null)
            {
                return BadRequest("El usuario existe, pero no tiene ficha de alumno asignada.");
            }

            // 4. Devolvemos los datos del alumno (ID, Nombre, Email) al Frontend
            return Ok(student);
        }

        // --- REGISTER: Lo ajustamos levemente para que cree el Estudiante también ---
        [HttpPost("register")]
        public async Task<ActionResult> Register(UserDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("Este correo ya está registrado.");
            }

            // 1. Crear Usuario de Seguridad
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = "Alumno"
            };
            _context.Users.Add(user);

            // 2. Crear Ficha de Estudiante (Para que pueda reservar después)
            // Usamos el mismo email para enlazarlos
            var student = new Student
            {
                Nombre = "Nuevo Usuario", // O podrías pedirlo en el DTO
                Email = request.Email,
                Telefono = ""
            };
            _context.Students.Add(student);

            await _context.SaveChangesAsync();

            return Ok(student); // Devolvemos el estudiante creado
        }
    }

    public class UserDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}