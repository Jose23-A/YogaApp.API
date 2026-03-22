using YogaApp.API.DTOs;
using YogaApp.API.Entities;

namespace YogaApp.API.Services
{
    public interface IBookingService
    {
        Task<Booking> CrearReservaAsync(CreateBookingDto dto);
    }
}
