using System.Net.Http.Json; // Necesario para enviar JSON
using YogaApp.Web.DTOs;

namespace YogaApp.Web.Services
{
    public class AlumnoService
    {
        private readonly HttpClient _http;

        // Inyectamos el HttpClient que configuraste en Program.cs
        public AlumnoService(HttpClient http)
        {
            _http = http;
        }

        // Cambiamos el nombre para la prueba y que devuelva HttpResponseMessage
        public async Task<HttpResponseMessage> CrearAlumnoAsyncWithResponse(CreateStudentDto alumno)
        {
            // Asegúrate de que "api/Students" coincida con lo que viste en Swagger
            return await _http.PostAsJsonAsync("api/Students", alumno);
        }
    }
}
