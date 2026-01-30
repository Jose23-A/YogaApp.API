namespace YogaApp.API.Entities
{
    public class ClassSession
    {
        public int Id { get; set; }

        // Relación con el Catálogo (Foreign Key)
        public int ClassDefinitionId { get; set; }
        public ClassDefinition? ClassDefinition { get; set; } // El '?' permite que sea nulo momentáneamente

        public DateTime FechaHoraInicio { get; set; }
        public int DuracionMinutos { get; set; }
        public int CupoMaximo { get; set; }

        // Lista de reservas para esta sesión
        public List<Booking> Reservas { get; set; } = new List<Booking>();
    }
}
