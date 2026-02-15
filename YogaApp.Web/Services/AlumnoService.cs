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

        public async Task<bool> CrearAlumnoAsync(CreateStudentDto alumno)
        {
            // ENVÍO (POST):
            // "Oye API, toma este alumno y guárdalo en /api/students"
            var respuesta = await _http.PostAsJsonAsync("api/students", alumno);

            // Si la API responde 201 Created, devolvemos true.
            return respuesta.IsSuccessStatusCode;
        }
    }
}
