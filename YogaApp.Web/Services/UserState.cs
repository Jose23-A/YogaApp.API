using YogaApp.Web.DTOs; // Asegúrate de que aquí esté tu StudentDto (o CreateStudentDto con ID)

namespace YogaApp.Web.Services
{
    public static class UserState
    {
        // Guardaremos el objeto que representa al alumno conectado
        public static StudentDto? AlumnoLogueado { get; private set; }

        public static bool EstaLogueado => AlumnoLogueado != null;

        public static void Login(StudentDto alumno)
        {
            AlumnoLogueado = alumno;
        }

        public static void Logout()
        {
            AlumnoLogueado = null;
        }
    }
}