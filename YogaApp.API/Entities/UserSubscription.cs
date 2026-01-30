namespace YogaApp.API.Entities
{
    public class UserSubscription
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student? Student { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        // Bolsa de créditos total del mes
        public int CreditosTotales { get; set; }
        public int CreditosUsados { get; set; }

        // TU REGLA DE NEGOCIO: Frecuencia máxima por semana
        public int LimiteClasesPorSemana { get; set; }

        public bool EstaActiva => DateTime.Now <= FechaFin && CreditosUsados < CreditosTotales;
    }
}
