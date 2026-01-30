using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YogaApp.API.Entities;

namespace YogaApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassDefinitionsController : ControllerBase
    {
        private readonly YogaAppContext _context;

        public ClassDefinitionsController(YogaAppContext context)
        {
            _context = context;
        }

        // GET: api/ClassDefinitions
        // Trae el catalogo completo de clases disponibles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClassDefinition>>> GetClassDefinitions()
        {
            return await _context.ClassDefinitions.ToListAsync();
        }

        // Post: api/ClassDefinitions
        // Alta de una nueva "Materia" (Ej: Hatha Yoga - Presencial)
        [HttpPost]
        public async Task<ActionResult<ClassDefinition>> PostClassDefinition(ClassDefinition classDef)
        {
            // Validaciones básicas (Business Logic)
            if(classDef.PrecioBase < 0)
            {
                return BadRequest("El precio base no puede ser negativo.");
            }

            _context.ClassDefinitions.Add(classDef);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClassDefinitions", new { id = classDef.Id }, classDef);
        }

        // DELETE: api/ClassDefinitions/5
        // Baja fisica (Cuidado con la integridad de referencia)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClassDefinition(int id)
        {
            var classDef = await _context.ClassDefinitions.FindAsync(id);
            if (classDef == null)
            {
                return NotFound();
            }
            _context.ClassDefinitions.Remove(classDef);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
