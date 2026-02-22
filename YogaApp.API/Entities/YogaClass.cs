using System.ComponentModel.DataAnnotations;

namespace YogaApp.API.Entities
{
    public class YogaClass
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Instructor { get; set; } = string.Empty;

        // Representa el día: 0=Domingo, 1=Lunes, ..., 6=Sábado
        [Required]
        public DayOfWeek Day { get; set; }

        // TimeSpan guarda solo la hora (ej: 09:00:00)
        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public int MaxCapacity { get; set; }
    }
}
