namespace YogaApp.Web.DTOs
{
    public class ClassSessionDto
    {
        public int Id { get; set; }
        public DateTime FechaHoraInicio { get; set; }
        public int CupoMaximo { get; set; }
        public ClassDefinitionDto ClassDefinition { get; set; }
    }
}
