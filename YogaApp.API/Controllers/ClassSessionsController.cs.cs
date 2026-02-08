using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YogaApp.API.Entities;


namespace YogaApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassSessionsController : ControllerBase
    {
        private readonly YogaDbContext _context;
        public ClassSessionsController(YogaDbContext context)
        {
            _context = context;
        }
        // GET: api/ClassSessions
        // Trae todas las clases agendadas (El calendario)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClassSession>>> GetClassSessions()
        {
            return await _context.ClassSessions
                                 .Include(s => s.ClassDefinition)
                                 .ToListAsync();
        }

        // Post: api/ClassSessions
        // Agendar una nueva fecha en el calendario
        [HttpPost]
        public async Task<ActionResult<ClassSession>> CreateSession(ClassSession session)
        {
            // 1. Validar que el tipo de clase exista
            var definitionExists = await _context.ClassDefinitions.AnyAsync(x => x.Id == session.ClassDefinitionId);
            if (!definitionExists)
            {
                return BadRequest($"No existe una definición de Clase con ID {session.ClassDefinitionId}");
            }

            // 2. Validar fechas logicas (no agendar en el pasado)
            if(session.FechaHoraInicio < DateTime.UtcNow)
            {
                return BadRequest("No se puede agendar una clase en el pasado.");
            }

            _context.ClassSessions.Add(session);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClassSessions), new { id = session.Id }, session);
        }
        // GET: api/ClassSessions/reporte-ocupacion
        // Acción: Generar un "Subfile" con el estado de ocupación de las clases futuras
        [HttpGet("reporte-ocupacion")]
        public async Task<ActionResult> GetReporteOcupacion()
        {
            var reporte = await _context.ClassSessions
                // 1. FILTRO (El QRYSLT): Solo clases que no han pasado todavía
                .Where(cs => cs.FechaHoraInicio > DateTime.Now)

                // 2. PROYECCIÓN (La Magia): Aquí creamos la "Hoja de Impresión" o "Subfile"
                // No estamos devolviendo la Entidad completa, creamos una estructura nueva al vuelo.
                .Select(cs => new
                {
                    Clase = cs.ClassDefinition.Nombre,        // Traemos el nombre del archivo maestro
                    Fecha = cs.FechaHoraInicio.ToShortDateString(), // Formateamos la fecha (DD/MM/AAAA)
                    Hora = cs.FechaHoraInicio.ToShortTimeString(),  // Formateamos la hora (HH:MM)
                    CupoTotal = cs.CupoMaximo,

                    // SQL Count() Automático: Cuenta registros en el archivo de Reservas
                    Anotados = cs.Reservas.Count(),

                    // Campo Calculado (RPG Logic): Resta simple
                    LugaresLibres = cs.CupoMaximo - cs.Reservas.Count()
                })
                .ToListAsync();

            return Ok(reporte);
        }
    }
}
