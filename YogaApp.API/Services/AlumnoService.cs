using Microsoft.EntityFrameworkCore;
using YogaApp.API.DTOs;
using YogaApp.API.Entities;

namespace YogaApp.API.Services
{
    public class AlumnoService : IAlumnoService
    {
        private readonly YogaDbContext _context;

        // Inyección de Dependencias:
        // El servicio pide la base de datos para poder trabajar.
        public AlumnoService(YogaDbContext context)
        {
            _context = context;
        }

        public async Task<Student> CrearAlumnoAsync(CreateStudentDto dto)
        {
            // 1. Convertimos DTO (Pantalla) a Entidad (DB)
            var nuevoAlumno = new Student
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                Telefono = string.Empty, // Valor por defecto
                // Bookings se inicializa vacío automáticamente en tu Entidad
            };

            // 2. Aquí podrías validar reglas de negocio
            // Ejemplo: if (ExisteEmail(dto.Email)) throw new Exception("Duplicado");

            // 3. Guardamos en SQL
            _context.Students.Add(nuevoAlumno);
            await _context.SaveChangesAsync();

            return nuevoAlumno;
        }

        public async Task<List<Student>> ObtenerTodosAsync()
        {
            // Simplemente devuelve la lista completa
            return await _context.Students.ToListAsync();
        }
    }
}
