namespace YogaApp.API.Entities
{
    public enum Modalidad { Presencial, Online }
    public enum TipoClase { Grupal, Individual }
    public class ClassDefinition
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty; // Inicializamos vacío para evitar warnings
        public string Descripcion { get; set; } = string.Empty;
        public Modalidad Modalidad { get; set; }
        public TipoClase Tipo { get; set; }
        public decimal PrecioBase { get; set; }
    }
}
