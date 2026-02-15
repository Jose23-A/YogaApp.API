using System.Collections.Generic;
using System.Threading.Tasks;
using YogaApp.API.DTOs;
using YogaApp.API.Entities;

namespace YogaApp.API.Services
{
    public interface IAlumnoService
    {
        Task<Student> CrearAlumnoAsync(CreateStudentDto dto);

        Task<List<Student>> ObtenerTodosAsync();
    }
}
