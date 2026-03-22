using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YogaApp.API.Entities;
using YogaApp.API.Services; // Para ver el IBookingService
using YogaApp.API.DTOs;     // Para ver el CreateBookingDto

namespace YogaApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly YogaDbContext _context;
        private readonly IBookingService _bookingService; // NUEVO: El servicio

        public BookingsController(YogaDbContext context, IBookingService bookingService)
        {
            _context = context;
            _bookingService = bookingService;
        }

        // POST: api/Bookings
        [HttpPost]
        public async Task<ActionResult<Booking>> CreateBooking(CreateBookingDto bookingDto)
        {
            try
            {
                // Delegamos toda la lógica al servicio profesional que creamos
                var resultado = await _bookingService.CrearReservaAsync(bookingDto);
                return CreatedAtAction(nameof(GetBooking), new { id = resultado.Id }, resultado);
            }
            catch (Exception ex)
            {
                // Si el servicio tira un error (ej: "Sin créditos"), devolvemos el mensaje real
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Student)
                .Include(b => b.ClassSession)
                    .ThenInclude(cs => cs.ClassDefinition)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();
            return Ok(booking);
        }
    }
}