using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YogaApp.API.Entities;

namespace YogaApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly YogaDbContext _context;

        public BookingsController(YogaDbContext context)
        {
            _context = context;
        }

        // POST: api/Bookings
        // Acción: Un alumno intenta reservar un lugar
        [HttpPost]
        public async Task<ActionResult<Booking>> CreateBooking(Booking booking)
        {
            // ---------------------------------------------------------
            // PASO 1: Validar la Clase (Si existe y si hay cupo)
            // ---------------------------------------------------------
            var session = await _context.ClassSessions
                .Include(s => s.Reservas)
                .FirstOrDefaultAsync(s => s.Id == booking.ClassSessionId);

            if (session == null) return NotFound("La clase no existe.");

            if (session.Reservas.Count >= session.CupoMaximo)
                return BadRequest("La clase está llena.");

            if (session.Reservas.Any(r => r.StudentId == booking.StudentId))
                return BadRequest("Ya estás anotado en esta clase.");

            // ---------------------------------------------------------
            // PASO 2: Validar la SUSCRIPCIÓN (EL CAMBIO ESTÁ AQUÍ)
            // ---------------------------------------------------------

            // Buscamos si el alumno tiene una suscripción activa
            // AGREGAMOS .OrderBy(s => s.FechaInicio) para usar siempre la más vieja primero.
            var suscripcion = await _context.Subscriptions
                .Where(s => s.StudentId == booking.StudentId 
                && s.CreditosUsados < s.CreditosTotales) 
                .OrderBy(s => s.FechaInicio) 
                .FirstOrDefaultAsync();

            if (suscripcion == null)
            {
                return BadRequest("El alumno no tiene una suscripción activa o se quedó sin créditos.");
            }

            // ---------------------------------------------------------
            // PASO 3: El algoritmo del Límite Semanal
            // ---------------------------------------------------------

            var fechaClase = session.FechaHoraInicio.Date;

            // Calculamos lunes y domingo de esa semana
            var diaSemana = (int)fechaClase.DayOfWeek;
            if (diaSemana == 0) diaSemana = 7;

            var lunesDeEsaSemana = fechaClase.AddDays(-(diaSemana - 1));
            var domingoDeEsaSemana = lunesDeEsaSemana.AddDays(7);

            // Contamos reservas en esa semana
            var reservasEnSemana = await _context.Bookings
                .Include(b => b.ClassSession)
                .CountAsync(b => b.StudentId == booking.StudentId
                                 && b.ClassSession.FechaHoraInicio >= lunesDeEsaSemana
                                 && b.ClassSession.FechaHoraInicio < domingoDeEsaSemana);

            if (reservasEnSemana >= suscripcion.LimiteClasesPorSemana)
            {
                return BadRequest($"Has alcanzado tu límite de {suscripcion.LimiteClasesPorSemana} clases para esa semana.");
            }

            // ---------------------------------------------------------
            // PASO 4: Si pasó todo, descontamos crédito y guardamos
            // ---------------------------------------------------------
            suscripcion.CreditosUsados++; // Cobramos la entrada

            booking.FechaReserva = DateTime.Now;
            booking.Estado = "Confirmada";

            _context.Bookings.Add(booking);
            // El _context detecta que tocamos 'suscripcion' y 'booking', guarda ambos cambios.
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
        }

        // GET: api/Bookings/5
        // Para ver el comprobante de la reserva
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Student)
                .Include(b => b.ClassSession)
                    .ThenInclude(cs => cs.ClassDefinition)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return Ok(booking);
        }
        // DELETE: api/Bookings/5
        // Acción: Cancelar reserva y devolver crédito
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            // 1. Buscamos la reserva y traemos también los datos del alumno
            // (Necesitamos el StudentId para saber a quién devolverle la plata)
            var booking = await _context.Bookings
                .Include(b => b.Student)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound("La reserva no existe.");

            // 2. Buscamos la suscripción activa de ese alumno para devolverle el crédito
            // Lógica: Buscamos la más nueva o la que tenga saldo usado.
            var suscripcion = await _context.Subscriptions
                .Where(s => s.StudentId == booking.StudentId
                       && s.CreditosUsados > 0) // Solo si tiene algo que devolver
                .OrderByDescending(s => s.FechaInicio) // Devolvemos al bolsillo más reciente
                .FirstOrDefaultAsync();

            // 3. Si encontramos suscripción, hacemos el asiento de reversión
            if (suscripcion != null)
            {
                suscripcion.CreditosUsados--;
            }

            // 4. Borramos el registro físico de la reserva
            _context.Bookings.Remove(booking);

            // 5. COMMIT (Guardamos ambos cambios: el delete y el update de saldo)
            await _context.SaveChangesAsync();

            return Ok("Reserva cancelada y crédito devuelto exitosamente.");
        }
    }
}