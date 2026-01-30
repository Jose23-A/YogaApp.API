namespace YogaApp.API.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;

        // Historial de reservas del alumno
        public List<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
