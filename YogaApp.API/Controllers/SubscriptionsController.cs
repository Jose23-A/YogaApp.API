using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YogaApp.API.Entities;

namespace YogaApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        private readonly YogaDbContext _context;

        public SubscriptionsController(YogaDbContext context)
        {
            _context = context;
        }

        // POST: api/Subscriptions
        // Vender una suscripción a un alumno
        [HttpPost]
        public async Task<ActionResult<UserSubscription>> CreateSubscription(UserSubscription subscription)
        {
            // 1. Validar que el alumno exista
            var studentExists = await _context.Students.AnyAsync(s => s.Id == subscription.StudentId);
            if (!studentExists)
            {
                return BadRequest("El alumno no existe.");
            }

            // 2. Lógica de fechas (Si no manda fechas, asumimos que arranca HOY y dura 30 días)
            if (subscription.FechaInicio == default)
            {
                subscription.FechaInicio = DateTime.Now;
            }

            if (subscription.FechaFin == default)
            {
                subscription.FechaFin = subscription.FechaInicio.AddDays(30);
            }

            // 3. Inicializar contadores
            subscription.CreditosUsados = 0; // Arranca nueva

            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            return Ok(subscription);
        }

        // GET: api/Subscriptions/student/5
        // Ver qué suscripción tiene un alumno específico
        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<UserSubscription>> GetByStudent(int studentId)
        {
            var sub = await _context.Subscriptions
                .Where(s => s.StudentId == studentId && s.FechaFin >= DateTime.Now)
                .OrderByDescending(s => s.FechaFin)
                .FirstOrDefaultAsync();

            if (sub == null) return NotFound("Este alumno no tiene suscripción activa.");

            return Ok(sub);
        }
    }
}