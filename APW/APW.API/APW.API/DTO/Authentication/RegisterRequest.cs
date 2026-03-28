namespace APW.API.DTO.Authentication
{
    public class RegisterRequest
    {
        public string Nombre { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string Contrasena { get; set; } = null!;
    }
}
