using System.Net.Http.Json;
using YogaApp.API.Entities;

namespace YogaApp.Web.Services
{
    public class ReservaService
    {
        private readonly HttpClient _http;

        public ReservaService(HttpClient http)
        {
            _http = http;
        }

        public async Task<HttpResponseMessage> SolicitarReservaAsync(int sessionId, int studentId)
        {
            var dto = new { ClassSessionId = sessionId, StudentId = studentId };
            return await _http.PostAsJsonAsync("api/Bookings", dto);
        }
    }
}
