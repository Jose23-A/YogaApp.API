using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YogaApp.API.DTOs;
using YogaApp.API.Entities;
using YogaApp.API.Services;

namespace YogaApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        // Ya no dependemos de SQL, dependemos del Servicio (Reglas de Negocio)
        private readonly IAlumnoService _alumnoService;

        public StudentsController(IAlumnoService alumnoService)
        {
            _alumnoService = alumnoService;
        }

        // GET: api/Students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            // El controlador solo dice: "Servicio, dame los datos" y los entrega.
            var alumnos = await _alumnoService.ObtenerTodosAsync();
            return Ok(alumnos);
        }

        // POST: api/Students
        [HttpPost]
        public async Task<ActionResult<Student>> CreateStudent(CreateStudentDto input)
        {
            // El controlador delega toda la lógica de creación al servicio.
            var alumnoCreado = await _alumnoService.CrearAlumnoAsync(input);

            // Retornamos 201 Created
            return CreatedAtAction(nameof(GetStudents), new { id = alumnoCreado.Id }, alumnoCreado);
        }

        // (Nota: He quitado temporalmente el GetStudent(id) individual para que no te de error 
        // hasta que lo agreguemos al servicio, pero con estos dos ya puedes probar).
    }
}