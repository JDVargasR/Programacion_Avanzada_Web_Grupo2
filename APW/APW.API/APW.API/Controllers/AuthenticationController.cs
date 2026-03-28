using APW.API.DTO.Authentication;
using APW.Data.Models.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APW.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ProyectoWebGrupo2Context _context;

        private const int ESTADO_ACTIVO_ID = 1;
        private const int ROL_VISITANTE_ID = 2;

        public AuthenticationController(ProyectoWebGrupo2Context context)
        {
            _context = context;
        }

        // POST: api/Authentication/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Correo) || string.IsNullOrWhiteSpace(request.Contrasena))
                return BadRequest("Correo y contraseña son requeridos.");

            var correo = request.Correo.Trim().ToLower();

            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo.ToLower() == correo);

            if (user == null)
                return Unauthorized("Credenciales inválidas.");

            if (user.EstadoId != ESTADO_ACTIVO_ID)
                return Unauthorized("Usuario inactivo.");

            if (user.Contrasena != request.Contrasena)
                return Unauthorized("Credenciales inválidas.");

            var resp = new AuthenticationResponse
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Correo = user.Correo,
                RolId = user.RolId,
                EstadoId = user.EstadoId
            };

            return Ok(resp);
        }

        // POST: api/Authentication/registro
        [HttpPost("registro")]
        public async Task<IActionResult> Registro([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre) ||
                string.IsNullOrWhiteSpace(request.Correo) ||
                string.IsNullOrWhiteSpace(request.Contrasena))
                return BadRequest("Nombre, correo y contraseña son requeridos.");

            var correo = request.Correo.Trim().ToLower();

            var yaExiste = await _context.Usuarios.AnyAsync(u => u.Correo.ToLower() == correo);
            if (yaExiste)
                return Conflict("Ya existe un usuario con ese correo.");

            var nuevo = new Usuario
            {
                Nombre = request.Nombre.Trim(),
                Correo = correo,
                Contrasena = request.Contrasena, 
                RolId = ROL_VISITANTE_ID,         
                EstadoId = ESTADO_ACTIVO_ID,     
                FechaCreacion = DateTime.Now
            };

            _context.Usuarios.Add(nuevo);
            await _context.SaveChangesAsync();

            var resp = new AuthenticationResponse
            {
                Id = nuevo.Id,
                Nombre = nuevo.Nombre,
                Correo = nuevo.Correo,
                RolId = nuevo.RolId,
                EstadoId = nuevo.EstadoId
            };

            return Ok(resp);
        }
    }
}
