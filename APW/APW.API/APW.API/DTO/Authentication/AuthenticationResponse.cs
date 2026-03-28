namespace APW.API.DTO.Authentication
{
    public class AuthenticationResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public int RolId { get; set; }
        public int EstadoId { get; set; }
    }
}
