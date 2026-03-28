using Microsoft.AspNetCore.Mvc;
using APW.Data.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace APW.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestDbController : ControllerBase
    {
        private readonly ProyectoWebGrupo2Context _context;

        public TestDbController(ProyectoWebGrupo2Context context)
        {
            _context = context;
        }

        [HttpGet("usuarios")]
        public async Task<IActionResult> GetUsuarios()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            return Ok(usuarios);
        }
    }
}
