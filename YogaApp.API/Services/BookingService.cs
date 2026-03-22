using Microsoft.EntityFrameworkCore;
using YogaApp.API.DTOs;
using YogaApp.API.Entities;

namespace YogaApp.API.Services
{
    public class BookingService : IBookingService
    {
        private readonly YogaDbContext _context;

        public BookingService(YogaDbContext context)
        {
            _context = context;
        }

        public async Task<Booking> CrearReservaAsync(CreateBookingDto dto)
        {
            // Usamos una transacción para que todo sea "todo o nada"
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Validar que la clase exista y tenga cupo
                var session = await _context.ClassSessions
                    .Include(s => s.Reservas)
                    .FirstOrDefaultAsync(s => s.Id == dto.ClassSessionId);

                if (session == null)
                    throw new Exception("La clase no existe.");
                if (session.Reservas.Count >= session.CupoMaximo)
                    throw new Exception("La clase ya está llena.");

                // 2. Validar que el alumno tenga suscripción con créditos
                var sub = await _context.Subscriptions
                    .Where(s => s.StudentId == dto.StudentId && s.CreditosUsados < s.CreditosTotales)
                    .OrderBy(s => s.FechaInicio)
                    .FirstOrDefaultAsync();

                if (sub == null)
                    throw new Exception("No tienes créditos suficientes.");

                // 3. Crear la reserva
                var nuevaReserva = new Booking
                {
                    ClassSessionId = dto.ClassSessionId,
                    StudentId = dto.StudentId,
                    FechaReserva = DateTime.Now,
                    Estado = "Confirmada"
                };

                // 4. Descontar crédito y guaradar
                sub.CreditosUsados++;
                _context.Bookings.Add(nuevaReserva);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return nuevaReserva;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw; // Re-lanzamos la excepción para que el controlador la maneje
            }
        }
    }
}
