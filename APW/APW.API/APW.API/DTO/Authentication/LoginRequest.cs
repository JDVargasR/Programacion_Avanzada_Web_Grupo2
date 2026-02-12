namespace APW.API.DTO.Authentication
{
    public class LoginRequest
    {
        public string Correo { get; set; } = null!;
        public string Contrasena { get; set; } = null!;
    }
}
