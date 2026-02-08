namespace YogaApp.API.Entities
{
    public class Booking
    {
        public int Id { get; set; }

        public int ClassSessionId { get; set; }
        public virtual ClassSession? ClassSession { get; set; }

        public int StudentId { get; set; }
        public virtual Student? Student { get; set; }

        public DateTime FechaReserva { get; set; }

        // Podemos guardar el estado: Confirmada, Cancelada, Asistió
        public string Estado { get; set; } = "Confirmada";
    }
}
