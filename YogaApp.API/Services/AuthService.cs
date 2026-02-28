using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using YogaApp.API.Entities;

namespace YogaApp.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly YogaDbContext _context;
        private readonly IConfiguration _config;

        // Inyectamos la Base de Datos y la Configuración (para leer la clave secreta)
        public AuthService(YogaDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                throw new Exception("Credenciales incorrectas.");
            }

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
            if (student == null) throw new Exception("El usuario no tiene ficha de alumno.");

            // GENERAR EL TOKEN JWT
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Guardamos datos adentro del token (Ej: el ID del alumno y su rol)
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim("StudentId", student.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2), // El token dura 2 horas
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Student> RegisterAsync(string email, string password)
        {
            if (await _context.Users.AnyAsync(u => u.Email == email))
                throw new Exception("Este correo ya está registrado.");

            var user = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "Alumno"
            };
            _context.Users.Add(user);

            var student = new Student
            {
                Nombre = "Nuevo Usuario",
                Email = email,
                Telefono = ""
            };
            _context.Students.Add(student);

            await _context.SaveChangesAsync();
            return student;
        }
    }
}
