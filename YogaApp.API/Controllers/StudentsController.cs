using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YogaApp.API.Entities;

namespace YogaApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly YogaDbContext _context;

        public StudentsController(YogaDbContext context)
        {
            _context = context;
        }

        // GET: api/Students
        // Ver la lista de todos los alumnos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            // Busca en la tabla Students y devuelve la lista completa
            return await _context.Students.ToListAsync();
        }

        // GET: api/Students/5
        // Buscar un alumno específico por su ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        // POST: api/Students
        // Crear un nuevo alumno (Darse de alta)
        [HttpPost]
        public async Task<ActionResult<Student>> CreateStudent(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
        }
    }
}