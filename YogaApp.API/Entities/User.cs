namespace YogaApp.API.Entities
{
    public class User
    {
        public int Id { get; set; }

        // El email será el "Username" para el login
        public string Email { get; set; } = string.Empty;

        // Aquí guardaremos la contraseña encriptada (Hash)
        public string PasswordHash { get; set; } = string.Empty;

        // Por ahora manejaremos "Alumno" o "Admin"
        public string Role { get; set; } = "Alumno";

        // Relación opcional: Un usuario puede tener una suscripción
        // Esto conecta tu nueva entidad con la que ya existía
        public UserSubscription? Subscription { get; set; }
    }
}